using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowTarget : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private PlaneMovement planeComp;
    private PlayerMovement movementComp;
    private PlayerDeath deathComp;
    private ObstaclePlacementState obstacleComp;

    [SerializeField] private Transform target;

    private GameObject progressionObstacle;

    [SerializeField] private GameObject whaleVisual;
    [SerializeField] private GameObject planeVisual;
    [SerializeField] private GameObject flareVisual;

    private PlayerMovement.State playerState;

    private float smoothSpeed;
    private float rotateSpeed;
    private float rollSpeed;
    private float rollProgress;

    private bool barrelRolling = false;
    private bool rollLeft = false;
    private bool rollRight = false;

    public float formTimer;
    private bool formTimerProgress = true;
    private bool progressionTriggerSpanwed = false;

    [SerializeField] public bool hasHitObstacle = false;

    //TEMP
    private bool spawnedEnd = false;

    /// <summary>
    /// Instantiate component references and variables
    /// </summary>
    private void OnEnable()
    {
        gameManager = GameManager.instance;

        planeComp = transform.parent.GetComponent<PlaneMovement>();
        movementComp = transform.parent.GetComponentInChildren<PlayerMovement>();
        deathComp = transform.parent.GetComponentInChildren<PlayerDeath>();
        obstacleComp = transform.parent.GetComponentInChildren<ObstaclePlacementState>();

        progressionObstacle = Resources.Load<GameObject>("ProgressionObstacle");

        target = transform.parent.GetChild(2).transform;
        smoothSpeed = 0.125f;
        rotateSpeed = 2f;
        rollSpeed = 800f;

        formTimer = 0f;

        whaleVisual = transform.GetChild(0).GetChild(0).gameObject;
        planeVisual = transform.GetChild(0).GetChild(1).gameObject;
        flareVisual = transform.GetChild(0).GetChild(2).gameObject;

        playerState = movementComp.playerState;
    }

    /// <summary>
    /// Update used for allowing barrel rolling in the plane state
    /// </summary>
    private void Update()
    {
        //Allows barrel rolling to execute if in plane state
        if (movementComp.GetCurrentState() == PlayerMovement.State.PLANE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!barrelRolling)
                {
                    if (!rollLeft)
                    {
                        rollLeft = true;
                        barrelRolling = true;
                        movementComp.xyspeed = 20f;
                    }
                }
            }

            //Initiate barrel roll on mouse press
            if (Input.GetMouseButtonDown(1))
            {
                if (!barrelRolling)
                {
                    if (!rollRight)
                    {
                        rollRight = true;
                        barrelRolling = true;
                        movementComp.xyspeed = 20f;
                    }
                }
            }
        }

        UpdateVisual();
    }

    /// <summary>
    /// Controls player movement and form timing. When form timer reaches certain value, form naturally progresses. 
    /// </summary>
    private void FixedUpdate()
    {
        if (formTimerProgress)
        {
            formTimer += Time.deltaTime;
        }

        if(formTimer >= 120f)
        {
            formTimerProgress = false;
        }

        if (!formTimerProgress)
        {
            if (!progressionTriggerSpanwed)
            {
                gameManager.SpawnProgressObstacle();
                progressionTriggerSpanwed = true;
            }
        }

        MoveToTarget();
        RotateToTarget();
        if(rollLeft)
        {
            RollLeft();
        }
        if(rollRight)
        {
            RollRight();
        }
    }

    private void UpdateVisual()
    {
        playerState = movementComp.playerState;

        switch(playerState)
        {
            case PlayerMovement.State.WHALE:
                whaleVisual.SetActive(true);
                planeVisual.SetActive(false);
                flareVisual.SetActive(false);
                break;

            case PlayerMovement.State.PLANE:
                whaleVisual.SetActive(false);
                planeVisual.SetActive(true);
                flareVisual.SetActive(false);
                break;

            case PlayerMovement.State.FLARE:
                whaleVisual.SetActive(false);
                planeVisual.SetActive(false);
                flareVisual.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Moves the player object to the invisible mouse target. Used to get smooth moving.
    /// </summary>
    private void MoveToTarget()
    {
        Vector3 desiredPos = new Vector3(target.position.x, target.position.y, target.position.z - 2);
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothedPos;
    }

    /// <summary>
    /// Keeps the player object rotated towards the invisible mouse target. Used alongside MoveToTarget()
    /// </summary>
    private void RotateToTarget()
    {
        Vector3 targetPosition = target.transform.position - transform.position;
        float rotateStep = rotateSpeed * Time.deltaTime;

        Vector3 direction = Vector3.RotateTowards(transform.forward, targetPosition, rotateStep, 0f);
        transform.rotation = Quaternion.LookRotation(direction);
        
    }

    /// <summary>
    /// Used to roll the player left when a barrel roll is initiated
    /// </summary>
    private void RollLeft()
    {
        rollProgress += rollSpeed * Time.deltaTime;
        transform.Rotate(0, 0, rollProgress, Space.Self);
        if(rollProgress >= 360)
        {
            rollLeft = false;
            barrelRolling = false;
            rollProgress = 0;
            movementComp.xyspeed = 12f;
        }
    }

    /// <summary>
    /// Used to roll the player right when a barrel roll is initiated
    /// </summary>
    private void RollRight()
    {
        rollProgress -= rollSpeed * Time.deltaTime;
        transform.Rotate(0, 0, rollProgress, Space.Self);
        if (rollProgress <= -360)
        {
            rollRight = false;
            barrelRolling = false;
            transform.Rotate(0, 0, 0);
            rollProgress = 0;
            movementComp.xyspeed = 12f;
        }
    }

    /// <summary>
    /// Collision event that is called when the player collides. Performs form changing, plane resetting, and has events for death in the future
    /// </summary>
    private void CollisionEvent(bool collision)
    {
        if (!hasHitObstacle)
        {
            hasHitObstacle = true;
        }

        if (collision)
        {
            gameManager.ProgressFormWithCollision();
        }
        else
        {
            gameManager.ProgressForm();
        }
    }

    public void ResetBools()
    {
        formTimerProgress = true;
        progressionTriggerSpanwed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ProgressObstacle")
        {
            if (other.name != "PlayerVisualTrigger")
            {
                CollisionEvent(false);
            }
        }
        else if (other.tag == "MainCliff")
        {
            CollisionEvent(true);
        }
        else if(other.tag == "SkyObstacle")
        {
            CollisionEvent(true);
        }
        else if(other.tag == "CaveObstacle")
        {
            gameManager.StartSceneChangeCoroutine(false);
        }
        else if(other.tag == "EndObstacle")
        {
            gameManager.StartSceneChangeCoroutine(true);
        }
    }
}
