using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    public Text playerTexte;
    public InputField display;

    private void Start()
    {
        playerTexte.text = PlayerPrefs.GetString("user_name");
    }

    public void Create()
    {
        playerTexte.text = display.text;
        PlayerPrefs.SetString("user_name", playerTexte.text);
        PlayerPrefs.Save();
    }
}
