using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EndScreenUI : MonoBehaviour
{
    public TextMeshProUGUI textClearTime;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float totalClearTime = GameManager.Instance.CalculateFinalTotalTime();
        textClearTime.text = "CLEAR TIME: " + GameManager.Instance.ConvertFloatTimeToString(totalClearTime);

        GameManager.Instance.SendTimeToLeaderboard();
    }

    // Update is called once per frame
    void Update()
    {
        
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
