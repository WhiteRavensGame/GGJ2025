using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    [Header("Debug Mode")]
    public bool isStaging; 
    public bool displayDebugLogs;
    
    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //Initializes analytics to start gathering play data.
            var options = new InitializationOptions();
            if(isStaging)
            {
                options.SetEnvironmentName("staging");
                await UnityServices.InitializeAsync(options);
                AnalyticsService.Instance.StartDataCollection();
            }
            else
            {
                options.SetEnvironmentName("production");
                
                await UnityServices.InitializeAsync(options);
                AnalyticsService.Instance.StartDataCollection();
            }
            
            
        }
        else
        {
            Destroy(this.gameObject);
        } 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecordDeath(int level, float deathX, float deathY)
    {
        //ignore events when coming from individual speedruns (don't check time grinders)
        if (GameManager.Instance.GetCurrentGameMode() == GameMode.SpeedrunLevel)
            return;

        DeathPoint myEvent = new DeathPoint
        {
            Level = level,
            DeathPointX = deathX,
            DeathPointY = deathY
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        if(displayDebugLogs) 
            Debug.Log($"[ANALYTICS]: DeathPoint Event sent: Level {level} at ({deathX},{deathY})");
    }

    public void RecordLevelStart(int level)
    {
        //ignore events when coming from individual speedruns (don't check time grinders)
        if (GameManager.Instance.GetCurrentGameMode() == GameMode.SpeedrunLevel)
            return;

        LevelStarted myEvent = new LevelStarted()
        {
            Level = level
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        if (displayDebugLogs)
            Debug.Log($"[ANALYTICS] LevelStarted Event sent: Level {level} started.");

    }

    public void RecordLevelClear(int level, float time)
    {
        //ignore events when coming from individual speedruns (don't check time grinders)
        if (GameManager.Instance.GetCurrentGameMode() == GameMode.SpeedrunLevel)
            return;

        LevelClear myEvent = new LevelClear()
        {
            Level = level,
            TimeTaken = time
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        if (displayDebugLogs)  
            Debug.Log($"[ANALYTICS] LevelClear Event sent: Level {level} took {time}");


    }
}
