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
    public Slider playerStaminaBar;
    public Image playerStaminaBarColor;
    public Animator loadingAnimator;

    [Header("Main Menu HUD")]
    public TMP_InputField playerNameField;

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

    public void RestartLevel()
    {
        StartCoroutine(RestartLevelFade());
    }

    IEnumerator RestartLevelFade()
    {
        yield return new WaitForSeconds(0.5f);

        loadingAnimator.Play("LoadingScreenStart");
        yield return new WaitForSeconds(0.5f);

        //Load next level
        DisplayLevelCompleteScreen(false);
        GameManager.Instance.LoadSameLevel();

        loadingAnimator.Play("LoadingScreenEnd");
        yield return null;
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
        else
        {
            endGamePanel.SetActive(false);
        }

        if(gameMode == GameMode.MainMenu)
        {
            playerNameField.gameObject.SetActive(true);
        }
        else
        {
            playerNameField.gameObject.SetActive(false);
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

    public void UpdatePlayerStaminaDisplay(float min, float max, float danger = 0.33f)
    {
        float percent = min / max;
        playerStaminaBar.value = percent;

        if (playerStaminaBar.value <= danger)
            playerStaminaBarColor.color = Color.red;
        else
            playerStaminaBarColor.color = Color.green;


    }
}
