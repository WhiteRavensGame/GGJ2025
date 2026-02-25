using UnityEngine;
using UnityEngine.Rendering;

public class QQQQ : MonoBehaviour
{
    //All hacks go in here that should be disabled when shipping the game out!

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the active render pipeline asset
        RenderPipelineAsset activePipeline = GraphicsSettings.currentRenderPipeline;

        if (activePipeline != null)
        {
            Debug.Log("Active Render Pipeline: " + activePipeline.GetType().Name);

            if (activePipeline.GetType().Name.Contains("UniversalRenderPipelineAsset"))
            {
                Debug.Log("The scene is using URP!");
            }
        }
        else
        {
            Debug.Log("The scene is using the Built-in Render Pipeline.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameManager.Instance.BackToMainMenu();
        }
    }
}
