using System.Collections;
using System.Collections.Generic;
using PlasticGui.Configuration.CloudEdition.Welcome;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class GameManagerSetup
{

    private static string _lastSceneName;
    private static GameManager _sceneGameManager;
    private static SceneLoader _sceneSceneLoader;
    
    static GameManagerSetup()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (!EditorApplication.isPlaying && SceneManager.GetActiveScene().name != _lastSceneName && SceneManager.GetActiveScene().name != "MainMenu")
        {
            _sceneGameManager = null;
            _sceneSceneLoader = null;
            if (!IsValid(SceneManager.GetActiveScene()))
            {
                if (_sceneGameManager == null)
                {
                    GameObject gmObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/GameManager.prefab");
                    PrefabUtility.InstantiatePrefab(gmObj);
                    Debug.Log("Un objet de type GameManager vient d'être crée. ");
                }

                if (_sceneSceneLoader == null)
                {
                    GameObject sLoader = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/SceneLoader.prefab");
                    PrefabUtility.InstantiatePrefab(sLoader);
                    Debug.Log("Un objet de type SceneLoader vient d'être crée. ");
                }
            }
        }
        else if (EditorApplication.isPlaying)
        {
            
            Debug.Log("isPlaying " + GameObject.FindObjectsOfType<SceneLoader>().Length);
            
            if (GameObject.FindObjectsOfType<SceneLoader>().Length >= 2)
            {
                Debug.Log("There are 2 scene loader in the scene. Please ensure there is always exactly one audio listener in the scene.");
                GameObject.Destroy(GameObject.FindObjectsOfType<SceneLoader>()[0]);
            }
        }
        
        
        _lastSceneName = SceneManager.GetActiveScene().name;
        
    }

    private static bool IsValid(Scene currentScene)
    {
        bool isValid = false;

        
        foreach (GameObject obj in currentScene.GetRootGameObjects())
        {

            if (obj.GetComponent<GameManager>() != null || obj.GetComponentInChildren<GameManager>() != null)
            {
                _sceneGameManager = obj.GetComponent<GameManager>() ?? obj.GetComponentInChildren<GameManager>();
            }

            if (obj.GetComponent<SceneLoader>() != null || obj.GetComponentInChildren<SceneLoader>() != null)
            {
                _sceneSceneLoader = obj.GetComponent<SceneLoader>() ?? obj.GetComponentInChildren<SceneLoader>();
            }
        }
        
        return _sceneGameManager != null && _sceneSceneLoader != null;
    }

  
}
