using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class StaticVariables : MonoBehaviour
{
    public static StaticVariables i;
    [SerializeField] private LayerMask whatIsGround, whatIsWall, whatIsPlayer, whatIsEnemy, 
        collectable, UI;
    [SerializeField] private AudioMixerGroup masterMixer, sfxMixer, musicMixer;

    private void Awake() 
    {
        i = this;
    }

    public LayerMask GetGroundLayer() { return whatIsGround; }
    public LayerMask GetWallLayer() { return whatIsWall; }
    public LayerMask GetPlayer() { return whatIsPlayer; }
    public LayerMask GetEnemy() { return whatIsEnemy; }
    public LayerMask GetCollectable() { return collectable; }
    public LayerMask GetUI(){ return UI; }
    public AudioMixerGroup GetMasterMixer(){ return masterMixer; }
    public AudioMixerGroup GetSFXMixer(){ return sfxMixer; }
    public AudioMixerGroup GetMusicMixer(){ return musicMixer; }

}
