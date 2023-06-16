using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlayRandom(SoundState.MainMenuTheme);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("PlayerSelection");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
