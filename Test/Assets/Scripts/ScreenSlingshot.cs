using System.Net;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenSlingshot : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform controllerParent;
    public Transform hook;
    public Transform mouseLoc;
    public Rigidbody2D testPhyObj;
    private Ball playerReference;

    [Header("Balancing")]
    public float maxPullDistance;
    public float launchForceMultiplier = 10;
    public float slingEnergyRequired = 25;

    public bool enableSlowMo;
    public CursorLockMode mouseControlLockMode;

    private Vector2 cursorLoc = Vector2.zero;
    private bool isPressed = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateHookLinePosition();
        playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<Ball>();

        //Switch Mouse Control mode.
        Cursor.lockState = mouseControlLockMode;        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerReference.IsDead() || playerReference.HasWon())
        {
            EnableSlowMo(false);
            return;
        }
            

        if(Input.GetMouseButtonDown(0))
        {
            //don't perform a pull if the player is pressing a button.
            if (IsPointerOverUIButton()) return;

            MousePressed();
            cursorLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0) && isPressed)
        {
            MouseReleased();
        }

        if(isPressed)
        {
            Vector2 hookPos = hook.position;

            if(Cursor.lockState == CursorLockMode.None || Cursor.lockState == CursorLockMode.Confined)
            {
                cursorLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //QUICK WAY TO SIMULATE PHYSICS (infinite pull)
                //if (Vector2.Distance(cursorLoc, hook.position) <= 3)
                //mouseLoc.position = cursorLoc;

                Vector2 direction = cursorLoc - hookPos;
                float dist = direction.magnitude;
                if (dist > maxPullDistance)
                    direction = direction.normalized * maxPullDistance;

                Vector2 constrainedPoint = hookPos + direction;

                mouseLoc.position = constrainedPoint;
                //Debug.Log(mouseLoc.position);
                //Debug.Log(Input.mousePositionDelta);


            }
            else if(Cursor.lockState == CursorLockMode.Locked)
            {
                cursorLoc += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                Vector2 direction = cursorLoc - hookPos;
                float dist = direction.magnitude;
                if (dist > maxPullDistance)
                    direction = direction.normalized * maxPullDistance;
                Vector2 constrainedPoint = hookPos + direction;

                mouseLoc.position = constrainedPoint;
                //mouseLoc.position = cursorLoc;
                Debug.Log(mouseLoc.position + "," + cursorLoc);

            }


            lineRenderer.SetPosition(1, mouseLoc.position);

            //TESTING THE INPUT
            //if(Input.GetKeyDown(KeyCode.Space))
            //{
            //    LaunchProjectile();
            //}
        }

    }

    private bool IsPointerOverUIButton()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true; // Pointer is over a button
            }
        }

        return false;
    }

    void EnableSlowMo(bool enable)
    {
        enableSlowMo = enable;
        if(enableSlowMo)
        {
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = .02f;
        }
    }

    void UpdateHookLinePosition()
    {
        lineRenderer.SetPosition(0, hook.position);
        lineRenderer.SetPosition(1, mouseLoc.position);
    }

    void MousePressed()
    {
        //if not enough energy, no slingshot.
        if (!playerReference.HasEnoughEnergy(slingEnergyRequired))
            return;

        //Use the ball's energy to be able to slingshot.
        playerReference.ConsumeEnergy(slingEnergyRequired);

        isPressed = true;
        Vector2 mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        controllerParent.position = mouseLoc;
        UpdateHookLinePosition();
        controllerParent.gameObject.SetActive(true);
        //rb.bodyType = RigidbodyType2D.Kinematic;

        EnableSlowMo(true);
            
    }

    void MouseReleased()
    {
        isPressed = false;
        LaunchProjectile();
        controllerParent.gameObject.SetActive(false);
        //rb.bodyType = defaultRbBodyType;

        EnableSlowMo(false);

        AudioManager.Instance.PlayJumpSFX();
            
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

        //Debug.Log("LAUNCH Power: " + magnitude);
    }

}
