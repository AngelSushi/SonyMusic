using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{

    private GameManager _gameManager;
    private static SceneLoader _sceneLoader;
    
    void Start()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
        _gameManager = GameManager.instance;
        
        if(_gameManager.SceneTransitionCanvas == null)
        {
            _gameManager.SceneTransitionCanvas = transform.gameObject;
        }

        if (_sceneLoader == null)
        {
            Debug.Log("here");
            _sceneLoader = this;
        }
        else
        {
            Destroy(transform.parent.gameObject);
            
            
        }
    }

}
