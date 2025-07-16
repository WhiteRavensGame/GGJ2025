using UnityEngine;

public class QQQQ : MonoBehaviour
{
    //All hacks go in here that should be disabled when shipping the game out!

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameManager.Instance.BackToMainMenu();
        }
    }
}
