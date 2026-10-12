using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogData : ScriptableObject
{
    public int dialogID;
    [TextArea(3, 3)]
    public string dialogDescription;
    public DialogClass[] dialog;
}


[System.Serializable]
public class DialogClass
{
    public int lineNumber;
    [TextArea(3, 3)]
    public string text;
}
