using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

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
        if(timerRunning) timeElapsed += Time.deltaTime;
    }

    public void StartTimer(bool start)
    {
        timerRunning = start;
        Debug.Log("TOTAL TIME: " + timeElapsed);
    }

}
