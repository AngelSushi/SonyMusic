using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    [SerializeField] private float speed;
    
    void Update()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
    }
}
