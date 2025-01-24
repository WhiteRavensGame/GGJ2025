using System.Net;
using TMPro;
using UnityEngine;

public class ScreenSlingshot : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform controllerParent;
    public Transform hook;
    public Transform mouseLoc;
    public Rigidbody2D testPhyObj;

    [Header("Balancing")]
    public float maxPullDistance;
    public float launchForceMultiplier = 10;

    [Header("Debug/TEMP")]
    public bool enableSlowMo;

    private Vector2 cursorLoc = Vector2.zero;
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

        if(enableSlowMo)
        {
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }
            
    }

    void MouseReleased()
    {
        isPressed = false;
        LaunchProjectile();
        controllerParent.gameObject.SetActive(false);
        //rb.bodyType = defaultRbBodyType;

        if (enableSlowMo)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = .02f;
        }
            
    }

    void LaunchProjectile()
    {
        Vector2 launchDirection = hook.position - mouseLoc.position;
        float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg;
        float magnitude = Vector2.Distance(hook.position, mouseLoc.position);

        //GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        //Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        testPhyObj.linearVelocity = launchDirection * launchForceMultiplier;

        //BAD IDEA - using AddForce (breaks the game by having super speed)
        //testPhyObj.AddForce(launchDirection * 5, ForceMode2D.Impulse);

        Debug.Log("LAUNCH Power: " + magnitude);
    }

}
