using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBlink : MonoBehaviour
{
    [SerializeField] private float maxIntensity;
    [SerializeField] private float speed;

    private Light2D _light2D;
    private float _progress;
    void Start()
    {
        _light2D = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _progress = Mathf.PingPong(Time.time * speed, maxIntensity);
        _light2D.intensity = _progress;
    }
}
