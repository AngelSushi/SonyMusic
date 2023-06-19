using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
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

        [Serializable]
        public class WEvent
        {
            public string word;
            public UnityEvent action;
            private bool hasBeenCalled;

            public bool HasBeenCalled
            {
                get => hasBeenCalled;
                set => hasBeenCalled = true;
            }
        }

        public List<WEvent> wordsAction;
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
        _controller.OnWordAction += ControllerOnWordAction;
        _controller.OnDialogEnd += ControllerOnOnDialogEnd;
    }

    private void OnDestroy()
    {
        _controller.OnDialogStart -= ControllerOnOnDialogStart;
        _controller.OnWordAction -= ControllerOnWordAction;
        _controller.OnDialogEnd -= ControllerOnOnDialogEnd;
    }

    private DEvent FindEventByID(int id) => _events.FirstOrDefault(d => d.id == id);
    private void ControllerOnOnDialogStart(int id) => FindEventByID(id)?.startEvent?.Invoke();     
    private void ControllerOnOnDialogEnd(int id) => FindEventByID(id)?.endEvent?.Invoke();

    private void ControllerOnWordAction(int id,string text)
    {
        if (FindEventByID(id) == null)
        {
            return;
        }
        
        foreach (DEvent.WEvent wEvent in FindEventByID(id).wordsAction)
        {
            Debug.Log("text " + text);
            Debug.Log("word " + wEvent.word);
            if (text.Contains(wEvent.word) && !wEvent.HasBeenCalled)
            {
                wEvent.action?.Invoke();
                wEvent.HasBeenCalled = true;
            }
        }
    }
    
}