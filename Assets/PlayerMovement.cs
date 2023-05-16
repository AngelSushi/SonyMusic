using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float jumpForce;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGrounded;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGrounded);

        if(_isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            _rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            if(jumpTimeCounter > 0)
            {
                _rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }


}
