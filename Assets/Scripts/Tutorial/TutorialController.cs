using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class TutorialController : MonoBehaviour
{

    public enum TState
    {
        MOVEMENT,
        OBSTACLE,
        COMBO,
        MAX_COMBO,
        TRANSFORMATION,
        END
    }

    [Serializable]
    public class TutorialState
    {
        [SerializeField] private TState state;
        [SerializeField] private int dialogID;
        [SerializeField] private UnityEvent endState;
        
        public int DialogID
        {
            get => dialogID;
            private set => dialogID = value;
        }

        public TState State
        {
            get => state;
            private set => state = value;
        }


        public UnityEvent EndState
        {
            get => endState;
            private set => endState = value;
        }
    }

    [SerializeField] private List<TutorialState> tutorialStates;
    
    [SerializeField] private TState actualState;

    private DialogDisplay _dialogDisplay;


    [Header("Movement State")] 
    [SerializeField] private Light2D growLight;

    [SerializeField] private Light2D generatorLight;

    private PlayerDash _player;

    private GameManager _instance;
    void Start()
    {
        _instance = GameManager.instance;
        _instance.Event.OnDestroyObstacle += DestroyObstacle;
        
        _dialogDisplay = FindObjectOfType<DialogDisplay>();
        _player = FindObjectOfType<PlayerDash>();
        
        _dialogDisplay.StartDialog(15);
    }

    private void OnDestroy()
    {
        _instance.Event.OnDestroyObstacle -= DestroyObstacle;
    }

    private void Update()
    {
        switch (actualState)
        {
            case TState.MOVEMENT:
                RaycastHit2D[] hit2D = Physics2D.RaycastAll(_player.transform.position, _player.transform.right, 1,LayerMask.GetMask("TutorialElement"));

                foreach (RaycastHit2D hit in hit2D)
                {
                    if (hit.collider != null)
                    {
                        generatorLight.gameObject.SetActive(false);
                        growLight.gameObject.SetActive(true);
                        SwitchState(TState.OBSTACLE);
                    }
                }
                
                break;
            
            case TState.TRANSFORMATION:
                if (!_player.IsSuperSayen && _player.HasBeenSayen)
                {
                    SwitchState(TState.END);
                }
                break;
        }
    }

    private void SwitchState(TState newState)
    {
        TutorialState lastTState = tutorialStates.Where(state => state.State == actualState).ToList()[0];
        lastTState.EndState?.Invoke();
        
        TutorialState newTState = tutorialStates.Where(tutorialState => tutorialState.State == newState).ToList()[0];
        _dialogDisplay.StartDialog(newTState.DialogID);
        
        actualState = newState;
        

    }

    private void DestroyObstacle(object sender,EventManager.OnDestroyedObstacleArgs e)
    {
        if (actualState == TState.OBSTACLE)
        {
            _player.ComboPoint = 0;
            _player.DashSlider.value = 0;
            SwitchState(TState.COMBO);
            return;
        }

        if (actualState == TState.COMBO)
        {
            SwitchState(TState.MAX_COMBO);
            return;
        }
        
        if (actualState == TState.MAX_COMBO)
        {

            if (_player.CanSayen)
            {
                SwitchState(TState.TRANSFORMATION);
                return;
            }
        }
        
    }
}
