using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer : MonoBehaviour
{

    private Vector3 _startPosition;
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                _startPosition = ConvertPoint(touch.position);

            if (touch.phase == TouchPhase.Ended)
            {
                Vector3 endPosition = ConvertPoint(touch.position);

                Vector3 direction = (endPosition - _startPosition).normalized;

                RaycastHit2D[] hits = Physics2D.RaycastAll(_startPosition, direction);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null)
                    {
                        Debug.Log("collide with " + hit.collider.name);
                    }
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
