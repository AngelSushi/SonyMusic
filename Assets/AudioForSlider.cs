using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioForSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer _myAudio;

    public void SetMasterVolume(float sliderValue)
    {
        _myAudio.SetFloat("Master", Mathf.Log10(sliderValue) * 20);
    }
    public void SetMusicVolume(float sliderValue)
    {
        _myAudio.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
    }
    public void SetSFXVolume(float sliderValue)
    {
        _myAudio.SetFloat("SFX", Mathf.Log10(sliderValue) * 20);
    }
}
