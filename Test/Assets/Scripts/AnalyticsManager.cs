using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;

            //Initializes analytics to start gathering play data.
            UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
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
        DeathPoint myEvent = new DeathPoint
        {
            Level = level,
            DeathPointX = deathX,
            DeathPointY = deathY
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        Debug.Log($"ANALYTICS: Event sent: Level {level} at ({deathX},{deathY})");
    }
}
