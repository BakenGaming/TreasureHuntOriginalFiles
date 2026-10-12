using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MinerRank : ScriptableObject
{
    public string rankName;
    public int minNumberOfEmpty;
    public int maxNumberOfEmpty;
    public int minNumberOfCoins;
    public int maxNumberOfCoins;
    public int minNumberOfPotions;
    public int maxNumberOfPotions;
    public int minNumberOfTraps;
    public int maxNumberOfTraps;
    public int minNumberOfEnemies;
    public int maxNumberOfEnemies;
    public ShopItemData[] availablePotionItems;
    public ShopItemData[] availableHPUpgradeItems;
    public ShopItemData[] availableATKUpgradeItems;
}
