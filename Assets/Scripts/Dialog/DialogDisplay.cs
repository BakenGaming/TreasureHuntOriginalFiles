using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class DialogDisplay : MonoBehaviour
{
    public static event Action OnDialogComplete;
    public static DialogDisplay i { get; private set; }
    [SerializeField] private GameObject dialogPanel, forwardButton;
    [SerializeField] private TextMeshProUGUI dialogBox;

    private DialogManager dialogManager;
    private string currentDialog;

    private void Awake()
    {
        i = this;
    }

    public void Finish()
    {
        /*if (dialogPanel.activeInHierarchy == false)
        {
            forwardButton.SetActive(false);
            OnDialogComplete?.Invoke();
            return;
        }*/
        forwardButton.SetActive(false);
        dialogBox.text = string.Empty;
        StopAllCoroutines();
        dialogBox.text = currentDialog;
        dialogManager = null;
        OnDialogComplete?.Invoke();
    }

    public void OpenDialog(DialogData dialogData)
    {
        dialogManager = null;
        forwardButton.SetActive(false);
        //dialogPanel.SetActive(true);
        dialogBox.text = string.Empty;
        dialogManager = new DialogManager(dialogData, .07f);
        StartConversation();
    }

    public void StartConversation()
    {
        StartCoroutine(DelayForward());
        dialogBox.text = string.Empty;
        currentDialog = dialogManager.GetRandomDialogLine();
        StartCoroutine(ReadText());
    }

    IEnumerator DelayForward()
    {
        yield return new WaitForSeconds(.5f);
        forwardButton.SetActive(true);
    }

    IEnumerator ReadText()
    {
        foreach (char c in currentDialog.ToCharArray())
        {
            dialogBox.text += c;
            SoundManager.PlaySound(SoundManager.Sound.text);
            yield return new WaitForSeconds(dialogManager.GetSpeed());
            
        }
    }
}
