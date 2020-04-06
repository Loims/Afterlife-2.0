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

    private List<GameObject> objectList;
    [SerializeField] private List<GameObject> obstacleList;

    private GameObject skyParent;

    [SerializeField] private GameObject mountain1;
    [SerializeField] private GameObject mountain2;
    [SerializeField] private GameObject hotAirBalloon1;
    [SerializeField] private GameObject hotAirBalloon2;
    [SerializeField] private GameObject hotAirBalloon3;
    [SerializeField] private GameObject pillar;

    [SerializeField] private int floorIncrement = 0;
    private int recentVariant;

    private float objDelay = 1.5f;
    
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

        StartCoroutine(SpawnObjs(objDelay));
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
    }

    /// <summary>
    /// Helper methods for instantiating prefabs
    /// </summary>
    private void InstantiatePrefabs()
    {
        mountain1 = Resources.Load<GameObject>("Area2.Mountain1");
        mountain2 = Resources.Load<GameObject>("Area2.Mountain2");
        hotAirBalloon1 = Resources.Load<GameObject>("Area2.HotAirBalloon1");
        hotAirBalloon2 = Resources.Load<GameObject>("Area2.HotAirBalloon2");
        hotAirBalloon3 = Resources.Load<GameObject>("Area2.HotAirBalloon3");
        pillar = Resources.Load<GameObject>("Area2.Pillar1");

    }

    /// <summary>
    /// Helper to return objects to pool
    /// </summary>
    private void ReturnObjToPool(GameObject obj)
    {
        pooler.ReturnToPool(obj);
    }

    /// <summary>
    /// Coroutine for spawning objects
    /// </summary>
    private IEnumerator SpawnObjs(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            SpawnObj();
        }
    }

    /// <summary>
    /// Method for spawning different object variants. Decides which object to spawn then calls helper method
    /// </summary>
    private void SpawnObj()
    {
        GameObject objVariant;
        int objVariantInt = Random.Range(0, 6);

        while (objVariantInt == recentVariant)
        {
            objVariantInt = Random.Range(0, 6);
        }
        recentVariant = objVariantInt;

        switch (objVariantInt)
        {
            case 0:
                objVariant = mountain1;
                break;

            case 1:
                objVariant = mountain2;
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
                objVariant = pillar;
                break;

            default:
                objVariant = mountain1;
                break;
        }

        SpawnAtRandomSpot(objVariant);
    }

    /// <summary>
    /// Helper method to generate spawn points. Spawn points are generated based on the passed in object
    /// </summary>
    private void SpawnAtRandomSpot(GameObject obj)
    {
        if (obj == mountain1 || obj == mountain2)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-17f, 17f), -25.8f, planeTransform.position.z + (1000f + (20 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
            GameObject newObj = pooler.NewObject(obj, spawnPos, spawnRot);
            newObj.transform.parent = skyParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }

        else if (obj == hotAirBalloon1 || obj == hotAirBalloon2 || obj == hotAirBalloon3)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-17f, 17f), Random.Range(-10f, 3.5f), planeTransform.position.z + (1000f + (20 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(0, 0, 0);
            GameObject newObj = pooler.NewObject(obj, spawnPos, spawnRot);
            newObj.transform.parent = skyParent.transform;
            if (!obstacleList.Contains(newObj))
            {
                obstacleList.Add(newObj);
            }
        }

        else if(obj == pillar)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-17f, 17f), -18.28f, planeTransform.position.z + (1000f + (20 * planeComp.speedMultiplier)));
            Quaternion spawnRot = Quaternion.Euler(-90, 0, 0);
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
