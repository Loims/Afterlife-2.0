using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.5f;

    private PostProcessProfile oceanProfile;
    private PostProcessProfile skyProfile;
    private PostProcessProfile caveProfile;

    [SerializeField] private PlayerMovement movementComp;

    public Vector3 offset = new Vector3(0, 3, -10);

    private void OnEnable()
    {
        target = transform.parent.GetComponentInChildren<PlayerFollowTarget>().gameObject.transform;

        movementComp = transform.parent.GetChild(2).GetComponentInChildren<PlayerMovement>();

        oceanProfile = Resources.Load<PostProcessProfile>("OceanProfile");
        skyProfile = Resources.Load<PostProcessProfile>("SkyProfile");
        caveProfile = Resources.Load<PostProcessProfile>("CaveProfile");
    }

    private void Update()
    {
        switch(movementComp.playerState)
        {
            case PlayerMovement.State.WHALE:
                Camera.main.GetComponent<PostProcessVolume>().profile = oceanProfile;
                break;

            case PlayerMovement.State.PLANE:
                Camera.main.GetComponent<PostProcessVolume>().profile = skyProfile;
                break;

            case PlayerMovement.State.FLARE:
                Camera.main.GetComponent<PostProcessVolume>().profile = caveProfile;
                break;
        }
    }

    /// <summary>
    /// Very simple camera follow script. Follows the player with a contrain to prevent the player from exiting the play space
    /// </summary>
    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPos;
    }
}
