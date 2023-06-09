using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(EventManager))]
public class GameManager : CoroutineSystem
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

    private EventManager _eventManager;
    public EventManager Event
    {
        get => _eventManager;
    }

    public GameObject SceneTransitionCanvas
    {
        get;
        set;
    }
    

    [SerializeField] private float minLoadDuration;

    private bool _isTransitioning;

    public bool IsTransitioning
    {
        get => _isTransitioning;
        set => _isTransitioning = value;
    }

    private Vector3 _startPoint, _endPoint;
    private GameObject _player;
    private float _transitionStartTime;

    private float _sceneProgress;
    
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

        _eventManager = GetComponent<EventManager>();
    }

    private void Update()
    {
        if (_isTransitioning)
        {
            float progress = Mathf.Clamp01((Time.time - _transitionStartTime) / (minLoadDuration / _sceneProgress));
            _player.transform.position = Vector3.Lerp(_startPoint, _endPoint,progress);
        }
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            AudioManager.instance.Stop(SoundState.Voiture);
            AudioManager.instance.Stop(SoundState.GameTheme);
        }
        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            AudioManager.instance.Stop(SoundState.Voiture);
            AudioManager.instance.Stop(SoundState.GameTheme);
        }


    }

    public void Win()
    {
        ChangeSceneWithAnim("Bercy");
    }
    
    
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

   

    public void ChangeSceneWithAnim(string sceneName)
    {
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas != null && canvas != SceneTransitionCanvas.GetComponent<Canvas>())
            {
                canvas.gameObject.SetActive(false);
            }
        }
        
        
        SceneTransitionCanvas.transform.parent.gameObject.SetActive(true);
        
        SceneTransitionCanvas.transform.GetChild(0).gameObject.SetActive(true);
        SceneTransitionCanvas.transform.parent.GetChild(6).gameObject.SetActive(true);
        
        int chapterIndex = 0;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sName = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);

            if (sName.Equals(sceneName))
            {
                chapterIndex = i;
                break;
            }
        }
        
        
        EnableChildren(true);
        SceneTransitionCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        SceneTransitionCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        SceneTransitionCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Chapitre " + chapterIndex;
        SceneTransitionCanvas.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = sceneName;

        RunDelayed(1.5f, () =>
        {
            Transform parent = SceneTransitionCanvas.transform.parent;
            _player = parent.GetChild(1).gameObject;
            
            _startPoint = parent.GetChild(3).gameObject.transform.position;
            _endPoint = parent.GetChild(4).gameObject.transform.position;

            if ((int)_startPoint.y != (int)_endPoint.y)
            {
                _endPoint.y = _startPoint.y;
            }

            StartCoroutine(StartLoadAnim(sceneName));
        });

    }

    private IEnumerator StartLoadAnim(string sceneName)
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName);
        _isTransitioning = true;
        sceneLoading.allowSceneActivation = false;
        _transitionStartTime = Time.time;
        
        while (!sceneLoading.isDone)
        {
            _sceneProgress = sceneLoading.progress;
            if (_sceneProgress >= 0.9f && Time.time - _transitionStartTime >= minLoadDuration && Vector2.Distance(_player.transform.position,_endPoint) < 0.2f)
            {
                sceneLoading.allowSceneActivation = true;
            } 
            
            yield return null;
        }

        _isTransitioning = false;
        _player.transform.position = _startPoint - Vector3.left * 50;
        
      //  SceneTransitionCanvas.transform.parent.gameObject.SetActive(false);
        EnableChildren(false);
    }

    private void EnableChildren(bool enabled)
    {
        Transform parent = SceneTransitionCanvas.transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(enabled);
        }
        
        Camera.main.gameObject.SetActive(!enabled);
    }

    public float GetSideValueBetweenTwoPoints(Vector3 first, Vector3 second,Vector3 forward)
    {
        Vector3 delta = (first - second).normalized;
        Vector3 cross = Vector3.Cross(delta, forward);
        
        return cross.y;
    }
}
