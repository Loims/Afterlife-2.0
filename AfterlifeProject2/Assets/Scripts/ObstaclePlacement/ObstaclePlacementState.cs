using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacementState : MonoBehaviour
{
    #region GenerationVariables
    private static ObstaclePlacementState instance;

    private PlayerMovement playerMovementScript;
    private PlaneMovement planeMovementScript;

    [SerializeField] private PlayerMovement.State playerGenerationState;

    public OceanPlacement oceanScript;
    public SkyPlacement skyScript;
    public CavePlacement caveScript;
    #endregion

    /// <summary>
    /// Generates an instance of this object and initializes variables
    /// </summary>
    private void OnEnable()
    {
        //Instance generation
        InstantiateSingleton();

        playerMovementScript = transform.parent.GetComponentInChildren<PlayerMovement>();
        playerGenerationState = playerMovementScript.playerState;

        planeMovementScript = transform.parent.GetComponent<PlaneMovement>();

        oceanScript = GetComponent<OceanPlacement>();
        skyScript = GetComponent<SkyPlacement>();
        caveScript = GetComponent<CavePlacement>();
    }

    /// <summary>
    /// Helper method for instantiating singleton
    /// </summary>
    private void InstantiateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Handles state machine for obstacle generation
    /// </summary>
    private void Update()
    {
        if(playerGenerationState != playerMovementScript.playerState)
        {
            playerGenerationState = playerMovementScript.playerState;
        }

        GenerationStateMachine();
    }
    
    /// <summary>
    /// Handles the enabling and disabling of generation scripts
    /// </summary>
    private void GenerationStateMachine()
    {
        switch (playerGenerationState)
        {
            case PlayerMovement.State.WHALE:
                oceanScript.enabled = true;
                skyScript.enabled = false;
                caveScript.enabled = false;
                break;

            case PlayerMovement.State.PLANE:
                oceanScript.enabled = false;
                skyScript.enabled = true;
                caveScript.enabled = false;
                break;

            case PlayerMovement.State.FLARE:
                oceanScript.enabled = false;
                skyScript.enabled = false;
                caveScript.enabled = true;
                break;
        }
    }

    /// <summary>
    /// Clears all objects from sorting objects, clearing the level for the next generation algorithm
    /// </summary>
    public void ClearObjectsInChildren()
    {
        switch (playerGenerationState)
        {
            case PlayerMovement.State.WHALE:
                oceanScript.ClearObjects();
                break;

            case PlayerMovement.State.PLANE:
                skyScript.ClearObjects();
                break;

            case PlayerMovement.State.FLARE:
                //caveScript.ClearObjects();
                break;
        }
    }
}
