using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UnitySpriteCutter;
using Slider = UnityEngine.UI.Slider;

public class PlayerDash : MonoBehaviour {

    [Header("Dash Values")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDistance;
    [SerializeField] private int angleOffset;
    [SerializeField] private int diagonalAngleOffset;

    [Header("Score")]
    [SerializeField] private float pointPerDash;
    [SerializeField] private float maxDashPoint;
    [SerializeField] private Slider dashSlider;
    
    [Header("Debug")]
    [SerializeField] private bool debugDash;
    
    [HideInInspector] public bool isDashing;
    
    
    
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;
    private Vector3 _playerPosition;
    private Vector3 _startObstaclePosition;
    private Vector3 _dashDirection;
    private float _dashPoint;
    
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

                _dashDirection = (_endPosition - _startPosition).normalized;
                _rb.velocity = _dashDirection * dashSpeed;

                _playerPosition = transform.position;
                isDashing = true;

            }
        }


        if (Vector3.Distance(_playerPosition, transform.position) > dashDistance && isDashing)
        {
            _rb.velocity = Vector2.zero;
            isDashing = false;
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
        Debug.Log("name " + col.gameObject.name + " isDashing " + isDashing + " velocity  " + _rb.velocity);
        
        if (col.gameObject.layer == LayerMask.NameToLayer("Destructible") && isDashing)
        {
            if (debugDash)
            {
                col.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                col.gameObject.transform.GetChild(0).position = transform.position;
            }
            
            _startObstaclePosition = transform.position;
            
            Debug.Log("startPosition " + _startObstaclePosition);

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
            
            Debug.Log("startPosition " + _startObstaclePosition);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Destructible") && isDashing)
        {
            if (debugDash)
            {
                col.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                Debug.Log("secondPos " + transform.position + "isDashing " + isDashing);
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
            
            
            float angle = Vector3.Angle(_dashDirection,dir);
            Debug.Log("angle " + (int)angle + " " + targetAngle);


            if ((int)angle <= targetAngle)
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
