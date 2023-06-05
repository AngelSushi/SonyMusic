using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObstaclePool : MonoBehaviour
{

    public GameObject emptyPrefab;

    private ObjectPool<GameObject> _pool;
    public ObjectPool<GameObject> pool
    {
        get
        {
            _pool = new ObjectPool<GameObject>(CreateObstacle, OnTakeFromPool, OnReturnToPool, OnObjectDestroy, true, 5,
                5);

            return _pool;
        }
        
        
    }

    private GameObject CreateObstacle()
    {
        GameObject instancePrefab = Instantiate(emptyPrefab, transform);
        MusicObstacle obstacle = instancePrefab.AddComponent<MusicObstacle>();
        obstacle.currentLane = GetComponent<MusicLane>();


        return instancePrefab;
    }

    private void OnTakeFromPool(GameObject go)
    {
        Debug.Log("set to true");
        go.SetActive(true);
    }

    private void OnReturnToPool(GameObject go)
    {
        Debug.Log("set to false");
        go.SetActive(false);
    }

    private void OnObjectDestroy(GameObject go) 
    {
        Debug.Log("on destroy");
        Destroy(go);
    }

}
