using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [Header("Trajectory Line Smoothness/Length")]
    [SerializeField] private int segmentCount = 50;
    [SerializeField] private float curveLength = 3.5f;

    private Vector2[] segments;
    private LineRenderer lineRenderer;

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    [SerializeField] private ScreenSlingshot slingshot;

    private const float TIME_CURVE_ADDITION = 0.5f;
   
    void Start()
    {
        //intialize segments
        segments = new Vector2[segmentCount];

        //grab line renderer component and set its number of points
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerRb = playerTransform.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //set the start position of line renderer
        Vector2 startPos = playerTransform.position;
        segments[0] = startPos;
        lineRenderer.SetPosition(0, startPos);

        //set the starting velocity based on the bullet physics
        //(Replaced transform.right with the actual direction of the ball)
        Vector2 startVelocity = slingshot.GetLaunchDirection() * slingshot.launchForceMultiplier;

        //set the starting velocity
        for (int i = 1; i < segmentCount; i++) 
        {
            //compute the time offset for the Rigidbody Physics
            float timeOffset = (i * Time.fixedUnscaledDeltaTime * curveLength);

            //compute the gravity offset for Rigidbody
            Vector2 gravityOffset = TIME_CURVE_ADDITION * Physics2D.gravity * playerRb.gravityScale * Mathf.Pow(timeOffset, 2);

            //set position of the point in the line renderer
            segments[i] = segments[0] + startVelocity * timeOffset + gravityOffset;
            lineRenderer.SetPosition(i, segments[i]);
        }

    }

}
