using System;
using System.Collections;
using LeaderboardCreatorDemo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main HUD")]
    public GameObject mainTimer;
    public TextMeshProUGUI mainTimerText;
    public Animator loadingAnimator;

    [Header("Level Complete Screen")]
    public GameObject levelCompleteScreen;
    public TextMeshProUGUI levelCompleteTimeText;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public GameObject leaderboardPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            GameMode mode = GameManager.Instance.GetCurrentGameMode();

            //just startup adapating to what level you spawn in.
            if ( mode != GameMode.MainMenu || mode != GameMode.End )
            {
                mainTimerText.gameObject.SetActive(true);
            }
        }
        else Destroy(this.gameObject);
    }

    public void DisplayLevelCompleteScreen(bool show, float timeFinish = 0)
    {
        levelCompleteScreen.SetActive(show);
        if (show)
        {
            string displayTime = GameManager.Instance.ConvertFloatTimeToString(timeFinish);
            levelCompleteTimeText.text = displayTime;
            StartCoroutine(LoadNextLevel());

        }
        else
        {
            levelCompleteTimeText.text = "";
        }
    }

    IEnumerator LoadNextLevel()
    {
        //Let the player digest the win screen first, and then fade after. 
        yield return new WaitForSeconds(2f);

        //Transition Time = 0.5 secoonds for Loading Screen Start. Modify if needed
        loadingAnimator.Play("LoadingScreenStart");
        yield return new WaitForSeconds(0.5f);
        
        //Load next level
        DisplayLevelCompleteScreen(false);
        GameManager.Instance.LoadNextLevel();

        loadingAnimator.Play("LoadingScreenEnd");

        yield return null;
    }

    public TextMeshProUGUI GetMainTimerUIText()
    {
        return mainTimerText;
    }

    public void DisplayGameModeUI(GameMode gameMode)
    {
        endGamePanel.SetActive(false);
        if (gameMode == GameMode.MainMenu || gameMode == GameMode.End)
        {
            mainTimer.SetActive(false);
        }
        else
        {
            mainTimer.SetActive(true);
        }

        if(gameMode == GameMode.End)
        {
            endGamePanel.SetActive(true);
        }
    }

    public void DisplayLeaderboard(bool show)
    {
        leaderboardPanel.SetActive(show);
        if(show)
        {
            GameManager.Instance.DisplayLeaderboard();
        }
        else
        {

        }
    }
}
