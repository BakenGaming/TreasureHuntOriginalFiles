using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CodeMonkey.Utils;
using System;

public class GameInputController : MonoBehaviour
{
    public static event Action OnEnterHazard;
    public static event Action OnReachedEnd;
    public static event Action OnPoisonDamageReceived;
    private Vector3 mousePosition;
    private Vector2 boxSize = new Vector2(.1f,.1f);
    private MapPoint currentMapPoint;
    private InputController input;
    private InputAction breakBlock, moveToPoint;
    private Vector3 lastKnownPosition;
    private bool movePlayer = false;

    private void Awake() 
    {
        input = new InputController();    
    }
    private void OnEnable() 
    {
        breakBlock = input.Main.LMB;
        breakBlock.performed += BreakBlock;
        
        moveToPoint = input.Main.RMB;
        moveToPoint.performed += MoveToPoint;

        GameManager.OnGameInitialized += Initialize;
        GameManager.OnGameResetting += HoldForReset;
        EnemyScript.OnEngageEnemy += StopMoving;    
    }

    private void OnDisable() 
    {
        breakBlock.Disable();
        moveToPoint.Disable(); 

        EnemyScript.OnEngageEnemy  -= StopMoving;
        GameManager.OnGameResetting -= HoldForReset;
        GameManager.OnGameInitialized -= Initialize;    
    }

    private void Initialize() 
    {
        breakBlock.Enable();
        moveToPoint.Enable(); 
    }

    public void HoldForReset()
    {
        breakBlock.Disable();
        moveToPoint.Disable(); 
    }

    private void Update() 
    {
        if(GameManager.i.GetPausedStatus()) return;
        if(movePlayer)
        {
            SoundManager.PlaySound(SoundManager.Sound.footSteps);
            GameManager.i.GetPlayer().transform.position = 
                Vector3.Lerp(GameManager.i.GetPlayer().transform.position, currentMapPoint.transform.position,
                GameManager.i.GetPlayerMoveSpeed() * Time.deltaTime);
            if(Vector2.Distance(GameManager.i.GetPlayer().transform.position,currentMapPoint.transform.position) <= .01f)
            {
                GameManager.i.GetPlayer().transform.position = currentMapPoint.transform.position;
                movePlayer = false;
                CheckTileType();
            }
        }
        else
        {
            mousePosition = UtilsClass.GetMouseWorldPosition();
            Collider2D collider  = Physics2D.OverlapBox(mousePosition, boxSize, 0, StaticVariables.i.GetGroundLayer());
            if(collider)
            {
                if(currentMapPoint != null)
                {
                    if(collider.gameObject.GetComponent<MapPoint>() != currentMapPoint)
                    {
                        currentMapPoint.UnHighlight();    
                        currentMapPoint = collider.gameObject.GetComponent<MapPoint>();
                        currentMapPoint.HighlightPoint();
                    }
                } 
                else
                {
                    currentMapPoint = collider.gameObject.GetComponent<MapPoint>();
                    currentMapPoint.HighlightPoint();
                }
            }
            else 
            {
                if (currentMapPoint != null) 
                {
                    currentMapPoint.UnHighlight();
                    currentMapPoint = null;
                }
                
            }
        }
        
    }

    private void CheckTileType()
    {
        Collider2D collider = Physics2D.OverlapBox(GameManager.i.GetPlayer().transform.position, boxSize, 0, StaticVariables.i.GetGroundLayer());
        MapPoint currentPoint = collider.gameObject.GetComponent<MapPoint>();
        
        if(currentPoint.GetIsHazard()) 
        {
            SoundManager.PlaySound(SoundManager.Sound.death);
            OnEnterHazard?.Invoke();
        }
        if(currentPoint.GetIsTreasure()) OnReachedEnd?.Invoke();
    }
    private void BreakBlock(InputAction.CallbackContext obj)
    {
        if(GameManager.i.GetPausedStatus()) {currentMapPoint = null; return; }
        else if(GameManager.i.GetUIOpen()) {currentMapPoint = null; return; }
        else if (currentMapPoint == null) return;
        else currentMapPoint.BreakPoint();
    }

    private void MoveToPoint(InputAction.CallbackContext obj)
    {
        if(GameManager.i.GetPausedStatus()) return;
        if(Vector2.Distance(GameManager.i.GetPlayer().transform.position,currentMapPoint.transform.position) <= 1)
        {
            if(currentMapPoint.GetBrokenStatus()) 
            {
                if(GameManager.i.GetPlayer().GetComponent<Player>().GetPoisonStatus()) 
                {
                    GameManager.i.GetPlayer().GetComponent<Player>().TakePoisonDamage(2);
                    DamagePopup.Create(GameManager.i.GetPlayer().transform.position, 2, true, true);
                    OnPoisonDamageReceived?.Invoke();
                }
                if(!movePlayer) lastKnownPosition = GameManager.i.GetPlayer().transform.position;
                movePlayer = true;
            }
            else SystemMessage.Create("You Can't Move Here");
        }
    }

    private void StopMoving(GameObject unused){ GameManager.i.GetPlayer().transform.position = currentMapPoint.transform.position; movePlayer = false; }
    public Vector3 GetLastKnownPosition(){ return lastKnownPosition; }
}
