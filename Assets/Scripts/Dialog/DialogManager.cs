using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager
{
    private DialogClass[] dialogLines;
    private float textSpeed;
    public DialogManager(DialogData dialogData, float tSpeed)
    {
        dialogLines = new DialogClass[dialogData.dialog.Length];
        for (int i = 0; i < dialogData.dialog.Length; i++)
        {
            dialogLines[i] = dialogData.dialog[i];
        }
        textSpeed = tSpeed;
    }

    public float GetSpeed()
    {
        return textSpeed;
    }

    public string GetRandomDialogLine()
    {
        return dialogLines[Random.Range(0, dialogLines.Length)].text;
    }
}
