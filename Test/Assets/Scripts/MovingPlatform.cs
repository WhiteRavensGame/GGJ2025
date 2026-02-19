using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform platform;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [SerializeField] private float speed = 1.5f;
    private const float distanceReachThreshold = 0.05f;

    int direction = 1;
    private Vector2 start;
    private Vector2 end;
    private Vector2 destination;

    private void Start()
    {
        start = startPoint.position;
        end = endPoint.position;
        destination = end;
    }

    private void Update()
    {
        //Linear movement for platform.
        platform.position = Vector2.MoveTowards(platform.position, destination, speed * Time.deltaTime);
        CheckReverseDirection();
    }

    private void CheckReverseDirection()
    {
        float distance = (destination - (Vector2)platform.position).magnitude;
        Debug.Log(distance);
        if (distance <= distanceReachThreshold)
        {
            direction *= -1;
            if (direction == 1) destination = end;
            else destination = start;
        }
            
    }

    private void OnDrawGizmos()
    {
        if(platform != null && startPoint != null && endPoint != null)
        {
            Gizmos.DrawLine(platform.position, startPoint.position);
            Gizmos.DrawLine(platform.position, endPoint.position);
        }
    }

}
