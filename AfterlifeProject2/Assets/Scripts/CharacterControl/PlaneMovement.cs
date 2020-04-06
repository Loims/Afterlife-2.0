using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMovement : MonoBehaviour
{
    private PlayerMovement movementComp;
    private PlayerFollowTarget followComp;

    public float moveSpeed;

    public float speedMultiplier;

    private void OnEnable()
    {
        movementComp = GetComponentInChildren<PlayerMovement>();
        followComp = GetComponentInChildren<PlayerFollowTarget>();
        speedMultiplier = 0.5f;
    }

    /// <summary>
    /// Moves the gameplay plane forward with a constant speed based on the player state
    /// </summary>
    void Update()
    {
        float formTimer = followComp.formTimer;
        if (movementComp.playerState == PlayerMovement.State.WHALE)
        {
            moveSpeed = 12f * speedMultiplier;
        }
        else if (movementComp.playerState == PlayerMovement.State.PLANE)
        {
            moveSpeed = 30f * speedMultiplier;
        }
        else if (movementComp.playerState == PlayerMovement.State.FLARE)
        {
            moveSpeed = 20f * speedMultiplier;
        }
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        speedMultiplier = IncreaseMultiplier(formTimer, moveSpeed);
    }


    private float IncreaseMultiplier(float timer, float mSpeed)
    {
        float multiplier;

        if (timer < 30f)
        {
            multiplier = 30f / 60f;
        }
        else
        {
            multiplier = timer / 60f;
        }
        return multiplier;
    }

    /// <summary>
    /// Resets the position of the plane back to Vector3.zero
    /// </summary>
    public void ResetPlane()
    {
        transform.position = Vector3.zero;
    }
}
