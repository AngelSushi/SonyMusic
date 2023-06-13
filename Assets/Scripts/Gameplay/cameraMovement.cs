using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    void Update()
    {
        transform.position += new Vector3(5f * Time.deltaTime, 0f, 0f);
    }
}
