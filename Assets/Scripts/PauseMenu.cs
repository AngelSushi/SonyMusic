using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    public GameObject buttonPause;


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        buttonPause.SetActive(true);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        buttonPause.SetActive(false);
        Time.timeScale = 0f;
        gameIsPaused = true;
        
    }

    public void LoadMenu()
    {
        Debug.Log("faudra mettre la scene vers le menu laaaaaaaaaaaaaaaaaaa");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
