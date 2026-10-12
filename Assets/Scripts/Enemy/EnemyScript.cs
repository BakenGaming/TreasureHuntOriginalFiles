using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyScript : MonoBehaviour, IEnemy
{
    public static event Action OnEnemyDied;
    public static event Action<GameObject> OnEngageEnemy;
    [SerializeField] private EnemyData enemyData;
    private Player player;
    private StatSystem stats;
    private HealthSystem healthSystem;
    private bool poisonMessage = false;

    private void Start() 
    {
        stats = new StatSystem(enemyData.maxHP, enemyData.atkPower);
        healthSystem = new HealthSystem(enemyData.maxHP);    
    }
    public void TriggerAttack()
    {
        OnEngageEnemy?.Invoke(this.gameObject);
    }

    public void Attack(Player player)
    {
        this.player = player;

        int block = UnityEngine.Random.Range(0, 101);
        if(block <= player.GetPlayerData().blockChance)
        {
            SystemMessage.Create(enemyData.enemyName + "'s attack is blocked by " + 
                player.GetPlayerData().playerName);
            DamagePopup.Create(player.transform.position, 0, false, false);
        }
        else
        {
                    SoundManager.PlaySound(SoundManager.Sound.hitPlayer);
                    Instantiate(GameAssets.i.pfPlayerHit, player.transform.position, Quaternion.identity);

                    int crit = UnityEngine.Random.Range(0, 101);
                    if(crit <= enemyData.critChance)
                    {
                        SystemMessage.Create(enemyData.enemyName + " attacks " + 
                            GameAssets.i.playerData.playerName + " for " + Mathf.RoundToInt((enemyData.atkPower * 1.5f)) + " damage");
                        if(enemyData.canPoison && !poisonMessage) 
                        {
                            SystemMessage.Create(GameAssets.i.playerData.playerName + " is Poisoned");                         
                            poisonMessage = true;
                        }  
                        DamagePopup.Create(player.transform.position, Mathf.RoundToInt((enemyData.atkPower * 1.5f)), true, false);
                        player.TakeDamage(Mathf.RoundToInt((enemyData.atkPower * 1.5f)), enemyData.canPoison);
                    }
                    else
                    {
                        SystemMessage.Create(enemyData.enemyName + " attacks " + 
                            player.GetPlayerData().playerName + " for " + enemyData.atkPower + " damage");
                        if(enemyData.canPoison  && !poisonMessage) 
                        {
                            SystemMessage.Create(GameAssets.i.playerData.playerName + " is Poisoned");
                            poisonMessage = true;
                        }  
                        DamagePopup.Create(player.transform.position, enemyData.atkPower, false, false);
                        player.TakeDamage(enemyData.atkPower, enemyData.canPoison);
                    }
        }
    }

    public void TakeDamage(int damage)
    {
        healthSystem.LoseHealth(damage);
        if(healthSystem.GetCurrentHealth() <= 0)
        {
            SystemMessage.Create(enemyData.enemyName + " has died");
            int rand = UnityEngine.Random.Range(enemyData.minCoinsDropped, enemyData.maxCoinsDropped);
            GameManager.i.IncreaseCoins(rand);
            PlayDeathSound();
            Destroy(gameObject);
            OnEnemyDied?.Invoke();
        }
    }

    private void PlayDeathSound()
    {
            switch(enemyData.classification)
            {
                case EnemyClassification.slime:
                    SoundManager.PlaySound(SoundManager.Sound.slimeDie);
                    break;
                case EnemyClassification.bat:
                    SoundManager.PlaySound(SoundManager.Sound.batDie);
                    break;
                case EnemyClassification.spider:
                    SoundManager.PlaySound(SoundManager.Sound.spiderDie);
                    break;
                case EnemyClassification.beetle:
                    SoundManager.PlaySound(SoundManager.Sound.beetleDie);
                    break;
                case EnemyClassification.snake:
                    SoundManager.PlaySound(SoundManager.Sound.snakeDie);
                    break;                                                                                
            }

    }
    public StatSystem GetStats(){ return stats; }
    public HealthSystem GetEnemyHealth(){ return healthSystem; }
    public EnemyData GetEnemyData(){ return enemyData; }
}
