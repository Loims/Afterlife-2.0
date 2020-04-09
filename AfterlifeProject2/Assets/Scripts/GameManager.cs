using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameObject gamePlane;
    private GameObject progressObstacle;
    private GameObject endObstacle;

    private GameObject progressObstacleInstance;
    private GameObject endObstacleInstance;

    [SerializeField] private GameObject whiteFade;
    [SerializeField] private Color whiteFadeColor;

    [SerializeField] private PlaneMovement planeComp;
    [SerializeField] private PlayerMovement movementComp;
    [SerializeField] private PlayerDeath deathComp;
    [SerializeField] private ObstaclePlacementState obstacleComp;
    [SerializeField] private PlayerFollowTarget followComp;

    public bool hitProgression = false;

    private bool whiteFadeStart;

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
        endObstacle = Resources.Load<GameObject>("CompletionObstacle");

        whiteFade = GameObject.Find("WhiteFade");
        whiteFadeColor = whiteFade.GetComponentInChildren<Image>().color;

        whiteFadeColor.a = 0f;
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

        if(whiteFadeStart)
        {
            FadeInWhite();
        }
    }

    public void SpawnProgressObstacle()
    {
        if (movementComp.playerState != PlayerMovement.State.FLARE)
        {
            progressObstacleInstance = Instantiate(progressObstacle, new Vector3(planeComp.transform.position.x, planeComp.transform.position.y, planeComp.transform.position.z + 150f), Quaternion.identity);
        }
        else
        {
            endObstacleInstance = Instantiate(endObstacle, new Vector3(planeComp.transform.position.x, planeComp.transform.position.y, planeComp.transform.position.z + 150f), Quaternion.identity);
        }
    }

    public void ProgressForm() //Progresses the players form. Called when the player makes it through a stage without colliding
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

    public void ProgressFormWithCollision() //Progresses the players form. Called when the player collides with an obstacle
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

    public void StartSceneChangeCoroutine(bool win)
    {
        if(win)
        {
            StartCoroutine(GameWin(3f));
        }
        else
        {
            StartCoroutine(GameLoss(3f));
        }
    }

    private void FadeInWhite()
    {
        whiteFadeColor.a += Time.deltaTime / 3f;
        whiteFade.GetComponentInChildren<Image>().color = whiteFadeColor;
    }

    private IEnumerator GameWin(float waitTime)
    {
        whiteFadeStart = true;
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(3);
    }

    private IEnumerator GameLoss(float waitTime)
    {
        //Put explosion particle here
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(3);
    }
}
