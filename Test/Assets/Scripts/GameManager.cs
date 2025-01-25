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

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            Debug.Log("WTF");
            times = new List<float>();
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

    public void StartNewLevel()
    {
        //TODO: Run the rest of the intialization here.
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

}
