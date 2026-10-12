using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : MonoBehaviour, ICollectable
{
    public static event Action<int> OnCoinCollected;
    public static event Action<ItemData> OnPotionCollected;
    [SerializeField] private ItemData itemData;
    public void Collect()
    {
        if(itemData.type == ItemType.potion) 
        {
            if(GameManager.i.GetPotionTotal() == 3 && GameManager.i.GetCurrentPotion() == itemData) 
            {
                SystemMessage.Create("Potions Full");
            }
            else 
            {
                Destroy(gameObject);
                OnPotionCollected?.Invoke(itemData);
            }
        }
        if(itemData.type == ItemType.coin) { OnCoinCollected?.Invoke(itemData.value); Destroy(gameObject); }
    }
}
