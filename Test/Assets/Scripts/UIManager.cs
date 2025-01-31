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
    public GameObject optionsButton;
    public Slider playerStaminaBar;
    public Image playerStaminaBarColor;
    public Animator loadingAnimator;

    [Header("Main Menu HUD")]
    public GameObject mainMenuPanel;
    public TMP_InputField playerNameField;
    public TMP_Text versionText;
    

    [Header("Level Complete Screen")]
    public GameObject levelCompleteScreen;
    public GameObject levelSpeedrunButtonsPanel;
    public TextMeshProUGUI levelCompleteTimeText;
    public Animator levelCompleteAnimator;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public GameObject leaderboardPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            versionText.text = GameManager.Instance.GetDevEnvironment().ToString() + " v" + Application.version;
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
        //Special effect when showing the screen. Using coroutine.
        if (show)
        {
            //Hide the buttons if doing level speedruns.
            if (GameManager.Instance.GetCurrentGameMode() == GameMode.SpeedrunLevel)
                levelSpeedrunButtonsPanel.SetActive(true);
            else
                levelSpeedrunButtonsPanel.SetActive(false);

            StartCoroutine(LoadNextLevel(timeFinish));
        }
        else
        {
            levelCompleteScreen.SetActive(show);
            levelCompleteTimeText.text = "";
        }
    }

    public void StartNewGame()
    {
        StartCoroutine(LoadNextLevel(-1));
    }

    //Only during level speedrun mode.
    public void BackToMainMenuFromMidLevel()
    {
        StartCoroutine(BackToMainMenuFade());
    }

    IEnumerator BackToMainMenuFade()
    {
        loadingAnimator.Play("LoadingScreenStart");
        yield return new WaitForSeconds(0.5f);

        //Load main menu
        DisplayLevelCompleteScreen(false);
        GameManager.Instance.BackToMainMenu();

        loadingAnimator.Play("LoadingScreenEnd");
        yield return null;
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartLevelFade());
    }

    IEnumerator RestartLevelFade()
    {
        //yield return new WaitForSeconds(0.5f);

        loadingAnimator.Play("LoadingScreenStart");
        yield return new WaitForSeconds(0.5f);

        //Load next level
        DisplayLevelCompleteScreen(false);
        GameManager.Instance.LoadSameLevel();

        loadingAnimator.Play("LoadingScreenEnd");
        yield return null;
    }

    IEnumerator LoadNextLevel(float timeFinish = -1)
    {
        if(timeFinish != -1)
        {
            //Let the player digest the win animation first. 
            yield return new WaitForSeconds(1f);

            string displayTime = GameManager.Instance.ConvertFloatTimeToString(timeFinish);
            levelCompleteTimeText.text = displayTime;
            levelCompleteScreen.SetActive(true);
            levelCompleteAnimator.Play("WinAnimateIn");

            yield return new WaitForSeconds(2f);
        }
        
        //Only animate out if playing regular mode.
        if(GameManager.Instance.GetCurrentGameMode() == GameMode.Regular)
        {
            //Transition Time = 0.5 secoonds for Loading Screen Start. Modify if needed
            loadingAnimator.Play("LoadingScreenStart");
            levelCompleteAnimator.Play("WinAnimateOut");
            yield return new WaitForSeconds(0.5f);

            //Load next level
            DisplayLevelCompleteScreen(false);
            GameManager.Instance.LoadNextLevel();

            loadingAnimator.Play("LoadingScreenEnd");
        }
        
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
            optionsButton.SetActive(false);
        }
        else
        {
            mainTimer.SetActive(true);
            optionsButton.SetActive(true);
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
            mainMenuPanel.gameObject.SetActive(true);
        }
        else
        {
            mainMenuPanel.gameObject.SetActive(false);
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

    //Main Menu Input
    public void StartGamePressed()
    {
        if(playerNameField.text == "")
        {
            playerNameField.GetComponent<Animator>().Play("ErrorInput");
        }
        else
        {
            GameManager.Instance.ChangePlayerName(playerNameField.text);

            //Start game
            GameManager.Instance.ChangeGameMode(GameMode.Regular);
        }
    }
}
