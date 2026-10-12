using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string levelSelectScene, mainMenuScene, gameControlsScene;
    [SerializeField] private Button firstSelected;

    private void Awake() { firstSelected.Select(); SoundManager.PlayMusic(SoundManager.Music.mainMenu); }
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        if(!PlayerPrefs.HasKey(gameControlsScene + "_unlocked"))
        {
            PlayerPrefs.SetInt(gameControlsScene + "_unlocked", 1);
        }
        SceneManager.LoadScene(gameControlsScene);
    } 

    public void LevelSelectMenu()
    {
        SceneManager.LoadScene(levelSelectScene);
    }

    public void LoadGame()
    {
        if(PlayerPrefs.HasKey(gameControlsScene + "_unlocked"))
        {
            SceneManager.LoadScene(levelSelectScene);
        }
        else
        {
            SystemMessage.Create("No Save Data Available...");
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void ExitGame()
    {
        Debug.Log("GAME EXIT");
        Application.Quit();
    }
}
