using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{

    private GameManager _gameManager;
    
    void Start()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
        _gameManager = GameManager.instance;
        _gameManager.SceneTransitionCanvas = transform.gameObject;
    }

}
