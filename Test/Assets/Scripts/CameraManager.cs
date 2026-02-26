using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineCamera[] allVirtualCameras;

    [Header("Controls for lerping the Y damping during player jump/fall")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling;

    private Coroutine lerpYPanCoroutine;

    private CinemachineCamera currentCamera;
    private CinemachinePositionComposer framingTransposer;

    private float normYPanAmount;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            for(int i = 0;  i < allVirtualCameras.Length; i++)
            {
                if(allVirtualCameras[i].enabled)
                {
                    //set the current active camera.
                    currentCamera = allVirtualCameras[i];

                    //set the cinemachine position composer
                    CinemachineComponentBase componentBase = currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
                    if(componentBase is CinemachinePositionComposer composer)
                    {
                        framingTransposer = composer;
                        normYPanAmount = framingTransposer.Damping.y;

                        //composer.Composition.ScreenPosition.y = 0.5f;
                    }
                }
            }

        }
            
        else
            Destroy(this.gameObject);
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

        if(isPlayerFalling)
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
        while(elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / fallYPanTime);
            framingTransposer.Damping.y = lerpedPanAmount;
        }

        IsLerpingYDamping = false;

        yield return null;

    }

    #endregion



}
