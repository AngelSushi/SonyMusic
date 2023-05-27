using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UnitySpriteCutter;
using Slider = UnityEngine.UI.Slider;

public class PlayerDash : MonoBehaviour {

    [SerializeField] private float dashSpeed;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;


    [SerializeField] private float dashDistance;
    
    private Vector3 _playerPosition;
    public bool _isDashing;

    private Vector3 _startObstaclePosition;

    [SerializeField] private bool debugDash;

    private float _dashPoint;
    [SerializeField] private float pointPerDash;
    [SerializeField] private float maxDashPoint;
    [SerializeField] private Slider dashSlider;

    [SerializeField] private Vector2[] directionsModel;
    [Range(0,1)]
    [SerializeField] private float directionOffset;


    private Vector3 _currentDashDirection;
    private Vector3 _lastDashDirection;
    
    void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _startPosition = ConvertPoint(touch.position);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _endPosition = ConvertPoint(touch.position);

                if (_currentDashDirection != Vector3.zero)
                    _lastDashDirection = _currentDashDirection;

                _currentDashDirection = (_endPosition - _startPosition).normalized;
                _rb.velocity = _currentDashDirection * dashSpeed;

                Debug.Log("currentDirection " + _currentDashDirection + " lastDirection " + _lastDashDirection);
                _playerPosition = transform.position;
                _isDashing = true;

            }
        }


        if (Vector3.Distance(_playerPosition, transform.position) > dashDistance && _isDashing)
        {
            _rb.velocity = Vector2.zero;
            _isDashing = false;
        }
    }
    
    
    
    // a chaque nouveau dash on récupérer sa direction 
    
    
    

    private Vector3 ConvertPoint(Vector3 point) 
    {
       Vector3 screenPosition = new Vector3(point.x, point.y,-Camera.main.transform.position.z);
       Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
       return worldPosition;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Destructible") && _isDashing)
        {
            if (debugDash)
            {
                col.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                col.gameObject.transform.GetChild(0).position = transform.position;
            }
            
            _startObstaclePosition = transform.position;

        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Destructible") && _isDashing)
        {
            if (debugDash)
            {
                col.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                col.gameObject.transform.GetChild(1).position = transform.position;
            }


            foreach (Vector2 direction in directionsModel)
            {
                if (direction.x - directionOffset <= _currentDashDirection.x && direction.x + directionOffset >= _currentDashDirection.x)
                {
                    
                }
            }


            SpriteCutterOutput output = SpriteCutter.Cut( new SpriteCutterInput() 
            {
                lineStart = _startObstaclePosition,
                lineEnd = transform.position,
                gameObject = col.gameObject,
                gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
            } );

            if ( output != null && output.secondSideGameObject != null ) 
            {
                Rigidbody2D newRigidbody = output.secondSideGameObject.AddComponent<Rigidbody2D>();

                if (output.firstSideGameObject.GetComponent<Rigidbody2D>() == null)
                {
                    output.firstSideGameObject.AddComponent<Rigidbody2D>();
                }
                
                newRigidbody.velocity = output.firstSideGameObject.GetComponent<Rigidbody2D>().velocity;
                AddPoint();
            }
        }
    }

    private void AddPoint() {
        _dashPoint += pointPerDash;
        _dashPoint = Mathf.Clamp(_dashPoint, 0, maxDashPoint);
        dashSlider.value = _dashPoint / maxDashPoint;
    }
}
