using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    public static event Action OnFadeOutComplete;
    private static UIController _i;

    public static UIController i
    { 
      get
        {
            return _i;
        }  
    }
    [SerializeField] private float fadeSpeed;
    [SerializeField] private Image fadeScreen;
    [SerializeField] private Button potionButton;
    [SerializeField] private TextMeshProUGUI coinsText, attackText, healthText, minerRankText, depthCount;
    [SerializeField] private GameObject settingMenu, deathMenu, shopMenu, levelCompleteMenu, 
        mainMenu, newMainMenu, fadeText, warningMenu, fadeBar, statusEffect, introMenu, creditsMenu;

    private int poisonCounter;
    private bool fadeFromBlack, fadeToBlack;
    private void Awake() 
    {
        _i = this;    
    }

    private void OnEnable() 
    {
        GameManager.OnCoinCollected += UpdateCoins;
        GameManager.PotionSuccessfulCollect += CollectPotions; 
        GameManager.PotionSuccessfulPurchase += CollectPotions;
        GameManager.OnPotionUsed += UpdatePotions; 
        GameManager.OnPotionUsed += UpdateHealth;
        GameManager.OnAttackIncrease += UpdateAttackPower;
        GameManager.OnHealthIncrease += UpdateHealth;
        GameManager.OnMinerRankIncrease += UpdateMinerRank;
        GameManager.OnGameOverWin += GameOverWin;
        Player.OnDamageTaken += UpdateHealth;
        Player.OnPlayerInitialized += Initialize;
        Player.OnPlayerDied += DisplayDeathMenu;
        Player.OnPlayerPoisoned += ActivatePoison;
        GameInputController.OnPoisonDamageReceived += PoisonTick;
        GameIntroController.OnTutorialOver += CloseIntro;
    }

    private void OnDisable()
    {
        GameManager.OnCoinCollected -= UpdateCoins;
        GameManager.PotionSuccessfulCollect -= CollectPotions;
        GameManager.PotionSuccessfulPurchase -= CollectPotions;
        GameManager.OnPotionUsed -= UpdatePotions;
        GameManager.OnPotionUsed -= UpdateHealth;
        GameManager.OnAttackIncrease -= UpdateAttackPower;
        GameManager.OnHealthIncrease -= UpdateHealth;
        GameManager.OnMinerRankIncrease -= UpdateMinerRank;
        GameManager.OnGameOverWin += GameOverWin;
        Player.OnDamageTaken -= UpdateHealth;
        Player.OnPlayerInitialized -= Initialize;
        Player.OnPlayerDied -= DisplayDeathMenu;
        Player.OnPlayerPoisoned -= ActivatePoison;
        GameInputController.OnPoisonDamageReceived -= PoisonTick;
        GameIntroController.OnTutorialOver -= CloseIntro;
    }

    private void Initialize() 
    {
        UpdateHealth();
        UpdatePotions();
        UpdateCoins();
        UpdateAttackPower();
        UpdateMinerRank();
        poisonCounter = 0;
    }

    void Update()
    {
        if (fadeFromBlack)
        {
            fadeText.SetActive(false);
            fadeBar.SetActive(false);
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 0)
            {
                fadeFromBlack = false;
            }
        }

        if (fadeToBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 1)
            {
                fadeToBlack = false;
                fadeText.SetActive(true);
                fadeBar.SetActive(true);
                fadeBar.GetComponentInParent<FadeScreenController>().Initialize();
                OnFadeOutComplete?.Invoke();
            }
        }

        if(poisonCounter > 0) statusEffect.SetActive(true);
        else statusEffect.SetActive(false);
        
    }

    public void NewGame()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        mainMenu.SetActive(false);
        introMenu.SetActive(true);
        GameManager.i.OpenUI();
        GameManager.i.SetIsInShop(false);
    }

    public void NewGameFromWithinNewGame()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.NewGameFromWithinGame();
        newMainMenu.SetActive(false);
        GameManager.i.SetIsInShop(false);
    }

    public void OpenMainMenu()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.OpenUI();
        SoundManager.PlayMusic(SoundManager.Music.mainMenu);
        fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1);
        fadeText.SetActive(true);
        fadeBar.SetActive(true);
        newMainMenu.SetActive(true);
        warningMenu.SetActive(false);
        deathMenu.SetActive(false);
        settingMenu.SetActive(false);
        levelCompleteMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void GameOverWin()
    {
        newMainMenu.SetActive(false);
        warningMenu.SetActive(false);
        deathMenu.SetActive(false);
        settingMenu.SetActive(false);
        levelCompleteMenu.SetActive(false);
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void OpenWarningMenu()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.OpenUI();
        warningMenu.SetActive(true);
    }

    public void CloseWarningMenu()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.CloseUI();
        warningMenu.SetActive(false);
        GameManager.i.SetIsInShop(false);
    }

    public void OpenNewMainMenu()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.OpenUI();
        SoundManager.PlayMusic(SoundManager.Music.mainMenu);
        fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1);
        fadeText.SetActive(true);
        fadeBar.SetActive(true);
        newMainMenu.SetActive(true);
        warningMenu.SetActive(false);
        deathMenu.SetActive(false);
        settingMenu.SetActive(false);
        levelCompleteMenu.SetActive(false);
        creditsMenu.SetActive(false);
        fadeScreen.GetComponent<FadeScreenController>().Initialize();
    }

    public void OpenSettingsMenu()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.OpenUI();
        GameManager.i.PauseGame();
        settingMenu.SetActive(true);
    }

    public void CloseSettingMenu()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.UnPauseGame();
        GameManager.i.CloseUI();
        settingMenu.SetActive(false);
        GameManager.i.SetIsInShop(false);
    }

    public void DisplayDeathMenu()
    {
        GameManager.i.OpenUI();
        GameManager.i.PauseGame();
        deathMenu.SetActive(true);
    }

    public void OpenLevelEndMenu(bool rankUp)
    {
        GameManager.i.OpenUI();
        GameManager.i.PauseGame();
        shopMenu.SetActive(false);
        deathMenu.SetActive(false);
        levelCompleteMenu.SetActive(true);
        UpdateDepthCount();
        if(rankUp) levelCompleteMenu.GetComponent<LevelCompleteMenu>().SetAdvText();
        else levelCompleteMenu.GetComponent<LevelCompleteMenu>().DisableAdvText();
        settingMenu.SetActive(false);
    }

    public void OpenShopMenu()
    {
        GameManager.i.OpenUI();
        shopMenu.SetActive(true);
        shopMenu.GetComponent<ShopController>().ResetShop();
        deathMenu.SetActive(false);
        levelCompleteMenu.SetActive(false);
        settingMenu.SetActive(false);
    }

    public void CloseShopMenu()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.CloseUI();
        shopMenu.SetActive(false);
        deathMenu.SetActive(false);
        levelCompleteMenu.SetActive(false);
        settingMenu.SetActive(false);
        warningMenu.SetActive(false);
        GameManager.i.SetIsInShop(false);
    }

    public void CloseDeathMenu()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        GameManager.i.CloseUI();
        shopMenu.SetActive(false);
        deathMenu.SetActive(false);
        levelCompleteMenu.SetActive(false);
        settingMenu.SetActive(false);
        GameManager.i.SetIsInShop(false);
    }
    private void CollectPotions(ItemData item) 
    { 
        potionButton.GetComponent<ButtonManager>().SetItemData(item);
        potionButton.GetComponent<ButtonManager>().UpdatePotionTotal(GameManager.i.GetPotionTotal()); 
    }

    private void UpdatePotions(){ potionButton.GetComponent<ButtonManager>().UpdatePotionTotal(GameManager.i.GetPotionTotal());}

    public void CloseIntro() { introMenu.SetActive(false); GameManager.i.CloseUI(); GameManager.i.NewGame();}

    private void ActivatePoison() { poisonCounter = 3; }
    private void PoisonTick(){ poisonCounter--; }
    private void UpdateHealth() { healthText.text = GameManager.i.GetHealthSystem().GetCurrentHealth().ToString(); }
    private void UpdateCoins() { coinsText.text = GameManager.i.GetCoinTotal().ToString(); }
    private void UpdateAttackPower() { attackText.text = GameManager.i.GetStats().GetAttackPower().ToString(); }
    public void UpdateMinerRank(){ minerRankText.text = GameManager.i.GetCurrentRank().rankName; }
    public void UpdateDepthCount(){depthCount.text = GameManager.i.GetDepthCount().ToString();}
    public void FadeIn(){ fadeFromBlack = true; fadeToBlack = false; }
    public void FadeOut(){ fadeFromBlack = false; fadeToBlack = true; }
    public int GetPotionHealthValue(){ return potionButton.GetComponent<ButtonManager>().GetItemData().value; }
    public void SetPotionButton(ItemData item) { potionButton.GetComponent<ButtonManager>().SetItemData(item); }
    public ItemData GetPotionButtonData(){ return potionButton.GetComponent<ButtonManager>().GetItemData(); }
}
