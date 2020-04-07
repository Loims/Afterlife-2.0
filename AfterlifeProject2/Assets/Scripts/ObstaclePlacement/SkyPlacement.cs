using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyPlacement : MonoBehaviour
{
    [Header("Plane Transform")]
    [SerializeField] private Transform planeTransform;

    [Space]
    [Header("Plane Script")]
    [SerializeField] private PlaneMovement planeComp;

    [Space]
    [Header("Object Pooler")]
    [SerializeField] private ObjectPooler pooler;

    private List<GameObject> cloudList;
    [SerializeField] private List<GameObject> obstacleList;

    private GameObject skyParent;

    #region CloudVars
    [SerializeField] private GameObject cloud1;
    [SerializeField] private GameObject cloud2;
    [SerializeField] private GameObject cloud3;
    [SerializeField] private GameObject cloud4;
    [SerializeField] private GameObject cloud5;
    [SerializeField] private GameObject cloud6;
    [SerializeField] private GameObject cloud7;
    [SerializeField] private GameObject cloud8;
    #endregion

    #region ObstacleVars
    [SerializeField] private GameObject mountain1;
    [SerializeField] private GameObject mountain2;
    [SerializeField] private GameObject mountain3;
    [SerializeField] private GameObject mountain4;
    [SerializeField] private GameObject mountain5;
    [SerializeField] private GameObject hotAirBalloon1;
    [SerializeField] private GameObject hotAirBalloon2;
    [SerializeField] private GameObject hotAirBalloon3;
    [SerializeField] private GameObject kite;
    [SerializeField] private GameObject pillar1;
    [SerializeField] private GameObject pillar2;
    #endregion

    [SerializeField] private int floorIncrement = 0;
    private int recentObjVariant;
    private int recentMntnVariant;

    private float obsDelay = 1.5f;
    private float mountainDelay = 3.5f;

    private float obsTimer = 0f;
    private float mountainTimer = 0f;
    
    /// <summary>
    /// Instantiates prefabs and variables
    /// </summary>
    private void OnEnable()
    {
        InstantiatePrefabs();

        planeTransform = transform.parent;
        planeComp = transform.parent.GetComponent<PlaneMovement>();

        obstacleList = new List<GameObject>();
    }

    /// <summary>
    /// Sets sky parent to a sorting object and gets object pooler
    /// </summary>
    private void Start()
    {
        skyParent = GameObject.Find("SkyObjects");
        pooler = ObjectPooler.instance;

        //for (int i = 0; i < 3; i++)
        //{
        //    GameObject cloud = GenerateRandomCloud()
        //    cloudList.Add(pooler.NewObject(oceanFloor, new Vector3(planeTransform.position.x, planeTransform.position.y - 8f, (i * oceanFloor.transform.localScale.z) * 10), Quaternion.identity));
        //    objectList[i].transform.parent = oceanParent.transform;
        //    floorIncrement++;
        //}
    }

    private void Update()
    {
        foreach (GameObject obj in obstacleList)
        {
            if (obj.transform.position.z <= planeTransform.position.z - 2f)
            {
                ReturnObjToPool(obj);
            }
        }

        SpawnMountains(mountainDelay);
        SpawnObs(obsDelay);
    }

    /// <summary>
    /// Helper methods for instantiating prefabs
    /// </summary>
    private void InstantiatePrefabs()
    {
        cloud1 = Resources.Load<GameObject>("Area2.Cloud1");
        cloud2 = Resources.Load<GameObject>("Area2.Cloud2");
        cloud3 = Resources.Load<GameObject>("Area2.Cloud3");
        cloud4 = Resources.Load<GameObject>("Area2.Cloud4");
        cloud5 = Resources.Load<GameObject>("Area2.Cloud5");
        cloud6 = Resources.Load<GameObject>("Area2.Cloud6");
        cloud7 = Resources.Load<GameObject>("Area2.Cloud7");
        cloud8 = Resources.Load<GameObject>("Area2.Cloud8");

        mountain1 = Resources.Load<GameObject>("Area2.Mountain1");
        mountain2 = Resources.Load<GameObject>("Area2.Mountain2");
        mountain3 = Resources.Load<GameObject>("Area2.Mountain3");
        mountain4 = Resources.Load<GameObject>("Area2.Mountain4");
        mountain5 = Resources.Load<GameObject>("Area2.Mountain5");
        hotAirBalloon1 = Resources.Load<GameObject>("Area2.HotAirBalloon1");
        hotAirBalloon2 = Resources.Load<GameObject>("Area2.HotAirBalloon2");
        hotAirBalloon3 = Resources.Load<GameObject>("Area2.HotAirBalloon3");
        kite = Resources.Load<GameObject>("Area2.Kite");
        pillar1 = Resources.Load<GameObject>("Area2.Pillar1");
        pillar2 = Resources.Load<GameObject>("Area2.Pillar2");

    }

    /// <summary>
    /// Helper to return objects to pool
    /// </summary>
    private void ReturnObjToPool(GameObject obj)
    {
        pooler.ReturnToPool(obj);
    }

    private void SpawnMountains(float waitTime)
    {
        mountainTimer += Time.deltaTime;
        if(mountainTimer >= waitTime)
        {
            DetermineMountain();
            mountainTimer = 0f;
        }
    }

    /// <summary>
    /// Coroutine for spawning objects
    /// </summary>
    private void SpawnObs(float waitTime)
    {
        obsTimer += Time.deltaTime;
        if (obsTimer >= waitTime)
        {
            DetermineObstacle();
            obsTimer = 0f;
        }
    }

    private void DetermineMountain()
    {
        GameObject mountainVariant;
        int mntnVariantInt = Random.Range(0, 5);

        while(mntnVariantInt == recentMntnVariant)
        {
            mntnVariantInt = Random.Range(0, 5);
        }
        recentMntnVariant = mntnVariantInt;

        switch(mntnVariantInt)
        {
            case 0:
                mountainVariant = mountain1;
                break;

            case 1:
                mountainVariant = mountain2;
                break;

            case 2:
                mountainVariant = mountain3;
                break;

            case 3:
                mountainVariant = mountain4;
                break;

            case 4:
                mountainVariant = mountain5;
                break;

            default:
                mountainVariant = mountain1;
                break;
        }

        SpawnAtRandomSpot(mountainVariant);
    }

    /// <summary>
    /// Method for spawning different object variants. Decides which object to spawn then calls helper method
    /// </summary>
    private void DetermineObstacle()
    {
        GameObject objVariant;
        int objVariantInt = Random.Range(0, 6);

        while (objVariantInt == recentObjVariant)
        {
            objVariantInt = Random.Range(0, 6);
        }
        recentObjVariant = objVariantInt;

        switch (objVariantInt)
        {
            case 0:
                objVariant = pillar1;
                break;

            case 1:
                objVariant = pillar2;
                break;

            case 2:
                objVariant = hotAirBalloon1;
                break;

            case 3:
                objVariant = hotAirBalloon2;
                break;

            case 4:
                objVariant = hotAirBalloon3;
                break;

            case 5:
                objVariant = kite;
                break;

            default:
                objVariant = hotAirBalloon1;
                break;
        }

        SpawnAtRandomSpot(objVariant);
    }

    //private GameObject GenerateRandomCloud()
    //{
    //    int randomCloudInt = Random.Range(0, 8);

    //    switch(randomCloudInt)
    //    {
    //        case 0:

    //    }
    //}

    /// <summary>
    /// Helper method to generate spawn points. Spawn points are generated based on the passed in object
    /// </summary>
    private void SpawnAtRandomSpot(GameObject obj)
    {
        if (obj == mountain1 || obj == mountain2)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-17f, 17f), -25.8f, planeTransform.position.z + (100f + (20 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(obj, spawnPos, spawnRot);
            newObj.transform.parent = skyParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }

        else if(obj == mountain3 || obj == mountain4 || obj == mountain5)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-17f, 17f), -25.8f, planeTransform.position.z + (100f + (20 * planeComp.speedMultiplier)));
            GameObject newObj = pooler.NewObject(obj, spawnPos, Quaternion.identity);
            newObj.transform.parent = skyParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }

        else if (obj == hotAirBalloon1 || obj == hotAirBalloon2 || obj == hotAirBalloon3)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-17f, 17f), Random.Range(-10f, 3.5f), planeTransform.position.z + (100f + (20 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(0, 0, 0);
            GameObject newObj = pooler.NewObject(obj, spawnPos, spawnRot);
            newObj.transform.parent = skyParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }

        else if(obj == pillar1)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-17f, 17f), -18.28f, planeTransform.position.z + (100f + (20 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(obj, spawnPos, spawnRot);
            newObj.transform.parent = skyParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }

        else if(obj == pillar2)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-17f, 17f), -18.28f, planeTransform.position.z + (100f + (20 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(0, -90, 90);
            GameObject newObj = pooler.NewObject(obj, spawnPos, spawnRot);
            newObj.transform.parent = skyParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
    }

    /// <summary>
    /// Clears all objects from object sorting gameobject
    /// </summary>
    public void ClearObjects()
    {
        obstacleList.Clear();
        StopAllCoroutines();

        foreach (Transform child in skyParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
