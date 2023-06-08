using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isMen
    {
        get;
        set;
    }

    public bool isWomen
    {
        get;
        set;
    }

    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public float GetSideValueBetweenTwoPoints(Vector3 first, Vector3 second,Vector3 forward)
    {
        Vector3 delta = (first - second).normalized;
        Vector3 cross = Vector3.Cross(delta, forward);
        
        return cross.y;
    }
}
