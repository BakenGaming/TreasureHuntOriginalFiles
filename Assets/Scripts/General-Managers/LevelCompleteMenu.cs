using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelCompleteMenu : MonoBehaviour
{
    public static event Action OnRankUp;
    [SerializeField] private GameObject adventurerText;
    [SerializeField] private TextMeshProUGUI rankUpText;

    public void SetAdvText()
    {
        adventurerText.SetActive(true);
        rankUpText.text = GameManager.i.GetCurrentRank().rankName;
        OnRankUp?.Invoke();
    }

    public void DisableAdvText()
    {
        adventurerText.SetActive(false);
    }
}
