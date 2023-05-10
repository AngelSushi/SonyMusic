using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour {

    public bool dashMode;
    public float forceImpulse;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;

    private float _startX;

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _startX = transform.position.x;
    }
    
    void Update() {
        if (Input.touchCount > 0) {

            Touch touch = Input.GetTouch(0);
            
            if (!dashMode) 
                transform.position = ConvertPoint(new Vector3(_startX,touch.position.y,0));
            else {
                if (touch.phase == TouchPhase.Began) 
                    _startPosition = ConvertPoint(touch.position);
                else if (touch.phase == TouchPhase.Ended) {
                    _endPosition = ConvertPoint(touch.position);

                    Vector3 direction = (_endPosition - _startPosition).normalized;
                    
                    Debug.Log("direction " + direction);
                    _rb.AddRelativeForce(direction * forceImpulse,ForceMode2D.Impulse);
                }
            }
        }


    }

    private Vector3 ConvertPoint(Vector3 point) {
        Vector3 screenPosition = new Vector3(point.x, point.y,-Camera.main.transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}
