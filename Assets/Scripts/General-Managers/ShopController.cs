using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
    [SerializeField] private Button[] shopButtons;
    [SerializeField] private DialogDisplay dm;
    [SerializeField] private DialogData data;
    [SerializeField] private GameObject shopBlock;
    [SerializeField] private TextMeshProUGUI coinsText, potionText;
    [SerializeField] private Image potionImage;

    private int atkUPLevel=0, hpUPLevel=0, potionLevel=0;
    private MinerRank tempRank;

    private void OnEnable() 
    {
        DialogDisplay.OnDialogComplete += CloseBlock;
        LevelCompleteMenu.OnRankUp += ResetPotionsOnRankUp;
        GameManager.OnCoinsDecrease += UpdateCoinsText;
        GameManager.OnGameResetting += ResetRanksOnNewGame;
        GameManager.PotionSuccessfulPurchase += UpdatePotions;
    }

    private void OnDisable() 
    {
        DialogDisplay.OnDialogComplete -= CloseBlock;    
        LevelCompleteMenu.OnRankUp -= ResetPotionsOnRankUp;
        GameManager.OnCoinsDecrease -= UpdateCoinsText;
        GameManager.OnGameResetting -= ResetRanksOnNewGame;
        GameManager.PotionSuccessfulPurchase -= UpdatePotions;
    }
    public void ResetShop() 
    {
        GameManager.i.SetIsInShop(true); 
        coinsText.text = GameManager.i.GetCoinTotal().ToString();
        shopBlock.SetActive(true);
        dm.OpenDialog(data);
        PopulateItems();
        UpdatePotions(GameManager.i.GetCurrentPotion());
    }

    private void UpdateCoinsText(){ coinsText.text = GameManager.i.GetCoinTotal().ToString(); }
    private void UpdatePotions(ItemData item)
    { 
        potionText.text = GameManager.i.GetPotionTotal().ToString();
        potionImage.sprite = item.uiIcon;
    }

    private void ResetPotionsOnRankUp() { potionLevel = 0; }
    private void ResetRanksOnNewGame(){ atkUPLevel = 0; hpUPLevel = 0; potionLevel = 0; }

    private void PopulateItems()
    {
        tempRank = GameManager.i.GetCurrentRank();

        if(potionLevel < tempRank.availablePotionItems.Length)
        {
            shopButtons[0].GetComponent<ShopButton>().SetButtonData(tempRank.availablePotionItems[potionLevel], this);
        }

        if(hpUPLevel < tempRank.availableHPUpgradeItems.Length)
        {
            shopButtons[1].GetComponent<ShopButton>().SetButtonData(tempRank.availableHPUpgradeItems[hpUPLevel], this);
        }
        
        if(atkUPLevel < tempRank.availableATKUpgradeItems.Length)
        {
            shopButtons[2].GetComponent<ShopButton>().SetButtonData(tempRank.availableATKUpgradeItems[atkUPLevel], this);
        }
    }
    private void CloseBlock() { shopBlock.SetActive(false); }

    public void IncreasePotionLevel() 
    { 
        potionLevel++;
        if(potionLevel < tempRank.availablePotionItems.Length-1)
        {
            shopButtons[0].GetComponent<ShopButton>().SetLowerRank(tempRank.availablePotionItems[potionLevel]);
        }
        else
            shopButtons[0].GetComponent<ShopButton>().SetSoldOut();
        PopulateItems();
    }

    public void IncreaseHPLevel()
    { 
        hpUPLevel++;
        if(hpUPLevel < tempRank.availableHPUpgradeItems.Length-1)
        {
            shopButtons[1].GetComponent<ShopButton>().SetLowerRank(tempRank.availableHPUpgradeItems[hpUPLevel]);
        }
        else
            shopButtons[1].GetComponent<ShopButton>().SetSoldOut();
        PopulateItems();
    }
    public void IncreaseAttackLevel() 
    { 
        atkUPLevel++;
        Debug.Log(hpUPLevel + " < HP Length " + (tempRank.availableHPUpgradeItems.Length-1));
        if(atkUPLevel < tempRank.availableATKUpgradeItems.Length-1)
        {
            shopButtons[2].GetComponent<ShopButton>().SetLowerRank(tempRank.availableATKUpgradeItems[atkUPLevel]);
        }
        else
            shopButtons[2].GetComponent<ShopButton>().SetSoldOut();
        PopulateItems();
    }

    
}
