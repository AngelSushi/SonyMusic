using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    private float _initialDashDistance;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;
    private Vector3 _playerPosition;
    private Vector3 _startObstaclePosition;
    [HideInInspector] public Vector3 dashDirection;
    private float _dashPoint;
    private Vector2 _lastVelocity;
    private bool _beginFromPlayer;

    private List<Plateform> _plateforms;
    
    
    void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerPosition = transform.localPosition;
    }

    private void Start()
    {
        _plateforms = FindObjectsOfType<Plateform>().ToList();
        _initialDashDistance = dashDistance;
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
                                dashDistance = _initialDashDistance;
                                dashDirection = (_endPosition - _startPosition).normalized;
                                _playerPosition = transform.localPosition;
                                isDashing = true;

                                if (dashDirection.y < 0f)
                                {
                                    float minDistance = 0;
                                    foreach (Plateform plateform in _plateforms)
                                    {
                                        float calculateDistance = Vector3.Distance(plateform.transform.position, transform.position);

                                        if (calculateDistance == 0 || calculateDistance < minDistance)
                                        {
                                            minDistance = calculateDistance;
                                        }
                                    }

                                    dashDistance = minDistance;
                                }
                            }
                            ;
                        }
                    }
                }
                else
                {
                    _endPosition = ConvertPoint(touch.position);
                    float distance = Vector2.Distance(_startPosition, _endPosition);
                            
                    if (distance >= minDistance)
                    {
                        dashDistance = _initialDashDistance;
                        dashDirection = (_endPosition - _startPosition).normalized;
                        _playerPosition = transform.localPosition;
                        isDashing = true;

                        if (dashDirection.y < 0f)
                        {
                            float minDistance = 0;
                            Plateform minPlateform = null;
                            
                            foreach (Plateform plateform in _plateforms)
                            {
                                
                                // Calculer la distance dans une direction 
                                float calculateDistance = Vector2.Distance(transform.position, plateform.transform.position);
                                if (minDistance == 0 || calculateDistance < minDistance)
                                {
                                    minDistance = calculateDistance;
                                    minPlateform = plateform;
                                }
                            }

                            float angle = Vector2.Angle(transform.up * -1, dashDirection);

                            float distanceY = minPlateform.transform.position.y - transform.position.y + minPlateform.transform.localScale.y / 2 ;

                            float ac = distanceY / Mathf.Cos(angle);
                            
                            Debug.Log("distanceY " + distanceY);
                            Debug.Log("distanceY " + angle);
                            Debug.Log("ac " + ac);
                            
                            Debug.Break();
                            dashDistance = minDistance;
                        }
                    }
                }
            }
        }
        
        Debug.DrawLine(transform.localPosition,transform.localPosition + transform.up * -1 * 20,Color.yellow);
    }

    private void LateUpdate()
    {
        float checkDistance = Vector2.Distance(_playerPosition, transform.localPosition);

        if (checkDistance > dashDistance && isDashing)
        {
            ResetDash();
        }
        else if (isDashing)
        {
            _rb.velocity = dashDirection * dashSpeed;
        }

        if (_rb.velocity.y < 0 && smoothDash)
        {
            _rb.velocity += Vector2.down * descentGravity * Time.deltaTime;
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Plateform>() != null)
        {
            if ((int)dashDistance != (int)_initialDashDistance)
            {
                ResetDash();
            }

            isDashing = false;
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


    private void ResetDash()
    {
        isDashing = false;
        _rb.velocity = Vector2.zero;
        dashDirection = Vector3.zero;
    }
    
    private void AddPoint() {
        _dashPoint += pointPerDash;
        _dashPoint = Mathf.Clamp(_dashPoint, 0, maxDashPoint);
        dashSlider.value = _dashPoint / maxDashPoint;
    }
}
