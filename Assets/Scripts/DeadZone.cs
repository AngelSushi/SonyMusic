using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(player);
        SceneManager.LoadScene("camMovement");
    }
}
