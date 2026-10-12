using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite footSteps, attack, dig, normalArrow;

    private SpriteRenderer mouseImage;
    private Animator anim;
    private Camera mainCamera;

    private void Start() 
    {
        //Cursor.visible = false;
        mouseImage = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();  
        mainCamera = Camera.main;  
    }

    private void Update() 
    {
        transform.position = UtilsClass.GetMouseWorldPosition();

        if(!GameManager.i.GetUIOpen()) 
        {
            if(GameManager.i.GetPlayer() == null) return;
            else UpdateMouse();
        }
        else
        {
            mouseImage.sprite = null; 
            Cursor.visible = true;
        } 
    }

    public void UpdateMouse()
    {
        if(GameManager.i.GetUIOpen()) 
        {
            Cursor.visible = true;
            return;
        }
        else
        {
            Cursor.visible = false;
        }
        var ray = UtilsClass.GetMouseWorldPosition();
        Collider2D collider = Physics2D.OverlapPoint(ray);
        
        if(collider != null) 
        {
            if(Vector2.Distance(GameManager.i.GetPlayer().transform.position,collider.transform.position) <= 1) { anim.SetBool("isInvalid", false);}
            else { anim.SetBool("isInvalid", true); return; }
        }

        if (Physics2D.OverlapPoint(ray, StaticVariables.i.GetPlayer())) { Cursor.visible = false; SetIdle();  anim.SetBool("isInvalid", false); return; }
        else if (Physics2D.OverlapPoint(ray, StaticVariables.i.GetCollectable())) { Cursor.visible = false; SetWalk();  anim.SetBool("isInvalid", false); return; }
        else if (Physics2D.OverlapPoint(ray, StaticVariables.i.GetEnemy())) { Cursor.visible = false; SetAttack();  anim.SetBool("isInvalid", false); return; }
        else if (Physics2D.OverlapPoint(ray, StaticVariables.i.GetWallLayer())) { Cursor.visible = false; SetIdle(); anim.SetBool("isInvalid", true); return; }        
        else if (Physics2D.OverlapPoint(ray, StaticVariables.i.GetGroundLayer())) 
        { 
            Cursor.visible = false;
            anim.SetBool("isInvalid", false);
            MapPoint point = collider.GetComponent<MapPoint>();
            if(!point.GetBrokenStatus())
                SetBreak(); 
            else if (point.GetBrokenStatus() && point.GetIsEnemy())
                SetAttack();
            else if (point.GetBrokenStatus() && point.GetIsCoin())
                SetWalk();
            else if (point.GetBrokenStatus() && point.GetIsPotion())
                SetWalk();
            else if (point.GetBrokenStatus() && point.GetIsHazard())
                SetWalk();
            else if (point.GetBrokenStatus() && point.GetIsStart())
                SetWalk();
            else if (point.GetBrokenStatus() && point.GetIsTreasure())
                SetWalk();
            else SetWalk(); 
            return; 
        } 
        else
        {
            mouseImage.sprite = null; 
            Cursor.visible = true;
        }
   }

   private void SetIdle()
   {
        anim.SetBool("isAttack", false);
        anim.SetBool("isWalk", false);
        anim.SetBool("isBreak", false);
   }
    private void SetWalk()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isWalk", true);
        anim.SetBool("isBreak", false);
     }

    private void SetBreak()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isWalk", false);
        anim.SetBool("isBreak", true);
     }

    private void SetAttack()
    {
        anim.SetBool("isAttack", true);
        anim.SetBool("isWalk", false);
        anim.SetBool("isBreak", false);
    }
}
