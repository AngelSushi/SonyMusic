using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UnitySpriteCutter;
using Slider = UnityEngine.UI.Slider;

public class PlayerDash : MonoBehaviour {

    [Header("Dash")]
    public bool isDashing;
    [SerializeField] private bool beginFromPlayer;
    [SerializeField] private int playerDragAngle;
    
    [Header("Dash Values")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private int angleOffset;
    [SerializeField] private int diagonalAngleOffset;

    [Header("Score")]
    [SerializeField] private float pointPerDash;
    [SerializeField] private float maxDashPoint;
    [SerializeField] private Slider dashSlider;
    [SerializeField] private Transform limit;
    [SerializeField] private Transform distanceCombo;
    private float comboPoint = 0;

    
    [Header("Debug")]
    [SerializeField] private bool debugDash;
    
    
    
    
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;
    private Vector3 _playerPosition;
    private Vector3 _startObstaclePosition;
    [HideInInspector] public Vector3 dashDirection;
    private float _dashPoint;

    private bool _beginFromPlayer;
    
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
                if (beginFromPlayer)
                {
                    foreach (Collider2D col2D in Physics2D.OverlapCircleAll(ConvertPoint(touch.position), playerDragAngle))
                    {
                        if (col2D.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            _endPosition = ConvertPoint(touch.position);

                            float distance = Vector3.Distance(_startPosition, _endPosition);
                            
                            if (distance >= minDistance)
                            {
                                dashDirection = (_endPosition - _startPosition).normalized;
                                _rb.velocity = dashDirection * dashSpeed;

                                _playerPosition = transform.position;
                                isDashing = true;
                            }
                            ;
                        }
                    }
                }
                else
                {
                    _endPosition = ConvertPoint(touch.position);
                    float distance = Vector3.Distance(_startPosition, _endPosition);
                            
                    if (distance >= minDistance)
                    {
                        dashDirection = (_endPosition - _startPosition).normalized;
                        _rb.velocity = dashDirection * dashSpeed;

                        _playerPosition = transform.position;
                        isDashing = true;
                    }
                }
            }
        }


        if (Vector3.Distance(_playerPosition, transform.position) > dashDistance && isDashing)
        {
            _rb.velocity = Vector2.zero;
            dashDirection = Vector2.zero;
            isDashing = false;
        }
    }

    private Vector3 ConvertPoint(Vector3 point) 
    {
       Vector3 screenPosition = new Vector3(point.x, point.y,-Camera.main.transform.position.z);
       Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
       return worldPosition;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Destructible") && isDashing)
        {
            if (debugDash)
            {
                col.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                col.gameObject.transform.GetChild(0).position = transform.position;
            }
            
            _startObstaclePosition = transform.position;

        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Destructible") && isDashing && _startObstaclePosition == Vector3.zero)
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
        if (col.gameObject.layer == LayerMask.NameToLayer("Destructible") && isDashing)
        {
            if (debugDash)
            {
                col.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                col.gameObject.transform.GetChild(1).position = transform.position;
            }


            DestroyableObject dObj = col.GetComponent<DestroyableObject>();

            Vector3 dir = Vector3.zero;
            float targetAngle = angleOffset;
            
            switch (dObj.dashDirection)
            {
                case DashDirection.UP:
                    dir = col.transform.up;
                    break;
                
                case DashDirection.DOWN:
                    dir = col.transform.up * -1;
                    break;
                
                case DashDirection.LEFT:
                    dir = col.transform.right;
                    break;
                
                case DashDirection.RIGHT:
                    dir = col.transform.right * -1;
                    break;
                
                case DashDirection.DIAGONAL_LUP:
                    dir = col.transform.right + col.transform.up * -1;
                    targetAngle = diagonalAngleOffset;
                    break;
                
                case DashDirection.DIAGONAL_RUP:
                    dir = col.transform.right * -1 + col.transform.up * -1;
                    targetAngle = diagonalAngleOffset;
                    break;
                
                case DashDirection.DIAGONAL_LDOWN:
                    dir = col.transform.right + col.transform.up;
                    targetAngle = diagonalAngleOffset;
                    break;
                
                case DashDirection.DIAGONAL_RDOWN:
                    dir = col.transform.right * -1 + col.transform.up;
                    targetAngle = diagonalAngleOffset;
                    break;
            }
            
            
            float angle = Vector3.Angle(dashDirection,dir);
            Debug.Log("angle " + (int)angle + " " + targetAngle);


            if ((int)angle <= targetAngle || dObj.dashDirection == DashDirection.ALL)
            {
                SpriteCutterOutput output = SpriteCutter.Cut( new SpriteCutterInput() 
                {
                    lineStart = _startObstaclePosition,
                    lineEnd = transform.position,
                    gameObject = col.gameObject,
                    gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
                } );
  
                Debug.Log("output " + output + " second " + output.secondSideGameObject);
                
                if ( output != null && output.secondSideGameObject != null ) 
                { 
                    Rigidbody2D newRigidbody = output.firstSideGameObject.AddComponent<Rigidbody2D>();

                    output.secondSideGameObject.GetComponent<MeshRenderer>().material.color = Color.black;
                    
                    if (output.secondSideGameObject.GetComponent<Rigidbody2D>() == null)
                    {
                        output.secondSideGameObject.AddComponent<Rigidbody2D>();
                       // output.secondSideGameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
                    }
                    
                    newRigidbody.velocity = output.secondSideGameObject.GetComponent<Rigidbody2D>().velocity;

                   _startObstaclePosition = Vector3.zero;
                    AddPoint();
                }
            }
        }
    }

    private void AddPoint() 
    {
        if (limit.position.x < gameObject.transform.position.x && gameObject.transform.position.x < distanceCombo.position.x)
        {
            comboPoint = comboPoint + 1;
            _dashPoint += pointPerDash * comboPoint;
            _dashPoint = Mathf.Clamp(_dashPoint, 0, maxDashPoint);
            dashSlider.value = _dashPoint / maxDashPoint;
        }
        else
        {
            comboPoint = 1;
            _dashPoint += pointPerDash * comboPoint;
            _dashPoint = Mathf.Clamp(_dashPoint, 0, maxDashPoint);
            dashSlider.value = _dashPoint / maxDashPoint;
        }
        Debug.Log("Les points totaux" + _dashPoint);
        Debug.Log("le combot point est de " + comboPoint);

    }
}
