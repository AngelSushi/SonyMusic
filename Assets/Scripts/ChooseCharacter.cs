using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseCharacter : MonoBehaviour
{
    public void MenButton()
    {
        GameManager.instance.isMen = true;
        GameManager.instance.isWomen = false;
        SceneManager.LoadScene("NameSelection");
    }
    public void WomenButton()
    {
        GameManager.instance.isWomen = true;
        GameManager.instance.isMen = false;
        SceneManager.LoadScene("NameSelection");

    }

}
