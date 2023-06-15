using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerDash>(out PlayerDash playerDash))
        {
            if (!playerDash.HasReachBercy)
            {
                Debug.Log("je suis mort ");
                playerDash.isDashing = false;
                Destroy(playerDash.gameObject);
                SceneManager.LoadScene("Gameplay");   
            }
        }
    }
}
