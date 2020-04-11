using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    public enum State
    {
        NULL,
        WHALE,
        PLANE,
        FLARE
    }

    public State playerState;
    public OnPlayerStateChange onStateChange;

    public float xyspeed = 8f;

    private float objRotationX;
    private float objRotationY;

    private void Awake()
    { 
        if (onStateChange == null)
        {
            onStateChange = new OnPlayerStateChange();
        }
    }

    //Locks the cursor at the start of the game
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerState == State.NULL)
        {
            ChangeStateData();
        }
    }

    /// <summary>
    /// Used to move the player aim target based on mouse input. Player follows the aim target
    /// </summary>
    private void Update()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        LocalMove(h, v, xyspeed);
        ClampPosition();
    }
    
    /// <summary>
    /// Helper method used to move the target
    /// </summary>
    /// <param name="x"> Mouse X input </param>
    /// <param name="y"> Mouse Y input </param>
    /// <param name="speed"> Speed variable </param>
    private void LocalMove(float x, float y, float speed)
    {
        transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime;
    }

    /// <summary>
    /// Clamps the player to the camera frustrum
    /// </summary>
    private void ClampPosition()
    {
        float xClamp = Screen.width / 8;
        float yClamp = Screen.height / 8;

        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Clamp(pos.x, -12f, 12);
        pos.y = Mathf.Clamp(pos.y, -6.5f, 6.5f);
        transform.localPosition = pos;
    }

    /// <summary>
    /// Called when player collides. Changes the player's state to the next state and updates the speed. Invokes an event for analytics tracking
    /// </summary>
    public void ChangeStateData()
    {
        playerState += 1;

        if (playerState == State.WHALE)
        {
            if(xyspeed != 8)
            {
                xyspeed = 8;
            }
        }

        if (playerState == State.PLANE)
        {
            if (xyspeed != 12)
            {
                xyspeed = 12;
            }
        }

        if (playerState == State.FLARE)
        {
            if (xyspeed != 10)
            {
                xyspeed = 10;
            }
        }
        onStateChange.Invoke(playerState);
    }

    /// <summary>
    /// Gets current state
    /// </summary>
    public State GetCurrentState()
    {
        return playerState;
    }
}

public class OnPlayerStateChange : UnityEvent<PlayerMovement.State> { }
