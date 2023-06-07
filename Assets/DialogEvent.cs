using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class DialogEvent : MonoBehaviour
{
    [Serializable]
    public class DEvent
    {
        public string name;
        public int id;
        public UnityEvent startEvent;
        public UnityEvent endEvent;
    }

    [SerializeField] List<DEvent> _events;
    [SerializeField] private DialogDisplay _controller;

    public List<DEvent> Events
    {
        get => _events;
        set => _events = value;
    }


    private void Reset()
    {
        _controller = GetComponent<DialogDisplay>() ?? GetComponentInChildren<DialogDisplay>();
    }

    private void OnValidate()
    {
        foreach (var el in _events)
        {
            el.name = el.id.ToString();
        }
    }

    private void Start()
    {
        _controller.OnDialogStart += ControllerOnOnDialogStart;
        _controller.OnDialogEnd += ControllerOnOnDialogEnd;
    }

    private void OnDestroy()
    {
        _controller.OnDialogStart -= ControllerOnOnDialogStart;
        _controller.OnDialogEnd -= ControllerOnOnDialogEnd;
    }

    private DEvent FindEventByID(int id) => _events.First(d => d.id == id);
    private void ControllerOnOnDialogStart(int id) => FindEventByID(id).startEvent?.Invoke();
    private void ControllerOnOnDialogEnd(int id) => FindEventByID(id).endEvent?.Invoke();


    
}