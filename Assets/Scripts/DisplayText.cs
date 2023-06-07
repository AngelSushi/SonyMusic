using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    public InputField display;

    private void Start()
    {
        FindObjectOfType<Button>().onClick.AddListener(SwitchScene);
    }

    public void Create()
    {
        PlayerPrefs.SetString("user_name", display.text);
        PlayerPrefs.Save();
    }

    private void SwitchScene()
    {
        GameManager.instance.ChangeScene("VisualNovel");
        AudioManager.instance.Stop(SoundState.MainMenuTheme);
        //GameManager.instance.ChangeScene("add anim test");
        AudioManager.instance.PlayRandom(SoundState.GameTheme);
    }
}
