using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerExit2D(Collider2D collision)
    {
        player.GetComponent<PlayerDash>()._isDashing = false;
        Destroy(player);
        SceneManager.LoadScene("Gameplay");
    }
}
