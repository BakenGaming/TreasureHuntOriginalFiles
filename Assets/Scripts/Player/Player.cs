using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    public static event Action OnPlayerDied;
    public static event Action OnDamageTaken;
    public static event Action OnPlayerInitialized;
    public static event Action OnPlayerPoisoned;

    private bool isPoisoned;
    private int poisonCounter;
    

    private void Initialize() 
    {
        OnPlayerInitialized?.Invoke();  
    }
    private void OnEnable() 
    {
        GameManager.OnGameInitialized += Initialize;
    }

    private void OnDisable() 
    {
        GameManager.OnGameInitialized -= Initialize;   
    }
    public void Attack(EnemyScript enemy)
    {
        int block = UnityEngine.Random.Range(0, 101);
        if(block <= enemy.GetEnemyData().blockChance)
        {
            SystemMessage.Create(GameAssets.i.playerData.playerName + "'s attack is blocked by " + 
                enemy.GetEnemyData().enemyName);
            DamagePopup.Create(enemy.transform.position, 0, false, false);
        }
        else
        {
            switch(enemy.GetEnemyData().classification)
            {
                case EnemyClassification.slime:
                    SoundManager.PlaySound(SoundManager.Sound.hitSlime);
                    Instantiate(GameAssets.i.pfSlimeHit, enemy.transform.position, Quaternion.identity);
                    break;
                case EnemyClassification.bat:
                    SoundManager.PlaySound(SoundManager.Sound.hitBat);
                    Instantiate(GameAssets.i.pfBatHit, enemy.transform.position, Quaternion.identity);
                    break;
                case EnemyClassification.spider:
                    SoundManager.PlaySound(SoundManager.Sound.hitSpider);
                    Instantiate(GameAssets.i.pfSpiderHit, enemy.transform.position, Quaternion.identity);
                    break;
                case EnemyClassification.beetle:
                    SoundManager.PlaySound(SoundManager.Sound.hitBeetle);
                    Instantiate(GameAssets.i.pfBeetleHit, enemy.transform.position, Quaternion.identity);
                    break;
                case EnemyClassification.snake:
                    SoundManager.PlaySound(SoundManager.Sound.hitSnake);
                    Instantiate(GameAssets.i.pfSnakeHit, enemy.transform.position, Quaternion.identity);
                    break;                                                                                
            }

            int crit = UnityEngine.Random.Range(0, 101);
            if(crit <= playerData.critChance)
            {
                SystemMessage.Create(GameAssets.i.playerData.playerName + " attacks " + 
                    enemy.GetEnemyData().enemyName + " for " + Mathf.RoundToInt((GameManager.i.GetStats().GetAttackPower() * 1.5f)) + " damage");

                DamagePopup.Create(enemy.transform.position, Mathf.RoundToInt((GameManager.i.GetStats().GetAttackPower() * 1.5f)), true, false);
                enemy.TakeDamage(Mathf.RoundToInt((GameManager.i.GetStats().GetAttackPower() * 1.5f)));
            }
            else
            {
                SystemMessage.Create(GameAssets.i.playerData.playerName + " attacks " + 
                    enemy.GetEnemyData().enemyName + " for " + (GameManager.i.GetStats().GetAttackPower()) + " damage");
                
                DamagePopup.Create(enemy.transform.position, GameManager.i.GetStats().GetAttackPower(), false, false);
                enemy.TakeDamage(GameManager.i.GetStats().GetAttackPower());
            }
        }
    }

    public void TakePoisonDamage(int damage)
    {
        GameManager.i.GetHealthSystem().LoseHealth(damage);
        OnDamageTaken?.Invoke();
        if(GameManager.i.GetHealthSystem().GetCurrentHealth() <= 0)
        {
            GameObject newObj = Instantiate(GameAssets.i.pfPlayerDie, transform.position, Quaternion.identity).gameObject;
            newObj.transform.SetParent(this.transform);
            GameManager.i.UpdateLevelObjectsList(newObj);
            SystemMessage.Create(GameAssets.i.playerData.playerName + " has died");
            OnPlayerDied?.Invoke();
        }
        poisonCounter--;
        if(poisonCounter == 0) UnPoison();
    }

    public void TakeDamage(int damage, bool poison)
    {
        if(poison && !isPoisoned)
        {
            isPoisoned = poison;
            poisonCounter = 3;
            OnPlayerPoisoned?.Invoke();
        }
        GameManager.i.GetHealthSystem().LoseHealth(damage);
        OnDamageTaken?.Invoke();
        if(GameManager.i.GetHealthSystem().GetCurrentHealth() <= 0)
        {
            SystemMessage.Create(GameAssets.i.playerData.playerName + " has died");
            OnPlayerDied?.Invoke();
            GameObject newObj = Instantiate(GameAssets.i.pfPlayerDie, transform.position, Quaternion.identity).gameObject;
            GameManager.i.UpdateLevelObjectsList(newObj);
        }
    }

    private void UnPoison(){ isPoisoned = false; }

    public PlayerData GetPlayerData(){ return playerData; }
    public bool GetPoisonStatus(){ return isPoisoned; }


}
