using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightGrowing : MonoBehaviour
{

    private Light2D _light;

    private bool _startGrowing;

    public bool StartGrowing
    {
        get => _startGrowing;
        set => _startGrowing = value;
    }


    [SerializeField] private float maxGrowing;
    [SerializeField] private float speed;
    [SerializeField] private Light2D globalLight;
    
    private float _progress;
    void Start()
    {
        _light = GetComponent<Light2D>();
        _light.gameObject.SetActive(true);
        _startGrowing = true;
        
        globalLight.gameObject.SetActive(false);
    }

    void Update()
    {
        if (_startGrowing)
        {
             _progress += Mathf.Lerp(0, maxGrowing, Time.deltaTime / speed);
            _light.pointLightOuterRadius = _progress;

            if (_progress >= maxGrowing)
            {
                _light.gameObject.SetActive(false);
                globalLight.intensity = 1f;
                globalLight.gameObject.SetActive(true);
            }
        }
    }
}
