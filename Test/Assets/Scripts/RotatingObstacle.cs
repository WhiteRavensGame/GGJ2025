using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private bool isClockwise;

    private void FixedUpdate()
    {
        int clockwise = isClockwise ? 1 : -1;
        transform.Rotate(0, 0, clockwise * rotateSpeed * Time.deltaTime);
    }
}
