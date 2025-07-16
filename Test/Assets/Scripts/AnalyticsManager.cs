using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    [Header("Debug Mode")]
    public bool isStaging; 
    public bool displayDebugLogs;


    [SerializeField] private bool displayDeathSpots = false;

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //Initializes analytics to start gathering play data.
            var options = new InitializationOptions();
            if(GameManager.Instance.GetDevEnvironment() == DevEnvironment.Staging)
            {
                options.SetEnvironmentName("staging");
                await UnityServices.InitializeAsync(options);
                AnalyticsService.Instance.StartDataCollection();
                Debug.Log("Staging Environment loaded.");
                SetupEvents();
                await SignUpAnonymouslyAsync();
            }
            else if (GameManager.Instance.GetDevEnvironment() == DevEnvironment.Production)
            {
                options.SetEnvironmentName("production");
                
                await UnityServices.InitializeAsync(options);
                AnalyticsService.Instance.StartDataCollection();
                Debug.Log("Production Environment loaded.");
                SetupEvents();
                await SignUpAnonymouslyAsync();
            }
            
            
        }
        else
        {
            Destroy(this.gameObject);
        } 
        
    }

    async Task SignUpAnonymouslyAsync()
    {
        try
        {
            //clearing session allows multiple scores submitted in the same computer.
            AuthenticationService.Instance.ClearSessionToken();

            //creates profile for the player quickly to start playing the game. 
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    // Setup authentication event handlers if desired
    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () => {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecordDeath(int level, float deathX, float deathY)
    {
        if (GameManager.Instance.GetDevEnvironment() == DevEnvironment.Development)
            return;

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
        if (GameManager.Instance.GetDevEnvironment() == DevEnvironment.Development)
            return;

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
        if (GameManager.Instance.GetDevEnvironment() == DevEnvironment.Development)
            return;

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

    public bool IsDisplayingDeathDeathSpots()
    {
        return displayDeathSpots;
    }
}
