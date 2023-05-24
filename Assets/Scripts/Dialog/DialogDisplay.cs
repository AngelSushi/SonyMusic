using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogDisplay : MonoBehaviour {

    public GameObject dialogParent;
    private Text _dialogText;
    private Image _dialogAuthor;
    private Image _dialogBackground;
    public InputActionAsset inputAsset;
    private DialogController.DialogContent _currentDialog;

    public bool displayDialog;
    private bool _isDisplayFinished;
    

    private void Start() 
    {
        inputAsset.FindAction("Player/TouchScreen").started += OnTouchScreen;
        _dialogText = dialogParent.transform.GetChild(0).GetComponent<Text>();
        _dialogAuthor = dialogParent.transform.GetChild(1).GetComponent<Image>();
        _dialogBackground = dialogParent.GetComponent<Image>();
        StartDialog(1);
    }


    public void StartDialog(int id) 
    {
        dialogParent.SetActive(true);
        
        _currentDialog = DialogController.instance.GetDialogById(id);
        displayDialog = true;

        DialogController.Speaker dialogSpeaker = DialogController.instance.GetSpeakerById(_currentDialog.speakerID);
        _dialogAuthor.sprite = dialogSpeaker.speakerSprite;
        _dialogBackground.sprite = dialogSpeaker.backgroundSprite;
        _isDisplayFinished = false;
        _currentDialog.beginAction?.Invoke();
        StartCoroutine(ShowText(_currentDialog));
    }
    
    private IEnumerator ShowText(DialogController.DialogContent dialogContent) 
    {
        for (int i = 1; i < dialogContent.content.Length + 1; i++)
        {
            yield return new WaitForSeconds(dialogContent.speed / dialogContent.content.Length);
            _dialogText.text = dialogContent.content.Substring(0, i);
        }

        _isDisplayFinished = true;

        
    }

    public void OnTouchScreen(InputAction.CallbackContext e) 
    {
        if (e.started) 
        {
            if (displayDialog && _isDisplayFinished) 
            {
                _currentDialog.endAction?.Invoke();
                if (_currentDialog.nextID >= 0) 
                    StartDialog(_currentDialog.nextID);
                else 
                    EndDialog();
                
            }
        }
    }

    private void EndDialog() 
    {
        dialogParent.SetActive(false);
        displayDialog = false;
        _isDisplayFinished = false;
    }
}
