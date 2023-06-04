using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class MusicObstacle : DestroyableObject
{
    [HideInInspector] public MusicLane currentLane;
    private float _speed;

    private Camera _mainCamera;
    
    private void Start()
    {
        _speed = currentLane.speed;
        _mainCamera = Camera.main;
    }

    public void Update()
    {
        if (_speed == 0)
        {
            _speed = currentLane.speed;    
        }
        
        transform.Translate(-_speed * Time.deltaTime,0,0);
        
        float borderX = (_mainCamera.transform.position.x - OrthographicBounds(_mainCamera).size.x / 2) - 1;
        if (transform.position.x <= borderX)
        {
            currentLane.lanePool.pool.Release(transform.gameObject);
        }
    }
    
    public static Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
    
}
