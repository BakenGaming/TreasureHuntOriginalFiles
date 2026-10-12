using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAttackDirection
{
    up, down, left, right
}
public class BattleManager : MonoBehaviour
{
    private bool playerReset = false, playerTurn, battling, battleOver = true, playerMoveBack = false;
    private GameObject player, enemy;
    private Player playerScript;
    private EnemyScript enemyScript;
    private Vector3 lastKnownPosition, battlePosition;
    private PlayerAttackDirection atkDir;
    private void OnEnable() 
    {
        GameManager.OnBattleStart += Battle; 
        Player.OnPlayerDied += EndBattleOnPlayerDeath;   
        EnemyScript.OnEnemyDied += EndBattle;
    }

    private void OnDisable() 
    {
        GameManager.OnBattleStart -= Battle;   
        Player.OnPlayerDied -= EndBattleOnPlayerDeath;   
        EnemyScript.OnEnemyDied -= EndBattle; 

    }

    private void Update() 
    {
        if(battleOver) return;
        if(playerReset)
        {
            player.transform.position = 
                Vector3.Lerp(player.transform.position, lastKnownPosition,
                GameManager.i.GetPlayerMoveSpeed() * Time.deltaTime);
  
            if(Vector2.Distance(GameManager.i.GetPlayer().transform.position, lastKnownPosition) <= .01f)
            {
                GameManager.i.GetPlayer().transform.position = lastKnownPosition;
                playerReset = false;
                CommenseBattle();
            }
        }  

        if(battling)
        {
            if(playerTurn && !playerMoveBack) 
            {
                playerScript.Attack(enemyScript);
                battling = false;
                StartCoroutine(AttackDelay());
            }
            if(!playerTurn && !playerMoveBack) 
            {
                enemyScript.Attack(playerScript);
                battling = false;
                StartCoroutine(AttackDelay());
            }
        }

        if(playerMoveBack)
        {
            player.transform.position = 
                Vector3.Lerp(player.transform.position, battlePosition,
                GameManager.i.GetPlayerMoveSpeed() * Time.deltaTime);
  
            if(Vector2.Distance(battlePosition, GameManager.i.GetPlayer().transform.position) <= .01f)
            {
                GameManager.i.GetPlayer().transform.position = battlePosition;
                playerMoveBack = false;
                battleOver = true;
                GameManager.i.UnPauseGame();
            }    
        }
      }

    private void Battle(GameObject enemy, GameObject player)
    {
        playerMoveBack = false;
        lastKnownPosition = GameManager.i.GetGameInputController().GetLastKnownPosition();
        battlePosition = enemy.transform.position;
        this.player = player;
        this.enemy = enemy;
        playerScript = player.GetComponent<Player>();;
        enemyScript = enemy.GetComponent<EnemyScript>();
        if(enemyScript.GetEnemyData().isFirstStrike)
        {
            playerTurn = true;
            SystemMessage.Create(enemyScript.GetEnemyData().enemyName + " gets the jump on " + playerScript.GetPlayerData().playerName);
        }
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        Instantiate(GameAssets.i.pfBattleText, player.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        playerReset = true;
        battleOver = false;
    }

    private void CommenseBattle()
    {
        GetAttackDirection();
        switch(atkDir)
        {
            case PlayerAttackDirection.up:
                break;
            case PlayerAttackDirection.down:
                break;
            case PlayerAttackDirection.left:
                break;
            case PlayerAttackDirection.right:
                break;                                            
        }
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(1f);
        if(!battleOver) 
        {
            playerTurn = !playerTurn;
            battling = true;
        }
    }

    private void EndBattleOnPlayerDeath()
    {
        StopAllCoroutines();
        battleOver = true;
        playerTurn = false;
        battling = false;
        playerMoveBack = true;
    }
    
    private void EndBattle()
    {
        StopAllCoroutines();
        playerTurn = false;
        battling = false;
        playerMoveBack = true;
    }

    private void GetAttackDirection()
    {
        if(player.transform.position.x == enemy.transform.position.x) //attack up or down
        {
            if(player.transform.position.y - enemy.transform.position.y == 1) atkDir = PlayerAttackDirection.down;
            else atkDir = PlayerAttackDirection.up;
        }
        else if (player.transform.position.y == enemy.transform.position.y) //attack left or right
        {
            if(player.transform.position.x - enemy.transform.position.x == 1) atkDir = PlayerAttackDirection.left;
            else atkDir = PlayerAttackDirection.right;            
        }

    }

    public bool GetBattleStatus(){ return battleOver; }
}
