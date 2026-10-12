using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ItemType
{
    potion, coin
}
[CreateAssetMenu]

public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea(3,3)]
    public string toolTip;
    public int value;
    public Sprite uiIcon;
    public ItemType type;
}
