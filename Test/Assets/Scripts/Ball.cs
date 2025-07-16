using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallMode currentBallMode;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer bubbleSprite;

    [Header("Energy System")]
    [SerializeField] private float currentEnergy = 100;
    [SerializeField] private float maxEnergy = 100;
    [SerializeField] private float energyRegenerationRate = 33;
    [SerializeField] private float drowningEnergyRate = 10;

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
        if( !insideWater || (insideWater && currentBallMode == BallMode.Bubbled) )
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

        //QQQQ : hotkeys for quick restart/main menu
        if(GameManager.Instance.GetCurrentGameMode() != GameMode.MainMenu
            && GameManager.Instance.GetCurrentGameMode() != GameMode.End
            && !HasWon() )
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                DisableBallMovement();
                UIManager.Instance.ShowOptionsMenu(false); //force close Options Menu
                GameManager.Instance.RestartLevel();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                DisableBallMovement();
                UIManager.Instance.ShowOptionsMenu(false); //force close Options Menu
                UIManager.Instance.OptionsMainMenuButtonPressed();
            }
            
        }
        
    }

    public void ChangeBallMode(BallMode newMode)
    {
        if (newMode == currentBallMode)
            return;

        currentBallMode = newMode;
        if(currentBallMode == BallMode.Small)
        {
            transform.localScale = Vector3.one * .5f;
            bubbleSprite.gameObject.SetActive(false);
        }
        else if (currentBallMode == BallMode.Bubbled)
        {
            transform.localScale = Vector3.one * 1f;
            bubbleSprite.gameObject.SetActive(true);
        }
        else if (currentBallMode == BallMode.Large)
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
        AudioManager.Instance.PlayDeathSFX();
        rb.linearVelocity = Vector3.zero;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.eulerAngles = Vector3.zero;

        //Record analytics for death
        AnalyticsManager.Instance.RecordDeath(GameManager.Instance.GetCurrentLevel(), transform.position.x, transform.position.y);

        GameManager.Instance.RestartLevel();
    }


    public void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("TRIGGER ENTERED: " + other.transform.tag);
        if(other.transform.tag == "Killzone")
        {
            if(currentBallMode == BallMode.Small)
            {
                Die();
            }
            else if(currentBallMode == BallMode.Bubbled)
            {
                //Shrink player back to small
                ChangeBallMode(BallMode.Small);
            }
            
        }
        
    }

    public bool IsDead()
    {
        return isDead;
    }
    public bool HasWon()
    {
        return finishedLevel;
    }

    public bool IsPlaying()
    {
        return (!isDead && !finishedLevel);
    }

    public void DisableBallMovement()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    private void ProcessWin()
    {
        GameManager.Instance.CompleteLevel();
        
        rb.bodyType = RigidbodyType2D.Static;
        finishedLevel = true;
        animator.Play("Victory");
        AudioManager.Instance.PlayYaySFX();
        transform.eulerAngles = Vector3.zero;
    }

    public BallMode GetPlayerBallMode()
    {
        return currentBallMode;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            collision.gameObject.SetActive(false);
            ProcessWin();
        }
        else if (collision.tag == "BubblePowerup")
        {
            ChangeBallMode(BallMode.Bubbled);
            collision.gameObject.SetActive(false);
            AudioManager.Instance.PlaySparkleSFX();
        }

        else if (collision.tag == "MainMenuTrigger")
        {
            //QQQQ: Load First Level
            GameManager.Instance.ChangeGameMode(GameMode.Regular);
        }

        if (collision.tag == "WaterZone")
        {
            AudioManager.Instance.PlayWaterSplashSFX();
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
