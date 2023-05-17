using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogDisplay : MonoBehaviour {

    public GameObject dialogParent;
    private TextMeshProUGUI _dialogText;
    private Image _dialogAuthor;
    public InputActionAsset inputAsset;
    private DialogController.DialogContent _currentDialog;

    public bool displayDialog;
    private bool _isDisplayFinished;
    

    private void Start() 
    {
        inputAsset.FindAction("Player/TouchScreen").started += OnTouchScreen;
        _dialogText = dialogParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _dialogAuthor = dialogParent.transform.GetChild(1).GetComponent<Image>();
        StartDialog(0);
    }


    public void StartDialog(int id) 
    {
        dialogParent.SetActive(true);
        _currentDialog = DialogController.instance.GetDialogById(id);
        displayDialog = true;
        DialogController.Speaker dialogSpeaker = DialogController.instance.GetSpeakerById(_currentDialog.speakerID);
        _dialogAuthor.sprite = dialogSpeaker.speakerSprite;
        _isDisplayFinished = false;
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
