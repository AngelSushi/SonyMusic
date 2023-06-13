using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnitySpriteCutter;
using Slider = UnityEngine.UI.Slider;

public class PlayerDash : CoroutineSystem
{


    [Header("Movement")] [SerializeField] private float speed;
    
    [Header("Dash")]
    public bool isDashing;
    [SerializeField] private bool beginFromPlayer;
    [SerializeField] private int playerDragAngle;
    [SerializeField] private GameObject slashAnim;
    
    [Header("Dash Values")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private int angleOffset;
    [SerializeField] private int diagonalAngleOffset;
    [SerializeField] private float descentGravity;

    [Header("Score")]
    [SerializeField] private float pointPerDash;
    [SerializeField] private float maxCombo;
    [SerializeField] private Slider dashSlider;
    [SerializeField] private Transform limit;
    [SerializeField] private Transform distanceCombo;
    [SerializeField] private TextMeshProUGUI scoreText;
    private float comboPoint;
    

    [Header("player")]
    public GameObject groundDetection;
    public GameObject landingAnimation;
    public Animator playerAnimator;
    
    [Header("Debug")]
    [SerializeField] private bool debugDash;
    [SerializeField] private bool smoothDash;
    [SerializeField] private bool useButtons;
    [SerializeField] private bool showSlashAreas;
    [SerializeField] private List<GameObject> buttons;
    [SerializeField] private List<GameObject> areas;
    [Range(0,1)]
    [SerializeField] private float slashAreaPercentage;
    
    
    [Header("Animation")]
    public SkeletonAnimation skeletAnimRun;
    public SkeletonAnimation skeletAnimationFall;
    public GameObject skeletRunObj;
    public GameObject skeletFallObj;
    public GameObject skeletDashObj;
    bool animDashDone = false;


    [HideInInspector] public GameObject lastHitPlateform;

    private Vector3 _startPosition;
    private Vector3 _startTouchPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;
    private Vector3 _playerPosition;
    private Vector3 _startObstaclePosition;
    [HideInInspector] public Vector3 dashDirection;
    private float _dashPoint;
    private GameManager _gameManager;

    private bool _return;
    private bool _lastGrounded;
    private float _originalDistance;
    
    
    private List<Plateform> _plateforms;
    private int _plateformIndex;
    
    
    
    void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerPosition = transform.localPosition;
        _gameManager = FindObjectOfType<GameManager>();
        _plateforms = FindObjectsOfType<Plateform>().ToList();

        
        foreach (GameObject button in buttons)
        {
            button.SetActive(useButtons);
        }

        foreach (GameObject area in areas)
        {
            area.SetActive(showSlashAreas);
        }

        areas[0].GetComponent<RectTransform>().anchorMax = new Vector2(1 - slashAreaPercentage, 1f);
        areas[1].GetComponent<RectTransform>().anchorMin = new Vector2(1 - slashAreaPercentage, 0f);



    }

    private void Start()
    {
        _gameManager.Event.OnReleaseObstacle += ReleaseObstacle;
        _originalDistance = dashDistance;
    }

    private void OnDestroy()
    {
        _gameManager.Event.OnReleaseObstacle -= ReleaseObstacle;
    }

    void Update() 
    {
        if(isDashing == true)
        {
            StartCoroutine(DashAnimation());
        }
        
        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _startPosition = ConvertPoint(touch.position);
                _startTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _endPosition = ConvertPoint(touch.position);
                
                if (_startTouchPosition.x <= Screen.width - Screen.width * slashAreaPercentage)
                {
                    Vector2 _direction = (_endPosition - _startPosition).normalized;
                    
                    Dash(_direction.y > 0 ? Vector2.up : Vector2.down,true);
                }
                else
                {
                    if (beginFromPlayer)
                    {
                        foreach (Collider2D col2D in Physics2D.OverlapCircleAll(ConvertPoint(touch.position), playerDragAngle))
                        {
                            if (col2D.gameObject.layer == LayerMask.NameToLayer("Player"))
                            {
                                Dash(false);
                            }
                        }
                    }
                    else
                    {
                        Dash(false);
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
            if (_gameManager.GetSideValueBetweenTwoPoints(transform.position, limit.transform.position, limit.transform.forward) < 0 && _return)
            {
                //    _rb.velocity = new Vector2(0, -1) * speed;
                _rb.velocity = Vector2.left * speed;
            }
            else if(_return)
            {
                _rb.velocity = Vector2.zero;
            }

            if (!_lastGrounded)
            {
                StartCoroutine(StartAndDestroyAnim());
            }
        }

        
        _lastGrounded = IsGrounded();
         
    }

    private bool IsGrounded()
    {
        var result = Physics2D.Raycast(transform.position, -Vector2.up, 1f, LayerMask.GetMask("Ground"));
        if (debugDash)
        {
            if (result.collider != null)
            {
                Debug.DrawRay(transform.position, -Vector2.up * 1f, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, -Vector2.up * 1f, Color.red);
            }
        }

        return result;
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
            Vector3 localEndPosition = Vector3.zero;
            DebugDash(col,localEndPosition);
            
            _startObstaclePosition = transform.position;

            DestroyableObject dObj = col.gameObject.GetComponent<DestroyableObject>();

            if (dObj == null || dObj.IsCut)
            {
                return;
            }

            Vector3 slashDirection = (localEndPosition - transform.localPosition).normalized;
            
            float targetAngle = angleOffset;
            float angle = Vector3.Angle(dashDirection,FindDirection(dObj,col,targetAngle));

            Vector3 slashPosition = col.transform.position;
            slashPosition.z = -2f;
            slashAnim.transform.position = slashPosition;
            
            Debug.DrawRay(col.transform.position, col.transform.up * 10,Color.yellow,10);
            Debug.DrawRay(col.transform.position,slashDirection * 10,Color.magenta,10);
            Debug.DrawRay(col.transform.position,dashDirection * 10,Color.green,10);
            
            
            float animAngle = Vector2.Angle(col.transform.up, slashDirection);

            Debug.Log("animAngle " + animAngle);
            
            Vector3 slashAngles = slashAnim.transform.eulerAngles;
            slashAngles.z = animAngle;

            slashAnim.transform.eulerAngles = slashAngles;



            slashAnim.SetActive(true);
            
            Debug.Break();
            RunDelayed(0.36f, () =>
            {
                slashAnim.SetActive(false);
            });
            
            
            if ((int)angle <= targetAngle || dObj.dashDirection == DashDirection.ALL)
            {
                dObj.IsCut = true;
                AddPoint();
                CutObject(col,localEndPosition);
                //Debug.Break();
            }

        }
    }

    private void DebugDash(Collider2D col,Vector3 localEndPosition)
    {
        if (debugDash)
        {
            col.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            col.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            col.gameObject.transform.GetChild(0).position = transform.position;

            Vector3 localPosition = col.gameObject.transform.GetChild(0).localPosition;
            localEndPosition = new Vector3(-localPosition.x, -localPosition.y, localPosition.z);

            col.gameObject.transform.GetChild(1).localPosition = localEndPosition;
        }
    }

    private void CutObject(Collider2D col,Vector3 localEndPosition)
    {
        SpriteCutterOutput output = SpriteCutter.Cut( new SpriteCutterInput() 
        {
            lineStart = _startObstaclePosition,
            lineEnd = col.transform.TransformPoint(localEndPosition),
            gameObject = col.gameObject,
            gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
        } );
                
        if ( output != null && output.secondSideGameObject != null ) 
        { 
            Rigidbody2D newRigidbody = output.firstSideGameObject.AddComponent<Rigidbody2D>();
            output.secondSideGameObject.GetComponent<MeshRenderer>().material.color = Color.black;
                    
            if (output.secondSideGameObject.GetComponent<Rigidbody2D>() == null)
            {
                Rigidbody2D rb2D = output.secondSideGameObject.AddComponent<Rigidbody2D>();
                rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            newRigidbody.velocity = output.secondSideGameObject.GetComponent<Rigidbody2D>().velocity;
            newRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    
            _startObstaclePosition = Vector3.zero;
        }
    }

    private void Dash(Vector2 _dashDirection,bool switchingPlateform)
    {
        float distance = Vector3.Distance(_startPosition, _endPosition);
                    
        Debug.Log("distance " + distance + " minDistance " + minDistance);
        if (distance >= minDistance /* && (_endPosition - _startPosition).normalized.x > 0 */)
        {
            dashDirection = _dashDirection;
                        
            if (dashDirection.y < 0.1f && !switchingPlateform)
            {
                if (IsGrounded())
                {
                    return;
                }
                else
                {
                                
                }
            }

                        
            _rb.velocity = dashDirection * dashSpeed;
            _playerPosition = transform.position;
            isDashing = true;
            _return = false;
            _gameManager.Event.OnDashLaunched?.Invoke(this,new EventManager.OnDashLaunchedArgs() { dashDirection = _dashDirection,switchingPlateform = switchingPlateform});
        }
    }
    private void Dash(bool switchingPlateform)
    {
        Dash((_endPosition - _startPosition).normalized,switchingPlateform);
    }
    
    
    private Vector3 FindDirection(DestroyableObject dObject,Collider2D col,float targetAngle)
    {
        switch (dObject.dashDirection)
        {
            case DashDirection.UP:
                return col.transform.up;
            case DashDirection.DOWN:
                return col.transform.up * -1;
            case DashDirection.LEFT:
                return col.transform.right;
            case DashDirection.RIGHT:
                return col.transform.right * -1;
            case DashDirection.DIAGONAL_LUP:
                targetAngle = diagonalAngleOffset;
                return col.transform.right + col.transform.up * -1;
            case DashDirection.DIAGONAL_RUP:
                targetAngle = diagonalAngleOffset;
                return col.transform.right * -1 + col.transform.up * -1;
            case DashDirection.DIAGONAL_LDOWN:
                targetAngle = diagonalAngleOffset;
                return col.transform.right + col.transform.up;
            case DashDirection.DIAGONAL_RDOWN:
                targetAngle = diagonalAngleOffset;
                return col.transform.right * -1 + col.transform.up;
            default:
                return col.transform.up;
        }
    }

    public IEnumerator StartAndDestroyAnim()
    {
        Vector3 smokePosition = transform.position;
        smokePosition.y -= 0.5f;
        var myNewSmoke = Instantiate(landingAnimation, smokePosition, Quaternion.identity);

        myNewSmoke.transform.parent = gameObject.transform;
        
        skeletRunObj.SetActive(false);
        skeletDashObj.SetActive(false);
        skeletFallObj.SetActive(true);
        skeletAnimationFall.AnimationName = "Atterissage";
        
        yield return new WaitForSeconds(0.3f);
        
        skeletRunObj.SetActive(true);
        skeletDashObj.SetActive(false);
        skeletFallObj.SetActive(false);
        _return = true;
        skeletAnimRun.AnimationName = "NarutoRun";

    }
    
    private IEnumerator DashAnimation()
    {
        skeletRunObj.SetActive(false);
        skeletDashObj.SetActive(false);
        skeletDashObj.SetActive(true);
        skeletFallObj.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        animDashDone = true;
    }


    private void AddPoint() 
    {
        if (limit.position.x < gameObject.transform.position.x && gameObject.transform.position.x < distanceCombo.position.x)
        {
            comboPoint = comboPoint + 1;
            _dashPoint += pointPerDash * comboPoint;
            scoreText.text = _dashPoint.ToString();
            dashSlider.value = comboPoint / maxCombo;
        }
        else
        {
            comboPoint = 1;
            _dashPoint += pointPerDash * comboPoint;
            scoreText.text = _dashPoint.ToString();
            dashSlider.value = comboPoint / maxCombo;
        }
  
        /*
        Debug.Log("Les points totaux" + _dashPoint);
        Debug.Log("le combot point est de " + comboPoint);
*/
        
    }

    private void ReleaseObstacle(object sender,EventManager.OnReleaseObstacleArgs e)
    {
        if (!e.isCut)
        {
            ResetCombo();
        }
    }
    
    
    public void ResetCombo()
    {
        comboPoint = 0;
        dashSlider.value = 0;
    }

    public void SwitchPlateform(bool up)
    {
        if (!useButtons)
        {
            return;
        }
        
        if (up)
        {
            if (_plateformIndex < _plateforms.Count - 1)
            {
                _plateformIndex++;
                Dash(Vector2.up,true);
            }
        }
        else
        {
            if (_plateformIndex > 0)
            {
                _plateformIndex--;
                Dash(Vector2.down,true);
            }
        }
    }
}
