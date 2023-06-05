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
    [SerializeField] private int descentGravity;

    [Header("Score")]
    [SerializeField] private float pointPerDash;
    [SerializeField] private float maxDashPoint;
    [SerializeField] private Slider dashSlider;
    
    [Header("Debug")]
    [SerializeField] private bool debugDash;
    [SerializeField] private bool smoothDash;


    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;
    private Vector3 _playerPosition;
    private Vector3 _startObstaclePosition;
    [HideInInspector] public Vector3 dashDirection;
    private float _dashPoint;
    private Vector2 _lastVelocity;
    private bool _beginFromPlayer;

    private Vector2 _startVelocity;
    private float startTime;
    void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerPosition = transform.localPosition;
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
                                _startVelocity = _rb.velocity;
                                dashDirection = (_endPosition - _startPosition).normalized;
                                isDashing = true;
                                _playerPosition = transform.localPosition;
                                startTime = Time.time;
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
                        _startVelocity = _rb.velocity;
                        dashDirection = (_endPosition - _startPosition).normalized;
                        _playerPosition = transform.localPosition;
                        isDashing = true;
                        startTime = Time.time;

                        if (dashDirection.y < 0f)
                        {
                            
                        }
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        
        // Adapter la distance si on va vers le bas ca va vers l'objet le plus proche vers le  bas 

        float checkDistance = Vector3.Distance(_playerPosition, transform.localPosition);

        if (checkDistance > dashDistance && isDashing)
        {
            _rb.velocity = Vector2.zero;
            dashDirection = Vector2.zero;
            isDashing = false;
        }
        else if (isDashing)
        {
            float t = (Time.time - startTime) / (dashDistance / dashSpeed);
            _rb.velocity = Vector2.Lerp(_startVelocity, dashDirection * dashSpeed, t);
        }
        
        if (_rb.velocity.y < 0 && smoothDash)
        {
            _rb.velocity += Vector2.down * descentGravity * Time.deltaTime;
        }
        
        
        //Debug.Log("velocity " + _rb.velocity);

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

    private void AddPoint() {
        _dashPoint += pointPerDash;
        _dashPoint = Mathf.Clamp(_dashPoint, 0, maxDashPoint);
        dashSlider.value = _dashPoint / maxDashPoint;
    }
}
