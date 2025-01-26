using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Ball : MonoBehaviour
{
    public BallMode currentMode;
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Energy System")]
    public float currentEnergy;
    public float maxEnergy;
    public float energyRegenerationRate;
    public float drowningEnergyRate;

    private bool insideWater;
    private bool finishedLevel;
    private bool isDead;

    //private RigidbodyType2D defaultRbBodyType;

    void Start()
    {
        //defaultRbBodyType = rb.bodyType;
        finishedLevel = false;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        //Small ball cannot breathe underwater
        if( !insideWater || (insideWater && currentMode == BallMode.Bubbled) )
        {
            currentEnergy += Time.deltaTime * energyRegenerationRate;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
        else
        {
            currentEnergy -= Time.deltaTime * drowningEnergyRate;
            if( currentEnergy <= 0 && !finishedLevel )
            {
                Die();
            }
        }
        
        UIManager.Instance.UpdatePlayerStaminaDisplay(currentEnergy, maxEnergy);

        //QQQQ : testing new levels
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeBallMode(BallMode.Small);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
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
        isDead = true;
        animator.Play("Die");
        rb.linearVelocity = Vector3.zero;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.eulerAngles = Vector3.zero;
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
            finishedLevel = true;
            animator.Play("Victory");
            transform.eulerAngles = Vector3.zero;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "WaterZone")
        {
            insideWater = false;
        }
    }

}

public enum BallMode
{
    Small = 0,
    Bubbled,
    Large
}
