using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private string mainMenu, endGame, mainGame;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(mainGame);
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene(endGame);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(mainGame);
    }

    public Scene GetActiveScene()
    {
        return SceneManager.GetActiveScene();
    }
    public void ExitGame()
    {
        Debug.Log("GAME EXIT");
        Application.Quit();
    }
}
