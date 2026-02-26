using Unity.VisualScripting;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        transform.position = target.transform.position;
    }

}
