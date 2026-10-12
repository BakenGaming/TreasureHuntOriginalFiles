using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioSettingsManager
{
    private static float currentMasterVol, currentSFXVol, currentMusicVol;
    public static void SetMasterVolume(float volume)
    {
        StaticVariables.i.GetMusicMixer().audioMixer.SetFloat("MasterVolume", volume);
        currentMasterVol = volume;
    }

    public static void SetSFXVolume(float volume)
    {
        StaticVariables.i.GetSFXMixer().audioMixer.SetFloat("SFXVolume", volume);
        currentSFXVol = volume;
    }

    public static void SetMusicVolume(float volume)
    {
        StaticVariables.i.GetMusicMixer().audioMixer.SetFloat("MusicVolume", volume);
        currentMusicVol = volume;
    }

    public static void MuteAudio()
    {
        StaticVariables.i.GetMusicMixer().audioMixer.SetFloat("MusicVolume", -80f);
        StaticVariables.i.GetSFXMixer().audioMixer.SetFloat("SFXVolume", -80f);
        StaticVariables.i.GetMusicMixer().audioMixer.SetFloat("MasterVolume", -80f);
    }

    public static void UnMuteAudio()
    {
        StaticVariables.i.GetMusicMixer().audioMixer.SetFloat("MusicVolume", currentMusicVol);
        StaticVariables.i.GetSFXMixer().audioMixer.SetFloat("SFXVolume", currentSFXVol);
        StaticVariables.i.GetMusicMixer().audioMixer.SetFloat("MasterVolume", currentMasterVol);
    }
}
