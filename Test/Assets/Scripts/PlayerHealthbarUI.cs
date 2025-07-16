using UnityEngine;

public class PlayerHealthbarUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    private Transform player;
    private Ball ball;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetPlayerReference();
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            if(ball.GetPlayerBallMode() == BallMode.Small)
                transform.position = player.position + offset;
            else
            {
                float largeBallOffsetMultiplier = 2f;
                transform.position = player.position + offset * largeBallOffsetMultiplier;
            }
                
        }
            
        else
        {
            GetPlayerReference();
        }

    }

    public void GetPlayerReference()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<Transform>();
            ball = playerObj.GetComponent<Ball>();
        }
    }
}
