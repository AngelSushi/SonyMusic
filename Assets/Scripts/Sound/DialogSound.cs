using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSound : MonoBehaviour
{
    
    void Start()
    {
        AudioManager.instance.PlayRandom(SoundState.Voiture);
        AudioManager.instance.PlayRandom(SoundState.GameTheme);
    }

    
}
