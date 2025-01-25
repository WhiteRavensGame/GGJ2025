using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main HUD")]
    public TextMeshProUGUI mainTimerText;
    public Animator loadingAnimator;

    [Header("Level Complete Screen")]
    public GameObject levelCompleteScreen;
    public TextMeshProUGUI levelCompleteTimeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public void DisplayLevelCompleteScreen(bool show, float timeFinish = 0)
    {
        levelCompleteScreen.SetActive(show);
        if (show)
        {
            string displayTime = GameManager.Instance.ConvertFloatTimeToString(timeFinish);
            levelCompleteTimeText.text = displayTime;
            StartCoroutine(LoadNextLevel());

        }
        else
        {
            levelCompleteTimeText.text = "";
        }
    }

    IEnumerator LoadNextLevel()
    {
        //Let the player digest the win screen first, and then fade after. 
        yield return new WaitForSeconds(2f);

        //Transition Time = 0.5 secoonds for Loading Screen Start. Modify if needed
        loadingAnimator.Play("LoadingScreenStart");
        yield return new WaitForSeconds(0.5f);
        
        //Load next level
        DisplayLevelCompleteScreen(false);
        GameManager.Instance.LoadNextLevel();

        loadingAnimator.Play("LoadingScreenEnd");

        yield return null;
    }

    public TextMeshProUGUI GetMainTimerUIText()
    {
        return mainTimerText;
    }
}
