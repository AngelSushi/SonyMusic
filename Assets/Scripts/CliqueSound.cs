using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliqueSound : MonoBehaviour
{
    public void CliqueSoundStart()
    {
        AudioManager.instance.PlayRandom(SoundState.Click);
    }
    public void CliqueSoundStartGameplay()
    {
        AudioManager.instance.Stop(SoundState.MainMenuTheme);
        AudioManager.instance.PlayRandom(SoundState.Click);
    }
    
}
