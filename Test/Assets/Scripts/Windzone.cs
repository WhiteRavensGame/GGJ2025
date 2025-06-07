using UnityEngine;

public class Windzone : MonoBehaviour
{
    [SerializeField]
    private AreaEffector2D windEffector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(windEffector == null ) 
            windEffector = GetComponent<AreaEffector2D>();

        windEffector.forceAngle = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
