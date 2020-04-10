using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanPlacement : MonoBehaviour
{
    [Header("Plane Transform")]
    [SerializeField] private Transform planeTransform;

    [Space]
    [Header("Plane Script")]
    [SerializeField] private PlaneMovement planeComp;

    [Space]
    [Header("Object Pooler")]
    [SerializeField] private ObjectPooler pooler;

    private List<GameObject> objectList;
    [SerializeField] private List<GameObject> obstacleList;

    private GameObject oceanParent;

    [SerializeField] private GameObject oceanFloor;
    [SerializeField] private GameObject cliff1;
    [SerializeField] private GameObject cliff2;
    [SerializeField] private GameObject cliff3;
    [SerializeField] private GameObject cliff4;
    [SerializeField] private GameObject rock1;
    [SerializeField] private GameObject rock2;
    [SerializeField] private GameObject rock3;
    [SerializeField] private GameObject rock4;


    [SerializeField] private int floorIncrement = 0;
    private int recentVariant;

    private float cliffDelay = 2f;

    /// <summary>
    /// Instantiate prefabs and variables
    /// </summary>
    private void OnEnable()
    {

        InstantiatePrefabs();

        planeTransform = transform.parent;
        planeComp = transform.parent.GetComponent<PlaneMovement>();

        objectList = new List<GameObject>();
        obstacleList = new List<GameObject>();
    }

    /// <summary>
    /// Sets the ocean parents object and gets the object pooler instance. Create initial ocean floor objects
    /// </summary>
    private void Start()
    {

        oceanParent = GameObject.Find("OceanObjects");
        pooler = ObjectPooler.instance;
        if (oceanFloor != null)
        {
            for (int i = 0; i < 3; i++)
            {
                objectList.Add(pooler.NewObject(oceanFloor, new Vector3(planeTransform.position.x, planeTransform.position.y - 8f, (i * oceanFloor.transform.localScale.z) * 10), Quaternion.identity));
                objectList[i].transform.parent = oceanParent.transform;
                floorIncrement++;
            }
        }

        StartCoroutine(SpawnCliffs(cliffDelay));
    }

    /// <summary>
    /// Move any floor tiles that go offscreen. Returns cliffs to the object pooler if offscreen
    /// </summary>
    private void Update()
    {
        foreach(GameObject obj in objectList)
        {
            MoveFloorTile(obj);
        }

        foreach(GameObject obj in obstacleList)
        {
            if(obj.transform.position.z <= planeTransform.position.z - 15f)
            {
                ReturnCliffToPool(obj);
            }
        }
    }

    /// <summary>
    /// Helper method for instatiating prefabs
    /// </summary>
    private void InstantiatePrefabs()
    {
        oceanFloor = Resources.Load<GameObject>("OceanFloor");

        cliff1 = Resources.Load<GameObject>("Area1.Cliff2");
        cliff2 = Resources.Load<GameObject>("Area1.Cliff3");
        cliff3 = Resources.Load<GameObject>("Area1.Cliff4");
        cliff4 = Resources.Load<GameObject>("Area1.Cliff8");
        cliff1 = Resources.Load<GameObject>("Area1.Rock1");
        cliff2 = Resources.Load<GameObject>("Area1.Rock2");
        cliff3 = Resources.Load<GameObject>("Area1.Rock3");
        cliff4 = Resources.Load<GameObject>("Area1.Rock4");
    }

    /// <summary>
    /// Helper method for moving floor tiles
    /// </summary>
    private void MoveFloorTile(GameObject obj)
    {
        if (obj.transform.position.z <= planeTransform.position.z - (oceanFloor.transform.localScale.z * 5))
        {
            obj.transform.position = new Vector3(planeTransform.position.x, planeTransform.position.y - 8f, (floorIncrement * oceanFloor.transform.localScale.z) * 10);
            floorIncrement++;
        }
    }

    /// <summary>
    /// Returns cliff object to pool
    /// </summary>
    /// <param name="cliff"></param>
    private void ReturnCliffToPool(GameObject cliff)
    {
        pooler.ReturnToPool(cliff);
    }


    /// <summary>
    /// Coroutine for spawning cliffs
    /// </summary>
    private IEnumerator SpawnCliffs(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            SpawnCliffPrefab();
        }
    }

    /// <summary>
    /// Helper method for spawning a cliff prefab. Decides what variant to spawn and then calls a helper method to generate a position
    /// </summary>
    private void SpawnCliffPrefab()
    {
        GameObject cliffVariant;
        int cliffVariantInt = Random.Range(0, 4);

        while(cliffVariantInt == recentVariant)
        {
            cliffVariantInt = Random.Range(0, 4);
        }
        recentVariant = cliffVariantInt;

        switch(cliffVariantInt)
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
                cliffVariant = rock1;
                break;

            case 5:
                cliffVariant = rock2;
                break;

            case 6:
                cliffVariant = rock3;
                break;

            case 7:
                cliffVariant = rock4;
                break;

            default:
                cliffVariant = cliff1;
                break;
        }

        SpawnAtRandomSpot(cliffVariant);
    }

    /// <summary>
    /// Decides spawn point for passed cliff objects and spawns it at the position, adding it to the list
    /// </summary>
    private void SpawnAtRandomSpot(GameObject cliffObj)
    {
        if (cliffObj != cliff3)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), -8f, planeTransform.position.z + (124f + (10 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = oceanParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
        else
        {
            Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), 0.94f, planeTransform.position.z + (124f + (10 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(cliffObj, spawnPos, spawnRot);
            newObj.transform.parent = oceanParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }
    }

    /// <summary>
    /// Clears objects from list
    /// </summary>
    public void ClearObjects()
    {
        objectList.Clear();
        obstacleList.Clear();
        StopAllCoroutines();

        foreach(Transform child in oceanParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
