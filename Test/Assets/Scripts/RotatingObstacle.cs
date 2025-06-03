using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    public float rotateSpeed;
    public bool isClockwise;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        int clockwise = isClockwise ? 1 : -1;
        transform.Rotate(0, 0, clockwise * rotateSpeed * Time.deltaTime);
    }
}
