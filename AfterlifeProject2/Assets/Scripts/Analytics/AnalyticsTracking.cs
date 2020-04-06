using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AnalyticsTracking : MonoBehaviour
{
    [Header("Save Properties")]
    public string savePath = "";
    public string saveFileName = "";
    public string filePath = "";
    public AnalyticsData bigData;


    private float timeStarted = 0;
    private float[] timeStateChanged;

    [Space]
    [Header("Movement Tracking Properties")]
    public float tickRateForTrackingMovement = 0.25f;
    public float trackingMovementIntervals = 10f;

    [Space]
    [Header("Raycast Collision Properties")]
    public LayerMask obstacleMask;
    public int raycastCount = 8;
    public float raycastAngleInterval = 45f;
    public float raycastDistance = 10f;
    public float raycastHitCooldown = 1f;
    private bool hitRecently = false;

    [Space]
    [Header("References")]
    public GameObject playerObject;
    public PlayerMovement playerMovement;

    private bool canSetTimeToFirstCollision = true;

    private void OnEnable()
    {
        if (playerMovement != null)
        {
            if (playerMovement.onStateChange == null)
            {
                playerMovement.onStateChange = new OnPlayerStateChange();
            }

            if (playerMovement.onStateChange != null)
            {
                playerMovement.onStateChange.AddListener(ChangeForm);
            }
        }
    }

    private void OnDisable()
    {
        if (playerMovement != null)
        {
            playerMovement.onStateChange.RemoveListener(ChangeForm);
        }
    }

    private void Awake()
    {
        bigData = new AnalyticsData();
        timeStateChanged = new float[3];
        timeStarted = Time.time;
        CreateAnalytics();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TrackPlayerMovement());
        StartCoroutine(RaycastForCollisions());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            UpdateData();
            SaveAnalytics(bigData, filePath);

            SavePretty(bigData);
        }

        if(canSetTimeToFirstCollision)
        {
            if(playerObject.GetComponentInChildren<PlayerFollowTarget>().hasHitObstacle)
            {
                bigData.timeToFirstCollision = Time.time;
                canSetTimeToFirstCollision = false;
            }
        }
    }

    public void RaycastCardinal()
    {
        if (playerObject != null)
        {
            float startAngle = Mathf.Atan2(Vector3.up.y, Vector3.up.z) * Mathf.Rad2Deg;
            for (int i = 0; i < raycastCount; i++)
            {
                if (hitRecently)
                {
                    continue;
                }

                float newAngle = startAngle + (raycastAngleInterval * i);
                newAngle *= Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Cos(newAngle), Mathf.Sin(newAngle), 0);
                RaycastHit hit;

                Physics.Raycast(playerObject.transform.position, direction, out hit, raycastDistance, obstacleMask.value);
                Debug.DrawRay(playerObject.transform.position, direction * raycastDistance, Color.red);

                if (hit.collider != null)
                {
                    hitRecently = true;
                    if (bigData != null)
                    {
                        if (bigData.distanceToObstacles != null)
                        {
                            bigData.distanceToObstacles.Add(hit.distance);
                            //if (bigData.timeToFirstCollision == 0)
                            //{
                            //    bigData.timeToFirstCollision = Time.time - timeStarted;
                            //}
                        }
                    }
                }
            } 
        }
    }

    //Since Flare is not implemented, we'll manually say that the time as a plane was [CurrentTime] - [TimeYouBecameAPlane]
    public void ChangeForm(PlayerMovement.State newForm) //Receive the new state the player has changed to
    {
        if (bigData != null)
        {
            int index = 0;
            switch (newForm)
            {
                case PlayerMovement.State.WHALE:
                    index = 0;
                    break;
                case PlayerMovement.State.PLANE:
                    index = 1;
                    break;
                case PlayerMovement.State.FLARE: //This will not work until Flare is implemented
                    index = 2;
                    break;
            }

            timeStateChanged[index] = Time.time;
            if (bigData.timeSpentInForm != null)
            {
                if (index != 0) //If not Whale
                {
                    index -= 1;
                    bigData.timeSpentInForm[index] = Time.time - timeStateChanged[index];
                }
            }
        }
    }

    public IEnumerator TrackPlayerMovement()
    {
        for(; ;)
        {
            List<Vector3> positions = new List<Vector3>();
            List<float> movementFromLastPosition = new List<float>();

            //Intervals of trackingInterval second averages
            int checkCount = Mathf.CeilToInt(trackingMovementIntervals / tickRateForTrackingMovement);
            for (int i = 0; i < checkCount; i++)
            {
                if (playerObject != null)
                {
                    Vector3 fixedPos = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, 0);
                    if (i != 0)
                    {
                        float amountPlayerMoved = Vector3.Distance(positions[i - 1], fixedPos);
                        movementFromLastPosition.Add(amountPlayerMoved);
                        positions.Add(fixedPos);
                    }
                    else //If first element
                    {
                        positions.Add(fixedPos);
                    }
                }
                yield return new WaitForSeconds(tickRateForTrackingMovement);
            }

            //Once done, calculate the average
            float total = 0;
            foreach(float mov in movementFromLastPosition)
            {
                total += mov;
            }
            float average = total / movementFromLastPosition.Count;

            //Round to 0 if small enough
            average = average < 0.001f ? 0 : average;
            total = total < 0.001f ? 0 : total;

            //Assign to bigData
            if (bigData != null)
            {
                bigData.averageAmountMoved.Add(average);
                bigData.amountMoved.Add(total);
            }
            yield return new WaitForSeconds(tickRateForTrackingMovement);
        }
    }

    public IEnumerator RaycastForCollisions()
    {
        for(; ;)
        {
            if (hitRecently)
            {
                yield return new WaitForSeconds(raycastHitCooldown);
                hitRecently = false;
            }
            else
            {
                RaycastCardinal();
            }
            yield return new WaitForFixedUpdate();
        }
    }
       
    #region Saving / Loading

    public void CreateAnalytics()
    {
        //Get Folder Path
        // ! Change to Application.persistentDataPath for final build and update variables in Inspector
        string path = Application.dataPath + savePath;

        //Assign Name - dynamic so that it will be 'Cup 1', 'Cup 2', and so on
        saveFileName = saveFileName + (Directory.GetFiles(path, "*.json").Length + 1).ToString();

        //Update Path
        path = path + saveFileName + ".json";

        Debug.Log(path);
        filePath = path;
        SaveAnalytics(bigData, path);
    }

    public void UpdateData() //Calculate all your averages and time played. CALL BEFORE END OF GAME AND SAVING
    {
        if (bigData != null)
        {
            bigData.timePlayed = Time.time - timeStarted;
        }
    }

    public void SaveAnalytics(AnalyticsData analytics, string path)
    {
        //Save
        string saveContent = JsonUtility.ToJson(analytics, true);
        File.WriteAllText(path, saveContent);
    }

    public void SavePretty(AnalyticsData analytics)
    {
        string pathForPretty = Application.dataPath + savePath + saveFileName + ".txt";
        SavePretty(bigData, pathForPretty);
    }

    public void SavePretty(AnalyticsData analytics, string path)
    {
        StreamWriter writer = new StreamWriter(path, true);

        // Time Played
        writer.WriteLine("Time Played:");
        writer.WriteLine(analytics.timePlayed);
        writer.WriteLine("\n");

        // timeToFirstCollision
        writer.WriteLine("timeToFirstCollision:");
        writer.WriteLine(analytics.timeToFirstCollision);
        writer.WriteLine("\n");

        // distanceToObstacles
        writer.WriteLine("distanceToObstacles:");
        for (int i = 0; i < analytics.distanceToObstacles.Count; i++)
        {
            writer.WriteLine(analytics.distanceToObstacles[i].ToString()); 
        }
        writer.WriteLine("\n");

        // timeSpentInForm
        writer.WriteLine("timeSpentInForm:");
        for (int i = 0; i < analytics.timeSpentInForm.Length; i++)
        {
            writer.WriteLine(analytics.timeSpentInForm[i].ToString());
        }
        writer.WriteLine("\n");

        // averageAmountMoved
        writer.WriteLine("averageAmountMoved:");
        for (int i = 0; i < analytics.averageAmountMoved.Count; i++)
        {
            writer.WriteLine(analytics.averageAmountMoved[i].ToString());
        }
        writer.WriteLine("\n");

        // amountMoved
        writer.WriteLine("amountMoved:");
        for (int i = 0; i < analytics.amountMoved.Count; i++)
        {
            writer.WriteLine(analytics.amountMoved[i].ToString());
        }
        writer.WriteLine("\n");

        writer.Close();
        Debug.Log("Done writing!");
    }

    #endregion


}
[System.Serializable]
public class AnalyticsData
{
    public float timePlayed;
    public float timeToFirstCollision = 0;

    [Space]

    public List<float> distanceToObstacles;
    public float[] timeSpentInForm;
    public List<float> averageAmountMoved;
    public List<float> amountMoved;
    
    public AnalyticsData()
    {
        distanceToObstacles = new List<float>();
        timeSpentInForm = new float[3];
        averageAmountMoved = new List<float>();
        amountMoved = new List<float>();
    }
}
