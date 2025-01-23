using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;

    private bool isPressed = false;
    private RigidbodyType2D defaultRbBodyType;

    void Start()
    {
        defaultRbBodyType = rb.bodyType;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPressed)
        {
            rb.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void OnMouseDown()
    {
        isPressed = true;
        //rb.isKinematic = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnMouseUp()
    {
        isPressed = false;
        rb.bodyType = defaultRbBodyType;

        Debug.Log("BOb was here!");
    }
}
