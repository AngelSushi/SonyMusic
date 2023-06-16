using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class Plateform : MonoBehaviour
{

    private BoxCollider2D _collider2D;
    private Vector2 _enterDashDirection;
    private GameManager _gameManager;

    public bool disableCollision;
    public bool _isPassingThrough;
    public Collider2D _playerCollider,_actualCollider;

    private PlatformEffector2D _effector2D;

    private void Start()
    {
        _collider2D = GetComponent<BoxCollider2D>();
        _gameManager = GameManager.instance;
        _effector2D = GetComponent<PlatformEffector2D>();
        _gameManager.Event.OnDashLaunched += OnDashLaunched;
        _playerCollider = FindObjectOfType<PlayerDash>().GetComponent<Collider2D>();
    }

    private void OnDestroy()
    {
        if (_gameManager != null && _gameManager.Event != null)
        {
            _gameManager.Event.OnDashLaunched -= OnDashLaunched;    
        }
        
    }


    private void Update()
    {
        if (_playerCollider != null)
        {
            Physics2D.IgnoreCollision(_collider2D,_playerCollider,disableCollision);
        }
    }
    
    

    private void OnDashLaunched(object sender, EventManager.OnDashLaunchedArgs e)
    {
        if (_effector2D == null)
        {
            return;
        }
        
        if (e.dashDirection.y < 0.2f && e.switchingPlateform && _actualCollider != null)
        {
            disableCollision = true;
            _isPassingThrough = true;
        }
        
       // Debug.Break();
    }
    
    
    private void OnCollisionEnter2D(Collision2D col)
    {

        if (_gameManager.IsTransitioning)
        {
            return;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
           
            if (col.gameObject.GetComponent<PlayerDash>().lastHitPlateform  != null && col.gameObject.GetComponent<PlayerDash>().lastHitPlateform != transform.gameObject)
            {
                col.gameObject.GetComponent<PlayerDash>().lastHitPlateform.GetComponent<Plateform>()._isPassingThrough = false;
                col.gameObject.GetComponent<PlayerDash>().lastHitPlateform.GetComponent<Plateform>().disableCollision = false;
                //StartCoroutine(col.gameObject.GetComponent<PlayerDash>().StartAndDestroyAnim());
            }
            
            _actualCollider = col.collider;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (_gameManager.IsTransitioning)
        {
            return;
        }
        
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            col.gameObject.GetComponent<PlayerDash>().lastHitPlateform = transform.gameObject;
            _actualCollider = null;
        }
    }
}
