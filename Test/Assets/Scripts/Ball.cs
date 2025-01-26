using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public BallMode currentMode;
    public Rigidbody2D rb;

    [Header("Energy System")]
    public float currentEnergy;
    public float maxEnergy;
    public float energyRegenerationRate;

    //private RigidbodyType2D defaultRbBodyType;

    void Start()
    {
        //defaultRbBodyType = rb.bodyType;
    }

    // Update is called once per frame
    void Update()
    {
        currentEnergy += Time.deltaTime * energyRegenerationRate;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        UIManager.Instance.UpdatePlayerStaminaDisplay(currentEnergy, maxEnergy);
        
    }

    public void ChangeBallMode(BallMode newMode)
    {
        if (newMode == currentMode)
            return;

        currentMode = newMode;
        if(currentMode == BallMode.Small)
        {
            transform.localScale = Vector3.one * .5f;
        }
        else if (currentMode == BallMode.Medium)
        {
            transform.localScale = Vector3.one * 1f;
        }
        else if (currentMode == BallMode.Large)
        {
            transform.localScale = Vector3.one * 1.5f;
        }
    }

    public bool HasEnoughEnergy(float energyRequired)
    {
        if (currentEnergy >= energyRequired)
            return true;
        else
            return false;
    }
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
    public void ConsumeEnergy(float energyConsumed)
    {
        currentEnergy -= energyConsumed;
    }


    public void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("TRIGGER ENTERED: " + other.transform.tag);
        if(other.transform.tag == "Killzone")
        {
            if(currentMode == BallMode.Small)
            {
                //Dead. Respawn.
                GameManager.Instance.RestartLevel();
            }
            else if(currentMode == BallMode.Medium)
            {
                //Shrink player back to small
                ChangeBallMode(BallMode.Small);
            }
            
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            GameManager.Instance.CompleteLevel();
            collision.gameObject.SetActive(false);
            rb.bodyType = RigidbodyType2D.Static;
        }
        else if (collision.tag == "BubblePowerup")
        {
            ChangeBallMode(BallMode.Medium);
            collision.gameObject.SetActive(false);
        }

        else if (collision.tag == "MainMenuTrigger")
        {
            //QQQQ: Load First Level
            GameManager.Instance.ChangeGameMode(GameMode.Regular);
            GameManager.Instance.LoadNextLevel();
        }

    }

}

public enum BallMode
{
    Small = 0,
    Medium,
    Large
}
