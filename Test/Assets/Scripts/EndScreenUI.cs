using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EndScreenUI : MonoBehaviour
{
    public TextMeshProUGUI textClearTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float totalClearTime = GameManager.Instance.CalculateFinalTotalTime();
        textClearTime.text = "CLEAR TIME: " + GameManager.Instance.ConvertFloatTimeToString(totalClearTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        
    public void BackToMainMenu()
    {
        GameManager.Instance.BackToMainMenu();
    }
}
