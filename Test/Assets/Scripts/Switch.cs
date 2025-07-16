using UnityEngine;

public class Switch : MonoBehaviour
{ 
    [SerializeField] private GameObject objectToHide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!objectToHide.activeInHierarchy)
            return;

        if(other.tag == "PhysicObstacles" || other.tag == "Player")
        {
            objectToHide.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "PhysicObstacles" || other.tag == "Player")
        {
            if(objectToHide != null)
                objectToHide.SetActive(true);
        }
    }
}
