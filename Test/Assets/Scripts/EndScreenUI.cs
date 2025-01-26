using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EndScreenUI : MonoBehaviour
{
    public TextMeshProUGUI textClearTime;

    private void OnEnable()
    {
        float totalClearTime = GameManager.Instance.CalculateFinalTotalTime();
        textClearTime.text = GameManager.Instance.ConvertFloatTimeToString(totalClearTime);
    }

    //Button Events
    public void BackToMainMenu()
    {
        GameManager.Instance.BackToMainMenu();
    }

    public void DisplayLeaderboard()
    {
        UIManager.Instance.DisplayLeaderboard(true);
    }
}
