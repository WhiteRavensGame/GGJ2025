using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public TextMeshProUGUI timeField;

    public float timeElapsed = 0;
    private bool timerRunning = false;


    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            StartTimer(true);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    void FixedUpdate()
    {
        if (timerRunning)
        {
            timeElapsed += Time.deltaTime;
            var timeSpan = TimeSpan.FromSeconds(timeElapsed);

            timeField.text = timeSpan.ToString("m\\:ss\\.ff");
        }
    }

    public void StartTimer(bool start)
    {
        timerRunning = start;
        Debug.Log("TOTAL TIME: " + timeElapsed);
    }

}
