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
    [Header("References")]
    public static GameManager Instance;
    [SerializeField] private TextMeshProUGUI timeField;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private LeaderboardManager leaderboardManager;

    [Header("Time Tracker")]
    public float timeElapsed = 0;
    private bool timerRunning = false;
    private int currentLevel = 0;

    [SerializeField] private List<float> times;
    [SerializeField] private List<float> bestTimes;
    private float bestFullRunTime;

    public string playerName;
    public bool displayDeathSpots;

    public DevEnvironment devEnvironment;

    private GameMode currentGameMode = GameMode.MainMenu;
    

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            times = new List<float>();
            bestTimes = new List<float>();

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
        
        InitializeLocalBestTimes();
    }

    public void InitializeLocalBestTimes()
    {
        //Levels
        for(int i = 1; i <= 8; i++)
        {
            float f = PlayerPrefs.GetFloat(GetRecordLevelName(i));
            if (f == 0) bestTimes.Add(Mathf.Infinity);
            else bestTimes.Add(f); 
        }

        //Full run
        float fullRunTime = PlayerPrefs.GetFloat("FullRunRecord");
        if (fullRunTime == 0) bestFullRunTime = Mathf.Infinity;
        else bestFullRunTime = fullRunTime;
    }

    public float GetLocalRecordTime(int level)
    {
        int levelIndex = level - 1;
        if (levelIndex < bestTimes.Count)
        {
            return bestTimes[levelIndex];
        }
        else
        {
            Debug.LogWarning("Level out of bounds in Record List");
            return Mathf.Infinity;
        }
    }
    public bool CheckNewRecordLocalLevelTime(int level, float timeAchieved)
    {
        int levelIndex = level - 1;
        if (bestTimes[levelIndex] <= timeAchieved)
            return false;

        bestTimes[levelIndex] = timeAchieved;
        PlayerPrefs.SetFloat(GetRecordLevelName(level), timeAchieved);
        return true;
    }
    public bool CheckNewRecordFullRunTime(float timeAchieved)
    {
        if (bestFullRunTime <= timeAchieved)
            return false;

        bestFullRunTime = timeAchieved;
        PlayerPrefs.SetFloat("FullRunRecord", timeAchieved);
        return true;
    }

    public float GetFullRunLocalRecord()
    {
        return bestFullRunTime;
    }

    public string GetRecordLevelName(int level)
    {
        return ("Record_Level" + level);
    }
    public void ResetLocalTimes()
    {
        for(int i = 0; i < bestTimes.Count; i++)
        {
            bestTimes[i] = Mathf.Infinity;
            PlayerPrefs.SetFloat(GetRecordLevelName(i+1), bestTimes[i]);
        }

        bestFullRunTime = Mathf.Infinity;
        PlayerPrefs.SetFloat("FullRunRecord", bestFullRunTime);
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    void SaveLevelTime(float levelTime)
    {
        Debug.Log("added time: " + levelTime);
        times.Add(levelTime);

        //Save to local best times
        CheckNewRecordLocalLevelTime(currentLevel, levelTime);

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
            //slowdown affected by time
            //timeElapsed += Time.deltaTime;

            //slowdown unaffected by time
            timeElapsed += Time.unscaledDeltaTime;

            timeField.text = ConvertFloatTimeToString(timeElapsed);
        }
    }

    private void StartTimer()
    {
        timerRunning = true;
    }

    private void ResetTimer()
    {
        timeElapsed = 0;
    }
    private void StopTimer()
    {
        timerRunning = false;
        Debug.Log("END TIME: " + timeElapsed);
    }



    public void CompleteLevel()
    {
        StopTimer();
        SaveLevelTime(timeElapsed);
        UIManager.Instance.ShowOptionsMenu(false);

        if(currentGameMode == GameMode.SpeedrunLevel)
            UIManager.Instance.DisplayLevelCompleteScreen(true, timeElapsed);
        else
            UIManager.Instance.DisplayLevelCompleteScreen(true, CalculateFinalTotalTime());

        AnalyticsManager.Instance.RecordLevelClear(currentLevel, timeElapsed);
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
        {
            ResetTimer();
            StartTimer();
        }
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
            ResetTimer();
            StartTimer();
            AnalyticsManager.Instance.RecordLevelStart(currentLevel);

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
        StopTimer();
        ResetTimer();
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

    public DevEnvironment GetDevEnvironment()
    {
        return devEnvironment;
    }

}

public enum GameMode
{
    MainMenu,
    Regular,
    SpeedrunLevel,
    End
}
public enum DevEnvironment
{ 
    Development = -1,
    Staging = 0,
    Production
}
