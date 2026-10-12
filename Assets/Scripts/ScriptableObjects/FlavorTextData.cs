using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FlavorTextData : ScriptableObject
{
    [TextArea(3, 3)]
    public List<string> text = new List<string>();
}
