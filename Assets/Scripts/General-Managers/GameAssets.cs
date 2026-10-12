using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    { 
      get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }  
    }

    private void Awake() 
    {
        _i = this;    
    }

    public Transform pfSysMessage, pfDamagePopup;
    public Sprite[] hazardSprite;
    public Sprite unBroken, treasure, start;
    public Sprite[] emptyGround;
    public GameObject player, pfCoin, mapPoint;
    public GameObject[] pfPotion;
    public GameObject[] pfEnemy;
    public ParticleSystem pfBlockBreak, pfBattleText, pfPlayerHit, pfSlimeHit, pfPlayerDie,
        pfBatHit, pfSpiderHit, pfSnakeHit, pfBeetleHit;
    public ShopItemData unavailableShopItem;
    public PlayerData playerData;
  
    public SoundAudioClip[] audioClipArray;
    public MusicAudioClip[] musicTrackArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
        [Range(0f, 1f)]
        public float volume;
    }

    [System.Serializable]
    public class MusicAudioClip
    {
        public SoundManager.Music music;
        public AudioClip audioClip;
        [Range(0f, 1F)]
        public float volume;
    }
}
