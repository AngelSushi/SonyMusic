using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(player);
        SceneManager.LoadScene("camMovement");
    }
}
