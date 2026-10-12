using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

public class ShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<int> OnHPUpPurchase;
    public static event Action<int> OnATKUpPurchase;
    public static event Action<ShopItemData> OnPotionPurchase;
    [SerializeField] private Image itemSprite;
    [SerializeField] private GameObject toolTip;
    [SerializeField] private TextMeshProUGUI toolTipText, cost, desc;
    private ShopController controller;
    private ShopItemData item;
    private int quantity;
    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.SetActive(true);
        toolTipText.text = UpdateToolTipText();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
    }

    public void SetButtonData(ShopItemData data, ShopController c)
    {
        controller = c;
        item = data;
        quantity = item.quantity;
        cost.enabled = true;

        switch(item.type)
        {
            case ShopType.potion:
                itemSprite.sprite = item.image;
                cost.text = item.itemCost.ToString();
                desc.text = item.itemName;
            break;
            case ShopType.hpUp:
                itemSprite.sprite = item.image;
                cost.text = item.itemCost.ToString();
                desc.text = item.itemName;
            break;
            case ShopType.atkUp:
                itemSprite.sprite = item.image;
                cost.text = item.itemCost.ToString();
                desc.text = item.itemName;
            break;
            case ShopType.none:
                itemSprite.sprite = item.image;
                cost.enabled = false;
                desc.text = item.itemName;
                desc.fontSize = 40;
            break;
        }
    }

    public void BuyItem()
    {
        if(quantity > 0)
        {
            if(GameManager.i.GetCoinTotal() >= item.itemCost)
            {
                switch(item.type)
                {
                    case ShopType.potion:
                        if(item.data == GameManager.i.GetCurrentPotion())
                        {
                            if(GameManager.i.GetPotionTotal() < 3)
                            {
                                GameManager.i.DecreaseCoins(item.itemCost);
                                SystemMessage.Create("Potion Purchased");
                                toolTipText.text = UpdateToolTipText();
                                OnPotionPurchase?.Invoke(item);
                                controller.IncreasePotionLevel();
                            } else SystemMessage.Create("Potions Full");                            
                        }
                        else
                        {
                            GameManager.i.ResetPotions();
                            GameManager.i.UpdateCurrentPotion(item.data);
                            GameManager.i.DecreaseCoins(item.itemCost);
                            SystemMessage.Create("New Potion Purchased");
                            toolTipText.text = UpdateToolTipText();
                            OnPotionPurchase?.Invoke(item);
                            controller.IncreasePotionLevel();
                        }
                    break;
                    case ShopType.hpUp:
                        GameManager.i.DecreaseCoins(item.itemCost);
                        SystemMessage.Create("HP Increase Purchased");
                        toolTipText.text = UpdateToolTipText();
                        OnHPUpPurchase?.Invoke(item.hpIncrease);
                        controller.IncreaseHPLevel();
                    break;
                    case ShopType.atkUp:
                        GameManager.i.DecreaseCoins(item.itemCost);
                        SystemMessage.Create("Attack Increase Purchased");
                        toolTipText.text = UpdateToolTipText();
                        OnATKUpPurchase?.Invoke(item.atkIncrease);
                        controller.IncreaseAttackLevel();
                    break;
                    case ShopType.none:
                        SystemMessage.Create("It's a Mystery");
                    break;
                }
            }
            else SystemMessage.Create("Not Enough Gold");
        }
        else SystemMessage.Create("Out of Stock");
    }

    private string UpdateToolTipText()
    {
        return item.itemDescription;
    }

    public void SetSoldOut()
    {
        cost.text = " ";
    }

    public void SetLowerRank(ShopItemData item)
    {
        itemSprite.sprite = item.image;
        cost.text = item.itemCost.ToString();
        desc.text = item.itemName;
    }
}
