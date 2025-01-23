using System.Net;
using TMPro;
using UnityEngine;

public class ScreenSlingshot : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform controllerParent;
    public Transform hook;
    public Transform mouseLoc;
    public float maxPullDistance;

    public Rigidbody2D testPhyObj;
    
    private Vector2 cursorLoc = Vector2.zero;
    private float launchForceMultiplier = 10;
    private bool isPressed = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateHookLinePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MousePressed();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            MouseReleased();
        }

        if(isPressed)
        {
            cursorLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //QUICK WAY TO SIMULATE PHYSICS (infinite pull)
            //if (Vector2.Distance(cursorLoc, hook.position) <= 3)
            //mouseLoc.position = cursorLoc;

            //LIMITED PULL (CAP PULL LENGTH)
            Vector2 hookPos = hook.position;
            Vector2 direction = cursorLoc - hookPos;
            float dist = direction.magnitude;
            if (dist > maxPullDistance)
                direction = direction.normalized * maxPullDistance;

            Vector2 constrainedPoint = hookPos + direction;

            mouseLoc.position = constrainedPoint;

            lineRenderer.SetPosition(1, mouseLoc.position);

            //TESTING THE INPUT
            //if(Input.GetKeyDown(KeyCode.Space))
            //{
            //    LaunchProjectile();
            //}
        }

    }

    void UpdateHookLinePosition()
    {
        lineRenderer.SetPosition(0, hook.position);
        lineRenderer.SetPosition(1, mouseLoc.position);
    }

    void MousePressed()
    {
        isPressed = true;
        Vector2 mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        controllerParent.position = mouseLoc;
        UpdateHookLinePosition();
        controllerParent.gameObject.SetActive(true);
        //rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void MouseReleased()
    {
        isPressed = false;
        LaunchProjectile();
        controllerParent.gameObject.SetActive(false);
        //rb.bodyType = defaultRbBodyType;
    }

    void LaunchProjectile()
    {
        Vector2 launchDirection = hook.position - mouseLoc.position;
        float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg;
        float magnitude = Vector2.Distance(hook.position, mouseLoc.position);

        //GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        //Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        testPhyObj.linearVelocity = launchDirection * launchForceMultiplier;

        Debug.Log("LAUNCH Power: " + magnitude);
    }

}
