using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UnitySpriteCutter;
using Slider = UnityEngine.UI.Slider;

public class PlayerDash : MonoBehaviour
{


    [Header("Movement")] [SerializeField] private float speed;
    
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
    [SerializeField] private float descentGravity;

    [Header("Score")]
    [SerializeField] private float pointPerDash;
    [SerializeField] private float maxDashPoint;
    [SerializeField] private Slider dashSlider;
    [SerializeField] private Transform limit;
    [SerializeField] private Transform distanceCombo;
    private float comboPoint = 0;

    [Header("player")]
    public GameObject groundDetection;
    public GameObject landingAnimation;
    public Animator playerAnimator;
    
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

    private List<Plateform> _plateforms;
    private GameManager _gameManager;
    
    void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerPosition = transform.localPosition;
        _gameManager = FindObjectOfType<GameManager>();
    }
    
    void Update() 
    {

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Dash", isDashing);    
        }
        

        if (Input.touchCount > 0) 
        {
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
                                //ici
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
                        //ici

                        _playerPosition = transform.position;
                        isDashing = true;
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (Vector3.Distance(_playerPosition, transform.position) > dashDistance && isDashing)
        {
            _rb.velocity = Vector2.zero;
            dashDirection = Vector2.zero;
            isDashing = false;
            //ici la 

        }
        else if (isDashing)
        {
            _rb.velocity = dashDirection * dashSpeed;
        }

        if (_rb.velocity.y < 0 && smoothDash)
        {
            _rb.velocity += Vector2.down * descentGravity * Time.deltaTime;
        }

        if (IsGrounded() && dashDirection == Vector3.zero)
        {
            if (_gameManager.GetSideValueBetweenTwoPoints(transform.position, limit.transform.position, limit.transform.forward) < 0)
            {
                //    _rb.velocity = new Vector2(0, -1) * speed;
                _rb.velocity = Vector2.left * speed;
            }
            else
            {
                _rb.velocity = Vector2.zero;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, -Vector2.up, 1.25f, 1 << 6);
    }

    private Vector3 ConvertPoint(Vector3 point) 
    {
       Vector3 screenPosition = new Vector3(point.x, point.y,-Camera.main.transform.position.z);
       Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
       return worldPosition;
    }
    


    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Ground")
        {
            Debug.Log("Le player touche le sol wesh");
            StartCoroutine(StartAndDestroyAnim());
        }
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

            Debug.Log(dObj.dashDirection);

            if ((int)angle <= targetAngle || dObj.dashDirection == DashDirection.ALL)
            {
                Debug.Log("destroy");
                SpriteCutterOutput output = SpriteCutter.Cut( new SpriteCutterInput() 
                {
                    lineStart = _startObstaclePosition,
                    lineEnd = transform.position,
                    gameObject = col.gameObject,
                    gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
                } );
                
                Debug.Log("output " + output + "second " + output.secondSideGameObject);
                
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
    public IEnumerator StartAndDestroyAnim()
    {
        var myNewSmoke = Instantiate(landingAnimation, groundDetection.transform.position, Quaternion.identity);
        myNewSmoke.transform.parent = gameObject.transform;
        yield return new WaitForSeconds(0.389f);
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
  
        /*
        Debug.Log("Les points totaux" + _dashPoint);
        Debug.Log("le combot point est de " + comboPoint);
*/
        
    }
}
