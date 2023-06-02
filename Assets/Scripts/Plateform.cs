using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateform : MonoBehaviour
{

    private BoxCollider2D _collider2D;
    private Vector2 _enterDashDirection;

    public float downMin;

    private void Start()
    {
        _collider2D = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerDash playerDash = col.gameObject.GetComponent<PlayerDash>();
            Physics2D.IgnoreCollision(_collider2D,col.collider,playerDash.dashDirection.y < downMin);
        }
    }
    
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StartCoroutine(EnableCollision(col));
        }
        
    }

    private IEnumerator EnableCollision(Collision2D col)
    {
        yield return new WaitForSeconds(1.5f);
        Physics2D.IgnoreCollision(_collider2D,col.collider,false);
    }
}
