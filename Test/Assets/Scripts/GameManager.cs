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
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

    }

    void Start()
    {
        StartTimer(true);
    }

    void SaveTime()
    {
        SceneManager.GetActiveScene();
    }

    void FixedUpdate()
    {
        if (timerRunning)
        {
            timeElapsed += Time.unscaledDeltaTime;
            var timeSpan = TimeSpan.FromSeconds(timeElapsed);
            timeField.text = timeSpan.ToString("m\\:ss\\.ff");
        }
    }

    private void StartTimer(bool start)
    {
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
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //TODO: Have a different trigger for officially starting the time (first click?)
        StartTimer(true);
    }

    public void StartNewLevel()
    {

        

        //TODO: Run the rest of the intialization here.
    }


}
