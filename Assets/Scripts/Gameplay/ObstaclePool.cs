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
            _pool ??= new ObjectPool<GameObject>(CreateObstacle, OnTakeFromPool, OnReturnToPool, OnObjectDestroy, true, 4,
                10);

            return _pool;
        }
        
        
    }

    private GameObject CreateObstacle()
    {
        GameObject instancePrefab = Instantiate(emptyPrefab, transform);
        MusicObstacle obstacle = instancePrefab.AddComponent<MusicObstacle>();
        obstacle.currentLane = GetComponent<MusicLane>();
        instancePrefab.transform.parent = transform;
        
        return instancePrefab;
    }

    private void OnTakeFromPool(GameObject go) => go.SetActive(true);
    private void OnReturnToPool(GameObject go) =>go.SetActive(false);
    private void OnObjectDestroy(GameObject go) => Destroy(go);
    

}
