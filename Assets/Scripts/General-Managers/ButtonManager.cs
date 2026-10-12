using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI totalText, descriptionText, toolTipText;
    [SerializeField] private GameObject toolTip;
    [SerializeField] private Image potionImage;
    private ItemData itemData;

    public void SetItemData(ItemData item) { itemData = item; descriptionText.text = item.itemName; potionImage.sprite = itemData.uiIcon;}
    public ItemData GetItemData() { return itemData; }
    public void UpdatePotionTotal(int amount) { totalText.text = amount.ToString();}

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.SetActive(true);
        if(itemData == null) toolTipText.text = "Poison deals 2 damage after every move for 3 moves";
        else toolTipText.text = itemData.toolTip;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
    }
}
