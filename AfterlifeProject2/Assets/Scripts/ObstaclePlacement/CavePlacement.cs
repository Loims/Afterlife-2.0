using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavePlacement : MonoBehaviour
{
    /// <summary>
    /// THIS ENTIRE SCRIPT IS TEMPORARY NOW. IT MUST BE CHANGED IN THE FUTURE
    /// SCRIPT IS IDENTICAL TO THE OCEAN PLACEMENT SCRIPT EXCEPT VARIABLE NAMES
    /// AND ADDS CEILING GENERATION
    /// </summary>
    [Header("Plane Transform")]
    [SerializeField] private Transform planeTransform;

    [Space]
    [Header("Object Pooler")]
    [SerializeField] private ObjectPooler pooler;

    private List<GameObject> objectList;
    [SerializeField] private List<GameObject> obstacleList;

    private GameObject caveParent;

    [SerializeField] private GameObject caveFloor;
    [SerializeField] private GameObject cliff1;
    [SerializeField] private GameObject cliff2;
    [SerializeField] private GameObject cliff3;
    [SerializeField] private GameObject cliff4;

    [SerializeField] private GameObject mushroom1;
    [SerializeField] private GameObject mushroom2;
    [SerializeField] private GameObject mushroom3;

    [SerializeField] private GameObject rock1;
    [SerializeField] private GameObject rock2;
    [SerializeField] private GameObject rock3;
    [SerializeField] private GameObject rock4;
    [SerializeField] private GameObject rock5;
    [SerializeField] private GameObject rock6;
    [SerializeField] private GameObject rock7;
    [SerializeField] private GameObject rock8;

    [SerializeField] private GameObject stalagmite1;
    [SerializeField] private GameObject stalagmite2;
    [SerializeField] private GameObject stalagmite3;
    [SerializeField] private GameObject stalagmite4;

    [SerializeField] private GameObject topRock1;
    [SerializeField] private GameObject topRock2;
    [SerializeField] private GameObject topRock3;
    [SerializeField] private GameObject topRock4;

    [SerializeField] private GameObject stalactite1;
    [SerializeField] private GameObject stalactite2;
    [SerializeField] private GameObject stalactite3;


    [SerializeField] private int floorIncrement = 0;
    private int recentVariant;

    private float cliffDelay = 2f;

    private void OnEnable()
    {
        InstantiatePrefabs();

        planeTransform = transform.parent;

        objectList = new List<GameObject>();
        obstacleList = new List<GameObject>();
    }

    private void Start()
    {
        caveParent = GameObject.Find("CaveObjects");
        pooler = ObjectPooler.instance;
        if (caveFloor != null)
        {
            for (int i = 0; i < 3; i++)
            {
                objectList.Add(pooler.NewObject(caveFloor, new Vector3(planeTransform.position.x, planeTransform.position.y - 8f, (i * caveFloor.transform.localScale.z) * 10), Quaternion.identity));
                objectList[i].transform.parent = caveParent.transform;
                floorIncrement++;
            }
        }

        StartCoroutine(SpawnCliffs(cliffDelay));
    }

    private void Update()
    {
        foreach (GameObject obj in objectList)
        {
            MoveFloorTile(obj);
        }

        foreach (GameObject obj in obstacleList)
        {
            if (obj.transform.position.z <= planeTransform.position.z - 15f)
            {
                ReturnCliffToPool(obj);
            }
        }
    }

    private void InstantiatePrefabs()
    {
        caveFloor = Resources.Load<GameObject>("CaveObj");

        cliff1 = Resources.Load<GameObject>("Area3.Cliff2");
        cliff2 = Resources.Load<GameObject>("Area3.Cliff3");
        cliff3 = Resources.Load<GameObject>("Area3.Cliff4");
        cliff4 = Resources.Load<GameObject>("Area3.Cliff8");

        mushroom1 = Resources.Load<GameObject>("Area3.Mushroom1");
        mushroom2 = Resources.Load<GameObject>("Area3.Mushroom2");
        mushroom3 = Resources.Load<GameObject>("Area3.Mushroom3");

        rock1 = Resources.Load<GameObject>("Area3.Rock1");
        rock2 = Resources.Load<GameObject>("Area3.Rock2");
        rock3 = Resources.Load<GameObject>("Area3.Rock3");
        rock4 = Resources.Load<GameObject>("Area3.Rock4");
        rock5 = Resources.Load<GameObject>("Area3.Rock5");
        rock6 = Resources.Load<GameObject>("Area3.Rock6");
        rock7 = Resources.Load<GameObject>("Area3.Rock7");
        rock8 = Resources.Load<GameObject>("Area3.Rock8");

        stalagmite1 = Resources.Load<GameObject>("Area3.Stalagmite1");
        stalagmite2 = Resources.Load<GameObject>("Area3.Stalagmite2");
        stalagmite3 = Resources.Load<GameObject>("Area3.Stalagmite3");
        stalagmite4 = Resources.Load<GameObject>("Area3.Stalagmite4");

        topRock1 = Resources.Load<GameObject>("Area3.Top.Rock1");
        topRock2 = Resources.Load<GameObject>("Area3.Top.Rock2");
        topRock3 = Resources.Load<GameObject>("Area3.Top.Rock3");
        topRock4 = Resources.Load<GameObject>("Area3.Top.Rock4");

        stalactite1 = Resources.Load<GameObject>("Area3.stalactite1");
        stalactite2 = Resources.Load<GameObject>("Area3.stalactite2");
        stalactite3 = Resources.Load<GameObject>("Area3.stalactite3");

    }

    private void MoveFloorTile(GameObject obj)
    {
        if (obj.transform.position.z <= planeTransform.position.z - (caveFloor.transform.localScale.z * 5))
        {
            obj.transform.position = new Vector3(planeTransform.position.x, planeTransform.position.y - 8f, (floorIncrement * caveFloor.transform.localScale.z) * 10);
            floorIncrement++;
        }
    }

    private void ReturnCliffToPool(GameObject cliff)
    {
        pooler.ReturnToPool(cliff);
    }

    private IEnumerator SpawnCliffs(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            SpawnStalagmitePrefab();
            SpawnStalactitePrefab();
        }
    }

    private void SpawnStalagmitePrefab()
    {
        GameObject cliffVariant;
        int cliffVariantInt = Random.Range(0, 18);

        while (cliffVariantInt == recentVariant)
        {
            cliffVariantInt = Random.Range(0, 18);
        }
        recentVariant = cliffVariantInt;

        switch (cliffVariantInt)
        {
            case 0:
                cliffVariant = cliff1;
                break;

            case 1:
                cliffVariant = cliff2;
                break;

            case 2:
                cliffVariant = cliff3;
                break;

            case 3:
                cliffVariant = cliff4;
                break;

            case 4:
                cliffVariant = mushroom1;
                break;

            case 5:
                cliffVariant = mushroom2;
                break;

            case 6:
                cliffVariant = mushroom3;
                break;

            case 7:
                cliffVariant = rock1;
                break;

            case 8:
                cliffVariant = rock2;
                break;

            case 9:
                cliffVariant = rock3;
                break;

            case 10:
                cliffVariant = rock4;
                break;

            case 11:
                cliffVariant = rock5;
                break;

            case 12:
                cliffVariant = rock6;
                break;

            case 13:
                cliffVariant = rock7;
                break;

            case 14:
                cliffVariant = rock8;
                break;

            case 15:
                cliffVariant = stalagmite1;
                break;

            case 16:
                cliffVariant = stalagmite2;
                break;

            case 17:
                cliffVariant = stalagmite3;
                break;

            case 18:
                cliffVariant = stalagmite4;
                break;

            default:
                cliffVariant = cliff1;
                break;
        }

        SpawnAtRandomGroundSpot(cliffVariant);
    }

    private void SpawnStalactitePrefab()
    {
        GameObject cliffVariant;
        int cliffVariantInt = Random.Range(0, 11);

        while (cliffVariantInt == recentVariant)
        {
            cliffVariantInt = Random.Range(0, 11);
        }
        recentVariant = cliffVariantInt;

        switch (cliffVariantInt)
        {
            case 0:
                cliffVariant = cliff1;
                break;

            case 1:
                cliffVariant = cliff2;
                break;

            case 2:
                cliffVariant = cliff3;
                break;

            case 3:
                cliffVariant = cliff4;
                break;

            case 4:
                cliffVariant = topRock1;
                break;

            case 5:
                cliffVariant = topRock2;
                break;

            case 6:
                cliffVariant = topRock3;
                break;

            case 7:
                cliffVariant = topRock4;
                break;

            case 8:
                cliffVariant = stalactite1;
                break;

            case 9:
                cliffVariant = stalactite2;
                break;

            case 10:
                cliffVariant = stalactite3;
                break;

            default:
                cliffVariant = cliff1;
                break;
        }

        SpawnAtRandomCeilingSpot(cliffVariant);
    }

    private void SpawnAtRandomGroundSpot(GameObject cliffObj)
    {
        // Ground Rock Spawning
        if (cliffObj == cliff1 || cliffObj == cliff2 || cliffObj == cliff3 || cliffObj == cliff4 || cliffObj == rock1 || cliffObj == rock2 || cliffObj == rock3)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), -8f, planeTransform.position.z + Random.Range(100f, 120f));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
        // Ground Mushroom Spawning
        else if (cliffObj == mushroom1 || cliffObj == mushroom2 || cliffObj == mushroom3 || cliffObj == rock4 || cliffObj == rock5 || cliffObj == rock6 || cliffObj == rock7 || cliffObj == rock8)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), -8, planeTransform.position.z + Random.Range(111f, 120f));
            Quaternion spawnRot = Quaternion.Euler(0, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }

        // Stalagmites Spawning
        else if (cliffObj == stalagmite1 || cliffObj == stalagmite2)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), -8, planeTransform.position.z + Random.Range(111f, 120f));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
        else if (cliffObj == stalagmite3 || cliffObj == stalagmite4)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), -8, planeTransform.position.z + Random.Range(111f, 120f));
            Quaternion spawnRot = Quaternion.Euler(90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }

        else
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), 0.94f, planeTransform.position.z + Random.Range(111f, 120f));
            Quaternion spawnRot = Quaternion.Euler(0, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
        
    }

    private void SpawnAtRandomCeilingSpot(GameObject cliffObj)
    {
        // stalactites Spawning
        if (cliffObj == stalactite1 || cliffObj == stalactite2 || cliffObj == stalactite3)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), 8f, planeTransform.position.z + Random.Range(100f, 110f));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
        // roof rock Spawning
        else if (cliffObj == topRock1 || cliffObj == topRock2 || cliffObj == topRock3 || cliffObj == topRock4)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), 8f, planeTransform.position.z + Random.Range(100f, 110f));
            Quaternion spawnRot = Quaternion.Euler(0, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
        /*
        else
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), 8f, planeTransform.position.z + Random.Range(100f, 124f));
            Quaternion spawnRot = Quaternion.Euler(90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
            
        }
        */
    }

    public void ClearObjects()
    {
        objectList.Clear();
        obstacleList.Clear();
        StopAllCoroutines();

        foreach (Transform child in caveParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
