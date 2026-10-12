using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class GameManager : MonoBehaviour
{
    public static event Action<ItemData> PotionSuccessfulCollect;
    public static event Action<ItemData> PotionSuccessfulPurchase;
    public static event Action OnPotionUsed;
    public static event Action OnCoinCollected;
    public static event Action<GameObject, GameObject> OnBattleStart; 
    public static event Action OnGameInitialized;
    public static event Action OnGameResetting;
    
    public static event Action OnHealthIncrease;
    public static event Action OnAttackIncrease;
    public static event Action OnMinerRankIncrease;
    public static event Action OnCoinsDecrease;
    public static event Action OnGameOverWin;
    private static GameManager _i;

    public static GameManager i
    { 
      get
        {
            return _i;
        }  
    }
    [SerializeField] private Transform sysMessagePoint, shopMessagePoint;
    [SerializeField] private NewMapGenerator generator;
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private ItemData startingPotion, currentPotion, lastCollectedPotion;
    [SerializeField] private MinerRank[] availableRanks;
    [SerializeField] private int mapCount=1, rankIndex=0, depthCount=1;
    
    private MinerRank currentRank;

    private GameObject player;

    private GameObject playerStart;
    private List<GameObject> levelObjects = new List<GameObject>();
    
    private bool paused, isInShop = false, uiOpen, audioMuted;
    private int coinTotal=0, potionTotal, coinsCollectedThisLevel, lastPotionCount;
    private StatSystem stats;
    private HealthSystem healthSystem;

    private void Awake() 
    {
        _i = this;  
        SoundManager.Initialize();

        //PauseGame();
        //Initialize(); 
    }

    private void Start() 
    {
        SetMasterVolume(-40);
        SetMusicVolume(0);
        SetSFXVolume(0);
        SoundManager.PlayMusic(SoundManager.Music.mainMenu);    
    }

    private void OnEnable() 
    {
        GameInputController.OnEnterHazard += HandleHazard;
        GameInputController.OnReachedEnd += HandleEnd;
        Item.OnCoinCollected += IncreaseCoins;
        Item.OnPotionCollected += IncreasePotions; 
        EnemyScript.OnEngageEnemy += StartBattle;  
        UIController.OnFadeOutComplete += ResetLevel; 
        Player.OnPlayerDied += ResetCoins;
        ShopButton.OnPotionPurchase += PotionPurchase;
        ShopButton.OnATKUpPurchase += IncreaseAttack;
        ShopButton.OnHPUpPurchase += IncreaseHP;
        NewMapGenerator.OnGameRestarted += ResetPlayer;
        NewMapGenerator.OnGameStarted += SetPlayer;
    }

    private void Disable() 
    {
        GameInputController.OnEnterHazard -= HandleHazard;
        GameInputController.OnReachedEnd -= HandleEnd;  
        Item.OnCoinCollected -= IncreaseCoins;
        Item.OnPotionCollected -= IncreasePotions; 
        EnemyScript.OnEngageEnemy -= StartBattle;  
        UIController.OnFadeOutComplete -= ResetLevel;
        Player.OnPlayerDied -= ResetCoins;
        ShopButton.OnPotionPurchase -= PotionPurchase;
        ShopButton.OnATKUpPurchase -= IncreaseAttack;
        ShopButton.OnHPUpPurchase -= IncreaseHP;
        NewMapGenerator.OnGameRestarted -= ResetPlayer;
        NewMapGenerator.OnGameStarted -= SetPlayer;
    }

    private void Initialize() 
    {
        rankIndex = 0;
        lastPotionCount = 1;
        potionTotal = 1;
        currentRank = availableRanks[rankIndex];
        OnMinerRankIncrease?.Invoke();
        coinsCollectedThisLevel = 0;
        generator.GenerateMap(currentRank);
        mapCount = 1;
        depthCount = 1;
    }
    private void SetPlayer()
    {
        playerStart = generator.GetStartingPoint();
        player = Instantiate(GameAssets.i.player, playerStart.transform.position, Quaternion.identity);
        stats = new StatSystem(GameAssets.i.playerData.maxHP,GameAssets.i.playerData.atkPower); 
        healthSystem = new HealthSystem(GameAssets.i.playerData.maxHP);
        currentPotion = startingPotion;
        lastCollectedPotion = currentPotion;
        UIController.i.SetPotionButton(startingPotion);
        OnGameInitialized?.Invoke();
        SoundManager.PlayMusic(SoundManager.Music.mainGame);
        CloseUI();
        UnPauseGame();
    }

    public void NewGame()
    {
        PauseGame();
        Initialize(); 
        OpenUI();
    }

    public void NewGameFromWithinGame()
    {
        PauseGame();
        OpenUI();
        //OnGameResetting?.Invoke();
        coinTotal = 0;
        potionTotal = 1;
        lastPotionCount = 1;
        coinsCollectedThisLevel = 0;
        rankIndex = 0;
        mapCount = 1;
        depthCount = 1;
        currentRank = availableRanks[rankIndex];
        OnMinerRankIncrease?.Invoke();
        lastCollectedPotion = startingPotion;
        currentPotion = startingPotion;
        ResetLevel();
        stats = new StatSystem(GameAssets.i.playerData.maxHP,GameAssets.i.playerData.atkPower); 
        healthSystem = new HealthSystem(GameAssets.i.playerData.maxHP);
    }
    public void ResetLevel()
    {
        coinsCollectedThisLevel = 0;
        OnGameResetting?.Invoke();
        Destroy(player.gameObject);
        foreach(GameObject obj in levelObjects)
        {
            Destroy(obj);
        }
        generator.ClearRooms(currentRank);
    }

    private void ResetPlayer()
    {
        playerStart = generator.GetStartingPoint();
        player = Instantiate(GameAssets.i.player, playerStart.transform.position, Quaternion.identity);
        healthSystem.RestoreHealth((int)healthSystem.GetMaxHealth());
        if(mapCount == 1) currentPotion = lastCollectedPotion;
        UIController.i.SetPotionButton(currentPotion);
        OnGameInitialized?.Invoke();
        CloseUI();
    }

    public void UpdateLevelObjectsList(GameObject obj)
    {
        levelObjects.Add(obj);
    }
    private void HandleHazard()
    {
        ResetCoins();
        potionTotal = lastPotionCount;
        player.GetComponent<Player>().TakeDamage(1000, false);
    }

    private void HandleEnd()
    {
        PauseGame();
        SaveCoinTotal();
        mapCount++;
        depthCount++;
        if(mapCount > 1) lastCollectedPotion = currentPotion;
        lastPotionCount = potionTotal;
        if(mapCount > 3)
        {
            mapCount = 1;
            rankIndex++;
            if(rankIndex == 4) { OnGameOverWin?.Invoke(); return; }
            currentRank = availableRanks[rankIndex];
            OnMinerRankIncrease?.Invoke();
            UIController.i.OpenLevelEndMenu(true);
        }
        else UIController.i.OpenLevelEndMenu(false);
        
    }

    public void StartFadeIn()
    {
        UIController.i.FadeIn();
    }

    public void RestartAfterDeath()
    {
        UIController.i.FadeOut();
        UIController.i.CloseDeathMenu();
    }

    public void RestartAfterComplete()
    {
        UIController.i.FadeOut();
        UIController.i.CloseShopMenu();
    }

    //Audio controls
    public void SetMasterVolume(float volume)
    {
        AudioSettingsManager.SetMasterVolume(volume);
    }
    public void SetMusicVolume(float volume)
    {
        AudioSettingsManager.SetMusicVolume(volume);
    }
    public void SetSFXVolume(float volume)
    {
        AudioSettingsManager.SetSFXVolume(volume);
    }

    public void MuteUnmuteAudio() 
    {
        if(audioMuted) 
        {
            audioMuted = false;
            AudioSettingsManager.UnMuteAudio();
        }
        else 
        {
            audioMuted = true;
            AudioSettingsManager.MuteAudio();
        }
    }

    //Set functions
    public void OpenUI() { uiOpen = true; }
    public void CloseUI() { uiOpen = false; }
    public void PauseGame() { paused = true; }
    public void UnPauseGame() { paused = false; }
    public void IncreaseCoins(int amount) 
    { 
        coinsCollectedThisLevel += amount; 
        coinTotal += amount;
        SoundManager.PlaySound(SoundManager.Sound.collectCoin);
        if (amount > 1) SystemMessage.Create(GameAssets.i.playerData.playerName + " collected " + amount.ToString() + " coins");
        else  SystemMessage.Create(GameAssets.i.playerData.playerName + " collected " + amount.ToString() + " coin");
        OnCoinCollected?.Invoke();
    }

    public void IncreasePotions(ItemData item)
    { 
        if(item == currentPotion)
        {
            if(potionTotal <= 2) 
            { 
                potionTotal++; 
                SoundManager.PlaySound(SoundManager.Sound.collectPotion);
                SystemMessage.Create(item.itemName + " Collected");
                PotionSuccessfulCollect?.Invoke(item); 
            } 
            else SystemMessage.Create("Potions Full");
        }
        else
        {
            potionTotal = 1;
            lastCollectedPotion = currentPotion;
            currentPotion = item;
            SoundManager.PlaySound(SoundManager.Sound.collectPotion);
            SystemMessage.Create(item.itemName + " Collected");
            PotionSuccessfulCollect?.Invoke(item);
        }
    }

    public void ResetPotions()
    {
        potionTotal = 0;
    }

    private void PotionPurchase(ShopItemData item)
    {
        SoundManager.PlaySound(SoundManager.Sound.collectPotion);
        potionTotal++;
        PotionSuccessfulPurchase?.Invoke(item.data);
        lastPotionCount = potionTotal;
    }

    public bool DecreaseCoins(int amount)
    { 
        if(coinTotal - amount >= 0) 
        {
            coinTotal -= amount;
            OnCoinsDecrease?.Invoke();
            return true;
        }
        else return false;
    }

    private void ResetCoins()
    {
        coinTotal -= coinsCollectedThisLevel;
        coinsCollectedThisLevel = 0;
    }

    private void IncreaseHP(int value)
    {
        Debug.Log(healthSystem.GetMaxHealth());
        healthSystem.IncreaseMaxHealth(value);
        Debug.Log(healthSystem.GetMaxHealth());
        OnHealthIncrease?.Invoke();
    }
    private void IncreaseAttack(int value)
    {
        stats.IncreaseAtkPower(value);
        OnAttackIncrease?.Invoke();
    }

    public void SaveCoinTotal()
    {
        
    }

    public void UsePotion()
    {
        if(!GetComponent<BattleManager>().GetBattleStatus()) SystemMessage.Create("In Battle");
        else if (potionTotal == 0) SystemMessage.Create("Out of Potions");
        else if (healthSystem.GetCurrentHealth() < healthSystem.GetMaxHealth()) 
        {
                if(DecreasePotions()) 
                {
                    healthSystem.RestoreHealth(UIController.i.GetPotionHealthValue());
                    SystemMessage.Create(UIController.i.GetPotionHealthValue() + " HP Restored");
                    OnPotionUsed?.Invoke();
                }
                
        }
        else SystemMessage.Create("Health Full");
        
    }

    private bool DecreasePotions()
    { 
        if(potionTotal - 1 >= 0) 
        {
            potionTotal--;
            return true;
        }
        else return false;
    }


    private void StartBattle(GameObject enemy)
    {   
        OnBattleStart?.Invoke(enemy, player);
        PauseGame();
    }

    //set functions

    public void UpdateCurrentPotion(ItemData item){ currentPotion = item;}
    public void SetIsInShop(bool answer)
    {
        isInShop = answer;
    }
    //Get functions
    public Transform GetSysMessagePoint()
    { 
        if(isInShop) return shopMessagePoint;
        else return sysMessagePoint; 
    }
    public bool GetIsShop(){ return isInShop; }
    public GameObject GetPlayer(){ return player; }
    public Player GetPlayerScript(){ return player.GetComponent<Player>(); }
    public float GetPlayerMoveSpeed(){ return playerMoveSpeed; }
    public bool GetPausedStatus(){ return paused; }
    public int GetCoinTotal(){ return coinTotal; }
    public int GetPotionTotal() { return potionTotal; }
    public GameInputController GetGameInputController(){ return GetComponent<GameInputController>(); }
    public MinerRank GetCurrentRank(){ return currentRank; }
    public HealthSystem GetHealthSystem(){ return healthSystem; }
    public StatSystem GetStats(){ return stats; }
    public NewMapGenerator GetMapGenerator(){ return generator; }
    public ItemData GetCurrentPotion(){ return currentPotion; }
    public bool GetUIOpen(){ return uiOpen; }
    public int GetMapCount(){ return mapCount; }
    public int GetDepthCount(){ return depthCount;}


}
