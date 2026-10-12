using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameIntroController : MonoBehaviour
{
    public static event Action OnTutorialOver;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject nextButton;
    
    private int panelIndex;

    private void Start() 
    {
        nextButton.SetActive(false);
        StartCoroutine(NextDelay());
    }
    public void NextPanel()
    {
        SoundManager.PlaySound(SoundManager.Sound.buttonClick);
        nextButton.SetActive(false);
        panels[panelIndex].SetActive(false);
        panelIndex++;
        if(panelIndex >= panels.Length)
        {
            OnTutorialOver?.Invoke();
            return;
        }
        panels[panelIndex].SetActive(true);
        StartCoroutine(NextDelay());
    }

    IEnumerator NextDelay()
    {
        yield return new WaitForSeconds(.5f);
        nextButton.SetActive(true);
    }
}
