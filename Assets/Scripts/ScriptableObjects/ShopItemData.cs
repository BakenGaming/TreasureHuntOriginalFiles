using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum ShopType
{
    potion, hpUp, atkUp, none
}
[CreateAssetMenu]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    [TextArea(3, 3)]
    public string itemDescription;
    public int itemCost;
    public int quantity;
    public int hpIncrease;
    public int atkIncrease;
    public Sprite image;
    public ItemData data;
    public ShopType type;
}
