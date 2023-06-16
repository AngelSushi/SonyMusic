using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    public GameObject buttonPause;


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        buttonPause.SetActive(true);
        //AudioManager.instance.PlayRandom(SoundState.GameTheme);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        buttonPause.SetActive(false);
        //AudioManager.instance.PauseSound(SoundState.GameTheme);
        Time.timeScale = 0f;
        gameIsPaused = true;
        
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
