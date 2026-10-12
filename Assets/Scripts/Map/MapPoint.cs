using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapPoint : MonoBehaviour
{
    public static event Action<string> OnRoomSet;
    public static event Action<string> OnEmptyReplaced;
    
    private bool isBroken, isTrap, isEmpty, isCoin, isStart, isTreasure, isEnemy, isPotion, 
        canBeBroken = true;
    private Sprite brokenSprite;
    private SpriteRenderer sr, outline;
    private int[] lastFourRooms = new int[4];
    private int lastSetRoom, meyc, mec, mpc, mcc, mtc;
    private bool eycFull, ecFull, ccFull, pcFull, tcFull;
    private bool fillEYC, fillEC, fillCC, fillPC, fillTC, roomSet;
    private NewMapGenerator generator;

    private void Awake() 
    {
        sr = GetComponent<SpriteRenderer>();
        outline = transform.Find("highlight").GetComponent<SpriteRenderer>(); 
        outline.gameObject.SetActive(false);  
    }
    public void Clear() 
    {
        isBroken = false;
        isStart = false;
        sr.sprite = null;
        canBeBroken = true;
        brokenSprite = null;
        isTrap = false;
        isEmpty = false;
        isCoin = false;
        isTreasure = false;
        isPotion = false;
        sr.sprite = null;
    }
    public void SetStart()
    {
        isBroken = true;
        isStart = true;
        sr.sprite = GameAssets.i.start;
        canBeBroken = false;
     }

    public void SetEndPoint()
    {
        isBroken = false;
        isTreasure = true;
        brokenSprite = GameAssets.i.treasure;
    }

    public void SetCoin() { isCoin = true; brokenSprite = GameAssets.i.emptyGround[UnityEngine.Random.Range(0, GameAssets.i.emptyGround.Length)]; SetUnbroken(); }
    public void SetEnemy(){ isEnemy = true; brokenSprite = GameAssets.i.emptyGround[UnityEngine.Random.Range(0, GameAssets.i.emptyGround.Length)]; SetUnbroken();}
    public void SetTrap(){ isTrap = true; brokenSprite = GameAssets.i.hazardSprite[UnityEngine.Random.Range(0, GameAssets.i.hazardSprite.Length)]; SetUnbroken();}
    public void SetPotion(){ isPotion = true; brokenSprite = GameAssets.i.emptyGround[UnityEngine.Random.Range(0, GameAssets.i.emptyGround.Length)]; SetUnbroken();}
    public void SetEmpty(){ isEmpty = true; brokenSprite = GameAssets.i.emptyGround[UnityEngine.Random.Range(0, GameAssets.i.emptyGround.Length)]; SetUnbroken();}

    private void SetUnbroken(){ sr.sprite = GameAssets.i.unBroken; isBroken = false; }

    public void HighlightPoint()
    {
        if(GameManager.i.GetPlayer().transform.position == transform.position) return;
        if(Vector2.Distance(GameManager.i.GetPlayer().transform.position,transform.position) <= 1)
        {
            outline.gameObject.SetActive(true);     
        }
    }

    public void UnHighlight()
    {
        outline.gameObject.SetActive(false);
    }

    public void BreakPoint()
    {
        if(!canBeBroken) return;
        else
        {
            if(Vector2.Distance(GameManager.i.GetPlayer().transform.position,transform.position) <= 1)
            {
                SoundManager.PlaySound(SoundManager.Sound.hitBlock);
                canBeBroken = false;
                isBroken = true;
                sr.sprite = brokenSprite;
                Instantiate(GameAssets.i.pfBlockBreak, transform.position, Quaternion.identity);
                SoundManager.PlaySound(SoundManager.Sound.breakBlock);
                InstantiateObjects();
            }
        }
    }

    public void TESTBreakPoint()
    {
        if(!canBeBroken) return;
        canBeBroken = false;
        isBroken = true;
        sr.sprite = brokenSprite;
        InstantiateObjects();
    }

    public bool GetBrokenStatus() { return isBroken; }

    public void SetMainPath()
    {
        SetEmpty();
        if(!isStart) sr.sprite = GameAssets.i.unBroken;
        else sr.sprite = brokenSprite;
    }


    public void SetMax(int eyc, int cc, int ec, int tc, int pc, NewMapGenerator gen)
    {
        meyc = eyc;
        mec = ec;
        mpc = pc;
        mtc = tc;
        mcc = cc;
        generator = gen;
    }
    public bool SetRoom(int eyc, int cc, int ec, int tc, int pc)
    {
        roomSet = false;
        int newRoomType;
        if(eyc > meyc) newRoomType = UnityEngine.Random.Range(0,4);
        else newRoomType = UnityEngine.Random.Range(0,5);
        switch (newRoomType)
        {
            case 0:            
            if(ec < mec)
            {
                SetEnemy();
                isEmpty = false;
                isPotion = false;
                isCoin = false;
                isEnemy = true;
                isTrap = false;
                OnRoomSet?.Invoke("enemy");
                roomSet = true;
            }
            else roomSet = false;
            break;
        
            case 1:
            if(tc < mtc)
            {
                SetTrap();
                isEmpty = false;
                isPotion = false;
                isCoin = false;
                isEnemy = false;
                isTrap = true;
                OnRoomSet?.Invoke("trap");
                roomSet = true;
            }
            else roomSet = false;
            break;
        
            case 2:
            if(cc < mcc)
            {
                SetCoin();
                isEmpty = false;
                isPotion = false;
                isCoin = true;
                isEnemy = false;
                isTrap = false;
                OnRoomSet?.Invoke("coin");
                roomSet = true;
            }
            else roomSet = false;
            break;

            case 3:
            if(pc < mpc)
            {
                SetPotion();
                isEmpty = false;
                isPotion = true;
                isCoin = false;
                isEnemy = false;
                isTrap = false;
                roomSet = true;
                OnRoomSet?.Invoke("potion");
            }
            else roomSet = false;
            break;

            case 4:
            if(eyc < meyc)
            {
                SetEmpty();
                isEmpty = true;
                isPotion = false;
                isCoin = false;
                isEnemy = false;
                isTrap = false;
                roomSet = true;
                OnRoomSet?.Invoke("empty");
            }
            else roomSet = false;
            break;
        }
        return roomSet;
    }

    public bool SetNonHazardRoom(int eyc, int cc, int ec, int pc)
    {
        roomSet = false;
        int newRoomType;
        if(eyc > meyc) newRoomType = UnityEngine.Random.Range(0,2);
        else newRoomType = UnityEngine.Random.Range(0,3);
        
        switch(newRoomType)
        {
            case 0:
                if(cc < mcc)
                {       
                    SetCoin();
                    isEmpty = false;
                    isPotion = false;
                    isCoin = true;
                    isEnemy = false;
                    OnEmptyReplaced?.Invoke("coin");
                    roomSet = true;
                }
                break;
            case 1:
                if(ec < mec)
                {      
                    SetEnemy();
                    isEmpty = false;
                    isPotion = false;
                    isCoin = false;
                    isEnemy = true;
                    OnEmptyReplaced?.Invoke("enemy");
                    roomSet = true;
                }
                break;
            case 2:
                if(pc < mpc)
                {          
                    SetPotion();
                    isEmpty = false;
                    isPotion = true;
                    isCoin = false;
                    isEnemy = false;
                    OnEmptyReplaced?.Invoke("potion");
                    roomSet = true;
                }
                break;                                                            
            case 3:
                if(eyc < meyc)
                {      
                    SetEmpty();
                    isEmpty = true;
                    isPotion = false;
                    isCoin = false;
                    isEnemy = false;
                    roomSet = true;
                    OnEmptyReplaced?.Invoke("empty");
                }
                break; 
        }
        return roomSet;
    }

    private void InstantiateObjects()
    {
        GameObject obj;
        if(isCoin) 
        {
            obj = Instantiate(GameAssets.i.pfCoin, transform.position, Quaternion.identity);
            GameManager.i.UpdateLevelObjectsList(obj);
        }

        if(isPotion) 
        {
            switch(GameManager.i.GetCurrentRank().rankName)
            {
                case "Apprentice":
                    obj = Instantiate(GameAssets.i.pfPotion[0], transform.position, Quaternion.identity);
                    GameManager.i.UpdateLevelObjectsList(obj);
                    break;
                case "Novice":
                    obj = Instantiate(GameAssets.i.pfPotion[1], transform.position, Quaternion.identity);
                    GameManager.i.UpdateLevelObjectsList(obj);
                    break;;
                case "Intermediate":
                    obj = Instantiate(GameAssets.i.pfPotion[2], transform.position, Quaternion.identity);
                    GameManager.i.UpdateLevelObjectsList(obj);
                    break;;
                case "Expert":
                    obj = Instantiate(GameAssets.i.pfPotion[3], transform.position, Quaternion.identity);
                    GameManager.i.UpdateLevelObjectsList(obj);
                    break;;
            }


        }

        if(isEnemy) 
        {
            switch(GameManager.i.GetCurrentRank().rankName)
            {
                case "Apprentice":
                    if(GameManager.i.GetMapCount() < 2)
                    {
                        obj = Instantiate(GameAssets.i.pfEnemy[0], 
                            transform.position, Quaternion.identity);
                        GameManager.i.UpdateLevelObjectsList(obj);
                    }
                    else
                    {
                        obj = Instantiate(GameAssets.i.pfEnemy[UnityEngine.Random.Range(0,2)], 
                            transform.position, Quaternion.identity);
                        GameManager.i.UpdateLevelObjectsList(obj);
                    }
                    break;
                case "Novice":
                    obj = Instantiate(GameAssets.i.pfEnemy[UnityEngine.Random.Range(0,3)], 
                        transform.position, Quaternion.identity);
                    GameManager.i.UpdateLevelObjectsList(obj);                
                    break;
                case "Intermediate":
                    obj = Instantiate(GameAssets.i.pfEnemy[UnityEngine.Random.Range(1,4)], 
                        transform.position, Quaternion.identity);
                    GameManager.i.UpdateLevelObjectsList(obj);
                    break;
                case "Expert":
                    obj = Instantiate(GameAssets.i.pfEnemy[UnityEngine.Random.Range(2,5)], 
                        transform.position, Quaternion.identity);
                    GameManager.i.UpdateLevelObjectsList(obj);                
                    break;
            }

        }
        
        
    }

    public bool GetIsHazard() { return isTrap; }
    public bool GetIsTreasure() { return isTreasure; }
    public bool GetIsStart(){ return isStart; }
    public bool GetIsEnemy(){ return isEnemy; }
    public bool GetIsCoin(){ return isCoin; }
    public bool GetIsPotion(){ return isPotion; }


}
