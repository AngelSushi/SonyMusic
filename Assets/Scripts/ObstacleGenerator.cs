using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Obstacle
{
    public GameObject obstacle;
    public Vector2 start;
    public Vector2 end;

    public Obstacle(GameObject obstacle, Vector2 start, Vector2 end)
    {
        this.obstacle = obstacle;
        this.start = start;
        this.end = end;
    }
}


public class ObstacleGenerator : MonoBehaviour 
{

    [SerializeField] private float timeBetweenSpawn;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float spawnY;
    [SerializeField] private float forceAmplifier;

    private Camera _camera;

    public List<Obstacle> _obstacles = new List<Obstacle>();

    

    private void Start()
    {
        _camera = Camera.main;
        StartCoroutine(SpawnObstacle());
    }


    private void Update()
    {
        foreach (Obstacle obstacle in _obstacles)
        {
            float obstacleY = obstacle.obstacle.transform.position.y;
            
            if (obstacleY < spawnY && obstacleY < 0)
            {
                _obstacles.Remove(obstacle);
               // Destroy(obstacle);
            }
                
        }
    }
    


    private IEnumerator SpawnObstacle() 
    {

        yield return new WaitForSeconds(timeBetweenSpawn);

        GameObject obstacle = Instantiate(obstaclePrefab);
        float randomX = Random.Range(-_camera.orthographicSize * 2, _camera.orthographicSize * 2);
        obstacle.transform.position = new Vector3(randomX,spawnY,0);
        obstacle.GetComponent<Rigidbody2D>().AddForce(Vector3.up * forceAmplifier,ForceMode2D.Impulse);

        float radius = obstacle.transform.localScale.x / 2;
        float angle = Random.Range(0, 180);

        float x = Mathf.Sin(angle) * radius;
        float y = Mathf.Cos(angle) * radius;

        obstacle.transform.GetChild(0).localPosition = new Vector3(x, y, 0);
        obstacle.transform.GetChild(1).localPosition = new Vector3(-x, -y, 0);
        
        Debug.Log("spawn obstacle " + obstacle);
        
        
        _obstacles.Add(new Obstacle(obstacle,new Vector2(x,y),new Vector2(-x,-y)));
        
       // StartCoroutine(SpawnObstacle());
    }
}
