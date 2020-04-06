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
        int cliffVariantInt = Random.Range(0, 4);

        while (cliffVariantInt == recentVariant)
        {
            cliffVariantInt = Random.Range(0, 4);
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

            default:
                cliffVariant = cliff1;
                break;
        }

        SpawnAtRandomGroundSpot(cliffVariant);
    }

    private void SpawnStalactitePrefab()
    {
        GameObject cliffVariant;
        int cliffVariantInt = Random.Range(0, 4);

        while (cliffVariantInt == recentVariant)
        {
            cliffVariantInt = Random.Range(0, 4);
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

            default:
                cliffVariant = cliff1;
                break;
        }

        SpawnAtRandomCeilingSpot(cliffVariant);
    }

    private void SpawnAtRandomGroundSpot(GameObject cliffObj)
    {
        if (cliffObj != cliff3)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), -8f, planeTransform.position.z + Random.Range(900f, 1000f));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
        else
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), 0.94f, planeTransform.position.z + Random.Range(900f, 1000f));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
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
        if (cliffObj != cliff3)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), 8f, planeTransform.position.z + Random.Range(900f, 1000f));
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
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), 8f, planeTransform.position.z + Random.Range(867f, 1024f));
            Quaternion spawnRot = Quaternion.Euler(90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = caveParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
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
