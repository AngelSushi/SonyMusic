using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogDisplay : MonoBehaviour {

    public GameObject dialogParent;
    private Text _dialogText;
    private Image _dialogAuthor;
    private Image _dialogBackground;
    public InputActionAsset inputAsset;
    private DialogController.DialogContent _currentDialog;

    private bool _isInDialog;

    public bool IsInDialog
    {
        get => _isInDialog;
        private set => _isInDialog = value;
    }
    
    private bool _isDisplayFinished;
    private float _originalSpeed;

    public event UnityAction<int> OnDialogStart;
    public event UnityAction<int,string> OnWordAction;
    public event UnityAction<int> OnDialogEnd;
    
    private void Start() 
    {
        inputAsset.FindAction("Player/TouchScreen").started += OnTouchScreen;
        _dialogText = dialogParent.transform.GetChild(1).GetComponent<Text>();
        _dialogAuthor = dialogParent.transform.GetChild(2).GetComponent<Image>();
        _dialogBackground = dialogParent.GetComponent<Image>();

        Debug.Log("dialogAuthor " + _dialogAuthor);

        if (SceneManager.GetActiveScene().name == "Dialog")
        {
            StartDialog(1);
        }
    }


    public void StartDialog(int id) 
    {
        dialogParent.SetActive(true);
        
        _currentDialog = DialogController.instance.GetDialogById(id);
        _isInDialog = true;
        
        DialogController.Speaker dialogSpeaker = DialogController.instance.GetSpeakerById(_currentDialog.speakerID);

        _dialogAuthor.sprite = dialogSpeaker.speakerSprite;
        _dialogBackground.sprite = dialogSpeaker.backgroundSprite;
        _isDisplayFinished = false;
        
        OnDialogStart?.Invoke(_currentDialog.dialogID);
        
        if (_currentDialog.content.Contains("%name%"))
        {
            _currentDialog.content = _currentDialog.content.Replace("%name%", PlayerPrefs.GetString("user_name"));
        }
        
        StartCoroutine(ShowText(_currentDialog));
    }
    
    private IEnumerator ShowText(DialogController.DialogContent dialogContent) 
    {
        for (int i = 1; i < dialogContent.content.Length + 1; i++)
        {
            _currentDialog.speed = 1;
            yield return new WaitForSecondsRealtime(dialogContent.speed / dialogContent.content.Length);
            _dialogText.text = dialogContent.content.Substring(0,i);
            
            OnWordAction?.Invoke(_currentDialog.dialogID,_dialogText.text);
        }

        _isDisplayFinished = true;

        
    }

    public void OnTouchScreen(InputAction.CallbackContext e) 
    {
        if (e.started) 
        {
            if (_isInDialog && _isDisplayFinished) 
            {
                OnDialogEnd?.Invoke(_currentDialog.dialogID);
                
                _currentDialog.speed = _originalSpeed;
                _originalSpeed = 0f;
                
                if (_currentDialog.nextID >= 0) 
                    StartDialog(_currentDialog.nextID);
                else 
                    EndDialog();
                
            }
            else if (_isInDialog && _originalSpeed == 0f)
            {
                _originalSpeed = _currentDialog.speed;
                _currentDialog.speed /= 2;
            }
        }
    }

    private void EndDialog()
    {
        if (dialogParent != null)
        {
            dialogParent.SetActive(false);
        }
        
        _isInDialog = false;
        _isDisplayFinished = false;
    }
}
