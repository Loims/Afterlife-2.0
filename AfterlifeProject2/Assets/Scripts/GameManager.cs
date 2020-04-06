using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameObject gamePlane;
    private GameObject progressObstacle;

    private GameObject progressObstacleInstance;

    [SerializeField] private PlaneMovement planeComp;
    [SerializeField] private PlayerMovement movementComp;
    [SerializeField] private PlayerDeath deathComp;
    [SerializeField] private ObstaclePlacementState obstacleComp;
    [SerializeField] private PlayerFollowTarget followComp;

    public bool hitProgression = false;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        gamePlane = GameObject.Find("Gameplay Plane");

        planeComp = gamePlane.GetComponentInChildren<PlaneMovement>();
        movementComp = gamePlane.GetComponentInChildren<PlayerMovement>();
        deathComp = gamePlane.GetComponentInChildren<PlayerDeath>();
        obstacleComp = gamePlane.GetComponentInChildren<ObstaclePlacementState>();
        followComp = gamePlane.GetComponentInChildren<PlayerFollowTarget>();

        progressObstacle = Resources.Load<GameObject>("ProgressionObstacle");
    }

    private void Update()
    {
        if (progressObstacleInstance != null)
        {
            if (hitProgression)
            {
                if (Mathf.Abs((progressObstacleInstance.transform.position - planeComp.transform.position).magnitude) >= 40f)
                {
                    Destroy(progressObstacleInstance);
                    hitProgression = false;
                }
            }
        }
    }

    public void SpawnProgressObstacle()
    {
        if (movementComp.playerState != PlayerMovement.State.FLARE)
        {
            progressObstacleInstance = Instantiate(progressObstacle, new Vector3(planeComp.transform.position.x, planeComp.transform.position.y, planeComp.transform.position.z + 150f), Quaternion.identity);
        }
    }

    public void ProgressForm()
    {
        Debug.Log("THIS IS CALLED");
        MoveProgressionObstacle();
        planeComp.ResetPlane();
        movementComp.ChangeStateData();
        deathComp.DeathEvent();
        obstacleComp.ClearObjectsInChildren();
        followComp.formTimer = 0f;
        followComp.ResetBools();
    }

    public void ProgressFormWithCollision()
    {
        if(progressObstacleInstance != null)
        {
            Destroy(progressObstacleInstance);
        }
        planeComp.ResetPlane();
        movementComp.ChangeStateData();
        deathComp.DeathEvent();
        obstacleComp.ClearObjectsInChildren();
        followComp.formTimer = 0f;
        followComp.ResetBools();
    }

    private void MoveProgressionObstacle()
    {
        Vector3 offset = progressObstacleInstance.transform.position - planeComp.transform.position;
        progressObstacleInstance.transform.position = Vector3.zero + offset;
        hitProgression = true;
    }

    /// <summary>
    /// Temp method. Ends the game with a loss
    /// </summary>
    public void StopGameWithLoss()
    {
        Debug.Log("YOU LOSE");
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Temp method. Ends the game with a win
    /// </summary>
    public void StopGameWithWin()
    {
        Debug.Log("YOU WIN");
        Time.timeScale = 0f;
    }
}
