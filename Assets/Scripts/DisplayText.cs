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
        Debug.Log("start");
    }

    public void Create()
    {
        Debug.Log("create name");
        PlayerPrefs.SetString("user_name", display.text);
        PlayerPrefs.Save();
    }

    private void SwitchScene()
    {
        Debug.Log("switch scene");
        GameManager.instance.ChangeSceneWithAnim("Dialog");
    }
}
