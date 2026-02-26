using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineCamera[] allVirtualCameras;

    [Header("Controls for lerping the Y damping during target jump/fall")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling;

    private Coroutine lerpYPanCoroutine;
    private Coroutine panCameraCoroutine;

    private CinemachineCamera currentCamera;
    private CinemachinePositionComposer framingTransposer;

    private float normYPanAmount;

    private Vector2 startingTrackedObjectOffset;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            for (int i = 0; i < allVirtualCameras.Length; i++)
            {
                if (allVirtualCameras[i].enabled)
                {
                    //set the current active camera.
                    currentCamera = allVirtualCameras[i];

                    //set the cinemachine position composer
                    framingTransposer = GetCinemachinePositionComposer();
                    normYPanAmount = framingTransposer.Damping.y;

                    //CinemachineComponentBase componentBase = currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
                    //if (componentBase is CinemachinePositionComposer composer)
                    //{
                    //    framingTransposer = composer;
                    //    normYPanAmount = framingTransposer.Damping.y;

                    //    //composer.Composition.ScreenPosition.y = 0.5f;
                    //}
                }
            }

            //set starting position of the tracked object
            startingTrackedObjectOffset = framingTransposer.TargetOffset;

        }

        else
            Destroy(this.gameObject);
    }

    private CinemachinePositionComposer GetCinemachinePositionComposer()
    {
        CinemachineComponentBase componentBase = currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is CinemachinePositionComposer composer)
        {
            return composer;
        }
        else
        {
            Debug.LogWarning("WARNING: Cinemachine Position Composer component not found on currentCamera.");
            return null;
        }
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        //grab the starting damping amount
        float startDampAmount = framingTransposer.Damping.y;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = normYPanAmount;
        }

        //lerp the pan amount
        float elapsedTime = 0f;
        while (elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / fallYPanTime);
            framingTransposer.Damping.y = lerpedPanAmount;
        }

        IsLerpingYDamping = false;

        yield return null;

    }

    #endregion

    #region Pan Camera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStartingPos)
        {
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
                default: break;

            }

            endPos *= panDistance;
            startingPos = startingTrackedObjectOffset;
            endPos += startingPos;
        }
        //handle the pan back to starting position
        else
        {
            startingPos = framingTransposer.TargetOffset;
            endPos = startingTrackedObjectOffset;
        }

        //handle actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            framingTransposer.TargetOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region Swap Cameras

    //This can be extended to allow movement Upward or Downward
    public void SwapCamera(CinemachineCamera cameraFromLeft, CinemachineCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        Debug.Log($"DOG SHIT {triggerExitDirection.x} - CurrentCamera{currentCamera} - R: {cameraFromRight==currentCamera} L: {cameraFromLeft==currentCamera}");
        //if the current camera is the camera on the left and our trigger exit direction was on the right
        if(currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            //activate the new camera
            cameraFromRight.enabled = true;
            //deactivate the old camera
            cameraFromLeft.enabled = false;

            //set the new camera as the current camera
            currentCamera = cameraFromRight;
            //update our composer variable.
            framingTransposer = GetCinemachinePositionComposer();
        }
        //if the current camera is the camera on the right and our trigger exit direction was on the left
        else if (currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            //activate the new camera
            cameraFromLeft.enabled = true;
            //deactivate the old camera
            cameraFromRight.enabled = false;

            //set the new camera as the current camera
            currentCamera = cameraFromLeft;
            //update our composer variable.
            framingTransposer = GetCinemachinePositionComposer();
        }
    }

    #endregion

}
