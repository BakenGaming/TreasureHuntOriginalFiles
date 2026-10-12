using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button[] mainMenuButtons;

    public void OpenSettingsMenu()
    {
        settingsPanel.SetActive(true);
        foreach (Button button in mainMenuButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void CloseSettingsMenu()
    {
        settingsPanel.SetActive(false);
        foreach (Button button in mainMenuButtons)
        {
            button.gameObject.SetActive(true);
        }
    }
}
