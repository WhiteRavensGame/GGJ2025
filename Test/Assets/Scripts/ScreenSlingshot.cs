using System.Net;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenSlingshot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform controllerParent;
    [SerializeField] private Transform hook;
    [SerializeField] private Transform mouseLoc;
    [SerializeField] private Rigidbody2D testPhyObj;
    private Ball playerReference;
    [SerializeField] private GameObject trajectoryLine;

    [Header("Balancing")]
    [SerializeField] private float maxPullDistance = 3;
    [SerializeField] private float launchForceMultiplier = 5;
    [SerializeField] private float slingEnergyRequired = 33;

    [Header("Slo-mo")]
    [SerializeField] private bool enableSlowMo = true;
    [SerializeField] private float sloMoTimescale = 0.1f;
    private float defaultFixedDeltaTime = .02f;
    [SerializeField] private CursorLockMode mouseControlLockMode = CursorLockMode.Locked;

    private Vector2 cursorLoc = Vector2.zero;
    private bool isPressed = false;

    

    void Start()
    {
        UpdateHookLinePosition();
        playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<Ball>();

        //Switch Mouse Control mode.
        Cursor.lockState = mouseControlLockMode;

        //Set initial values. 
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        if (playerReference.IsDead() || playerReference.HasWon())
        {
            EnableSlowMo(false);
            return;
        }

        //QQQQ - change after fan expo
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            mouseControlLockMode = CursorLockMode.None;
            Cursor.lockState = mouseControlLockMode;
        }
        else
        {
            mouseControlLockMode = CursorLockMode.Locked;
            Cursor.lockState = mouseControlLockMode;
        }

        if (Input.GetMouseButtonDown(0))
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
            }

            lineRenderer.SetPosition(1, mouseLoc.position);
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
        //TODO: Fix slo-mo timers to be consistent with actual slowdown. 
        enableSlowMo = enable;
        if(enableSlowMo)
        {
            Time.timeScale = sloMoTimescale;
            Time.fixedDeltaTime = Time.timeScale * defaultFixedDeltaTime;
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
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

        //activate trajectory line on player ball
        trajectoryLine.SetActive(true);

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
        //activate trajectory line on player ball
        trajectoryLine.SetActive(false);

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

    public Vector2 GetLaunchDirection()
    {
        return hook.position - mouseLoc.position;
    }
    public float GetLaunchForceMultiplier()
    {
        return launchForceMultiplier;
    }

}
