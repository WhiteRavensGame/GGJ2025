using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    //References
    public static GameManager Instance;
    public TextMeshProUGUI timeField;

    public float timeElapsed = 0;
    private bool timerRunning = false;

    public List<float> times;

    [SerializeField]private GameMode currentGameMode = GameMode.MainMenu;

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
        } 
        else Destroy(this.gameObject);

    }

    void Start()
    {
        StartTimer(true);
    }

    void SaveLevelTime(float levelTime)
    {
        Debug.Log("added time: " + levelTime);
        times.Add(levelTime);
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
        UIManager.Instance.DisplayLevelCompleteScreen(true, timeElapsed);
        SaveLevelTime(timeElapsed);
    }

    public void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        //TODO: Modify this to be less likely to break everything.  
        if(nextScene >=  SceneManager.sceneCountInBuildSettings)
        {
            //No more levels after. Game end!
            Debug.Log("YOU WIN!");
            nextScene = nextScene % SceneManager.sceneCountInBuildSettings;
        }

        SceneManager.LoadScene(nextScene);

        //TODO: Have a different trigger for officially starting the time (first click?)
        StartTimer(true, true);
    }

    public void StartNewGame()
    {
        times.Clear();
        timeElapsed = 0;
        
        SceneManager.LoadScene(0);
    }

    public string ConvertFloatTimeToString(float time)
    {
        var timeSpan = TimeSpan.FromSeconds(time);
        return timeSpan.ToString("m\\:ss\\.ff");
    }

    public void ChangeGameMode(GameMode newGameMode)
    {
        Debug.Log("Change game mode: " + newGameMode.ToString());
        currentGameMode = newGameMode;

        if (currentGameMode == GameMode.MainMenu)
        {
            //Load main menu and disable the gameplay UI
            SceneManager.LoadScene(0);
            
        }
        else if(currentGameMode == GameMode.Regular)
        {
            SceneManager.LoadScene(1);
        }

        if (UIManager.Instance != null)
            UIManager.Instance.DisplayGameModeUI(currentGameMode);
        else
            Debug.LogWarning("WARNING: UI Manager Instance is null");

    }

    public GameMode GetCurrentGameMode()
    {
        return currentGameMode;
    }

    public void BackToMainMenu()
    {
        ChangeGameMode(GameMode.MainMenu);
    }


}

public enum GameMode
{
    MainMenu,
    Regular,
    Speedrun,
    End
}
