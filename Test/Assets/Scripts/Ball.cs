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
    public float drowningEnergyRate;

    private bool insideWater;
    private bool finishedLevel;

    //private RigidbodyType2D defaultRbBodyType;

    void Start()
    {
        //defaultRbBodyType = rb.bodyType;
    }

    // Update is called once per frame
    void Update()
    {
        //Small ball cannot breathe underwater
        if( !insideWater || (insideWater && currentMode == BallMode.Bubbled) )
        {
            currentEnergy += Time.deltaTime * energyRegenerationRate;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
        else
        {
            currentEnergy -= Time.deltaTime * drowningEnergyRate;
            if( currentEnergy <= 0 && finishedLevel )
            {
                Die();
            }
        }
        
        UIManager.Instance.UpdatePlayerStaminaDisplay(currentEnergy, maxEnergy);

        //QQQQ : testing new levels
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeBallMode(BallMode.Small);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeBallMode(BallMode.Bubbled);
        }
        
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
        else if (currentMode == BallMode.Bubbled)
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

    private void Die()
    {
        //Dead. Respawn.
        GameManager.Instance.RestartLevel();
    }


    public void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("TRIGGER ENTERED: " + other.transform.tag);
        if(other.transform.tag == "Killzone")
        {
            if(currentMode == BallMode.Small)
            {
                Die();
            }
            else if(currentMode == BallMode.Bubbled)
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
            ChangeBallMode(BallMode.Bubbled);
            collision.gameObject.SetActive(false);
        }

        else if (collision.tag == "MainMenuTrigger")
        {
            //QQQQ: Load First Level
            GameManager.Instance.ChangeGameMode(GameMode.Regular);
            GameManager.Instance.LoadNextLevel();
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "WaterZone")
        {
            insideWater = true;
        }
    }

}

public enum BallMode
{
    Small = 0,
    Bubbled,
    Large
}
