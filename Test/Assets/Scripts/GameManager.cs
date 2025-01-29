using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using LeaderboardCreatorDemo;
using Dan.Main;

public class GameManager : MonoBehaviour
{
    //References
    public static GameManager Instance;
    public TextMeshProUGUI timeField;
    public TMP_InputField nameField;

    public float timeElapsed = 0;
    private bool timerRunning = false;
    private int currentLevel = 0;

    public List<float> times;

    public string playerName;

    [SerializeField] private GameMode currentGameMode = GameMode.MainMenu;
    [SerializeField] private LeaderboardManager leaderboardManager;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            Debug.Log("SDFSDF");
            times = new List<float>();

            if (SceneManager.GetActiveScene().name == "MainMenu")
                ChangeGameMode(GameMode.MainMenu);
            else
                ChangeGameMode(GameMode.Regular);

            currentLevel = SceneManager.GetActiveScene().buildIndex;
        } 
        else Destroy(this.gameObject);

    }

    void Start()
    {
        string savedName = PlayerPrefs.GetString("PlayerName");
        if ( !string.IsNullOrEmpty(savedName) )
        {
            nameField.text = savedName.ToString();
        }
        Debug.Log(savedName);

        //QQQQ ?
        //StartTimer(true);
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    void SaveLevelTime(float levelTime)
    {
        Debug.Log("added time: " + levelTime);
        times.Add(levelTime);

        //Save to online leaderboard as well.
        leaderboardManager.UploadEntryLevel(currentLevel, timeElapsed);
    }

    public float CalculateFinalTotalTime()
    {
        float total = 0;
        foreach(var time in times)
        {
            total += time;
        }

        return total;
    }

    void FixedUpdate()
    {
        if (currentGameMode == GameMode.MainMenu || currentGameMode == GameMode.End)
            return;

        if (timerRunning)
        {
            timeElapsed += Time.unscaledDeltaTime;
            timeField.text = ConvertFloatTimeToString(timeElapsed);
        }
    }

    private void StartTimer(bool start, bool resetTimer = false)
    {
        if (resetTimer && start) timeElapsed = 0;

        timerRunning = start;
        if (!start)
        {
            Debug.Log("END TIME: " + timeElapsed);

        }
    }

    public void CompleteLevel()
    {
        StartTimer(false);
        SaveLevelTime(timeElapsed);

        if(currentGameMode == GameMode.SpeedrunLevel)
            UIManager.Instance.DisplayLevelCompleteScreen(true, timeElapsed);
        else
            UIManager.Instance.DisplayLevelCompleteScreen(true, CalculateFinalTotalTime());
        
    }

    public void RestartLevel()
    {
        UIManager.Instance.RestartLevel();
    }

    public void LoadSameLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(currentLevel);

        if (currentGameMode == GameMode.SpeedrunLevel)
            StartTimer(true, true);
    }

    public void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        currentLevel = nextScene;

        if (nextScene == totalScenes - 1)
        {
            //You reached the final scene. You win!
            Debug.Log("YOU WIN!");
            ChangeGameMode(GameMode.End);
        }
        else
        {
            SceneManager.LoadScene(nextScene);

            //TODO: Have a different trigger for officially starting the time (first click?)
            StartTimer(true, true);
        }
        
    }

    public string ConvertFloatTimeToString(float time)
    {
        var timeSpan = TimeSpan.FromSeconds(time);
        return timeSpan.ToString("m\\:ss\\.ff");
    }

    public void ChangeGameMode(GameMode newGameMode)
    {
        Debug.Log("Change game mode: " + newGameMode.ToString());
        GameMode previousGameMode = currentGameMode;
        currentGameMode = newGameMode;

        if (newGameMode == GameMode.MainMenu)
        {
            //Load main menu and disable the gameplay UI
            SceneManager.LoadScene(0);
        }
        else if(previousGameMode == GameMode.MainMenu && currentGameMode == GameMode.Regular)
        {
            UIManager.Instance.StartNewGame();
        }
        else if(newGameMode == GameMode.End)
        {
            SendTimeToLeaderboard();
        }

        if (UIManager.Instance != null)
            UIManager.Instance.DisplayGameModeUI(currentGameMode);
        else
        {
            Debug.LogWarning("WARNING: UI Manager Instance is null. Reload attempting...");
            Invoke("ReloadUI", 0.2f);
        }
            

    }

    private void ReloadUI()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.DisplayGameModeUI(currentGameMode);
        else
            Debug.LogWarning("WARNING: UI Manager Instance is STILL null. Increase timer/loads.");
    }

    public void BeginLevelSpeedrun(int level)
    {
        currentLevel = level;
        UIManager.Instance.RestartLevel();
        ChangeGameMode(GameMode.SpeedrunLevel);

        //TODO: Have a different trigger for officially starting the time (first click?)
        //StartTimer(true, true);
    }

    public GameMode GetCurrentGameMode()
    {
        return currentGameMode;
    }

    public void BackToMainMenu()
    {
        ChangeGameMode(GameMode.MainMenu);
        ResetPlayerForLeaderboard();
        times.Clear();
    }
    private void ResetPlayerForLeaderboard()
    {
        //Reset the player to allow more entries to leaderboard.
        LeaderboardCreator.ResetPlayer();
        Debug.Log("PLAYER RESET");
    }

    public void SendTimeToLeaderboard()
    {
        leaderboardManager.UploadEntry();
    }

    public void DisplayLeaderboard()
    {
        leaderboardManager.DisplaySelectedLeaderboard();
    }

    public void ChangePlayerName(string newName)
    {
        playerName = newName;
        PlayerPrefs.SetString("PlayerName", playerName);
        Debug.Log("player name changed to " + playerName);
    }
    public string GetPlayerName()
    {
        return playerName;
    }
    public void ChangeNameDebug()
    {
        float rand = UnityEngine.Random.Range(100, 9999);
        playerName = "Guest " + rand;
        Debug.Log("player name changed to " + playerName);
    }

}

public enum GameMode
{
    MainMenu,
    Regular,
    SpeedrunLevel,
    End
}
