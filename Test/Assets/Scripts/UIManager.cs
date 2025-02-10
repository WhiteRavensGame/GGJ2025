using System;
using System.Collections;
using LeaderboardCreatorDemo;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main HUD")]
    [SerializeField] private GameObject mainTimer;
    [SerializeField] private TextMeshProUGUI mainTimerText;
    [SerializeField] private GameObject optionsButton;
    [SerializeField] private Slider playerStaminaBar;
    [SerializeField] private Image playerStaminaBarColor;
    [SerializeField] private Animator loadingAnimator;

    [Header("Main Menu HUD")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private TMP_Text localRecordText;
    [SerializeField] private TMP_Text[] levelRecordsText;

    [Header("Options HUD")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Level Complete Screen")]
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private GameObject levelSpeedrunButtonsPanel;
    [SerializeField] private GameObject newPersonalBestText;
    [SerializeField] private TextMeshProUGUI levelCompleteTimeText;
    [SerializeField] private Animator levelCompleteAnimator;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject leaderboardPanel;

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

            ReloadLevelRecordTimes();

        }
        else Destroy(this.gameObject);
    }

    public void ReloadLevelRecordTimes()
    {
        for(int i = 0; i < levelRecordsText.Length; i++) 
        {
            float f = GameManager.Instance.GetLocalRecordTime(i + 1);
            if (f == Mathf.Infinity) levelRecordsText[i].text = "---";
            else levelRecordsText[i].text = GameManager.Instance.ConvertFloatTimeToString(f);

        }
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

    public void ShowOptionsMenu(bool show)
    {
        if (show)
        {
            //Disable opening options when already processing the win/lose.
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                Ball b = playerObj.GetComponent<Ball>();
                if (b != null && !b.IsPlaying())
                    return;
            }

            optionsPanel.SetActive(true);
            //Time.timeScale = 0;
            //StartCoroutine(DisplayTimerDuringPause());
        }
        else
        {
            optionsPanel.SetActive(false);
            //Time.timeScale = 1;
            //StopCoroutine(DisplayTimerDuringPause());
        }
    }

    IEnumerator DisplayTimerDuringPause()
    {
        while(true)
        {
            mainTimerText.text = GameManager.Instance.ConvertFloatTimeToString(GameManager.Instance.timeElapsed);
            mainTimerText.text = "IE " + GameManager.Instance.ConvertFloatTimeToString(GameManager.Instance.timeElapsed);
            Debug.Log("IE " + GameManager.Instance.timeElapsed);
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    public void StartNewGame()
    {
        StartCoroutine(LoadNextLevel(-1));
    }

    public void OptionsMainMenuButtonPressed()
    {
        ShowOptionsMenu(false);
        DisablePlayerBall();
        BackToMainMenuFromMidLevel();
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

    private void DisablePlayerBall()
    {
        GameObject g = GameObject.FindGameObjectWithTag("Player");
        if(g != null)
        {
            Ball b = g.GetComponent<Ball>();
            if(b != null) b.DisableBallMovement(); else Debug.LogWarning("WARNING: Ball script not found in player ball.");
        }
        else
        {
            Debug.LogWarning("WARNING: Ball not found from UI Manager.");
        }
    }

    public void RestartLevel()
    {
        DisablePlayerBall();
        ShowOptionsMenu(false); //safety
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
            //Transition Time = 0.5 seconds for Loading Screen Start. Modify if needed
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
            float fullRunRecord = GameManager.Instance.GetFullRunLocalRecord();
            if (fullRunRecord == Mathf.Infinity) localRecordText.gameObject.SetActive(false);
            else
            {
                localRecordText.gameObject.SetActive(true);
                localRecordText.text = "Local Record: " + GameManager.Instance.ConvertFloatTimeToString(fullRunRecord);
            }
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

    public void UpdatePlayerStaminaDisplay(float min, float max, float danger = 0.33f, float warning = 0.66f)
    {
        float percent = min / max;
        playerStaminaBar.value = percent;

        if (playerStaminaBar.value <= danger)
            playerStaminaBarColor.color = Color.red;
        else if (playerStaminaBar.value <= warning)
            playerStaminaBarColor.color = Color.yellow;
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
