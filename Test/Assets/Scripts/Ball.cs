using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public BallMode currentMode;
    public Rigidbody2D rb;

    //private RigidbodyType2D defaultRbBodyType;

    void Start()
    {
        //defaultRbBodyType = rb.bodyType;
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Alpha1) )
        {
            ChangeBallMode(BallMode.Small);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeBallMode(BallMode.Medium);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeBallMode(BallMode.Large);
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
        else if (currentMode == BallMode.Medium)
        {
            transform.localScale = Vector3.one * 1f;
        }
        else if (currentMode == BallMode.Large)
        {
            transform.localScale = Vector3.one * 1.5f;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("TRIGGER ENTERED: " + other.transform.tag);
        if(other.transform.tag == "Killzone")
        {
            if(currentMode == BallMode.Small)
            {
                //Dead. Respawn.
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if(currentMode == BallMode.Medium)
            {
                //Shrink player back to small
                ChangeBallMode(BallMode.Small);
            }
            
        }
    }

}

public enum BallMode
{
    Small = 0,
    Medium,
    Large
}
