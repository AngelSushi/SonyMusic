using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class MusicObstacle : DestroyableObject
{
    [HideInInspector] public MusicLane currentLane;
    private float _speed;
    
    

    private GameManager _gameManager;
    private PlayerDash _playerDash;
    
    private void Start()
    {
        _speed = currentLane.speed;
        _gameManager = FindObjectOfType<GameManager>();
        _playerDash = FindObjectOfType<PlayerDash>();
    }

    public void Update()
    {
        if (_speed == 0)
        {
            _speed = currentLane.speed;    
        }
        
        transform.Translate(-_speed * Time.deltaTime,0,0);
        
        if (_gameManager.GetSideValueBetweenTwoPoints(transform.position,_playerDash.Limit.transform.position,_playerDash.Limit.transform.forward) > 0)
        {
            currentLane.lanePool.pool.Release(transform.gameObject);
            _gameManager.Event.OnReleaseObstacle?.Invoke(this,new EventManager.OnReleaseObstacleArgs { obstacle = transform.gameObject, player = _playerDash, isCut = IsCut});
        }
    }

}
