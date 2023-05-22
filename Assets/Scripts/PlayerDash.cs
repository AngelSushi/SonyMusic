using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnitySpriteCutter;

public class PlayerDash : MonoBehaviour {

    [SerializeField] private float dashSpeed;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;


    [SerializeField] private float dashDistance;
    
    private Vector3 _playerPosition;
    public bool _isDashing;
    
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

                Vector3 direction = (_endPosition - _startPosition).normalized;
                _rb.velocity = direction * dashSpeed;

                Debug.Log("dash");
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
            
            Debug.Log("dash hit trigger ");

            Vector3 localStart = col.transform.position - (Vector3)col.ClosestPoint(transform.position);

            Debug.Log("localStart " + localStart);

            col.gameObject.transform.GetChild(0).position =new Vector3(col.transform.position.x - localStart.x, col.transform.position.y - localStart.y,localStart.z);;


            /*   SpriteCutterOutput output = SpriteCutter.Cut( new SpriteCutterInput() 
               {
                   lineStart = new Vector3(col.transform.position.x -localStart.x, col.transform.position.y -localStart.y,localStart.z),
                   lineEnd = localStart,
                   gameObject = col.gameObject,
                   gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
               } );
   
               if ( output != null && output.secondSideGameObject != null ) 
               {
                   Rigidbody2D newRigidbody = output.secondSideGameObject.AddComponent<Rigidbody2D>();
                   newRigidbody.velocity = output.firstSideGameObject.GetComponent<Rigidbody2D>().velocity;
               }
               
               */
        }
    }
}
