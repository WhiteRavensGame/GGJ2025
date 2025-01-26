using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
