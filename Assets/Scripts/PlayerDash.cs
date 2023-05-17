using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    

}
