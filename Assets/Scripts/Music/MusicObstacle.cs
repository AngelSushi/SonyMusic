using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class MusicObstacle : DestroyableObject
{
    [HideInInspector] public MusicLane currentLane;
    private float _speed;

    
    private void Start()
    {
        _speed = currentLane.speed;
        
    }

    public void Update()
    {
        if (_speed == 0)
        {
            _speed = currentLane.speed;    
        }
        
        transform.Translate(-_speed * Time.deltaTime,0,0);
    }
    
}
