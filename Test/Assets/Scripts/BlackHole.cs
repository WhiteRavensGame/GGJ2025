using UnityEngine;

public class BlackHole : MonoBehaviour
{
    //NOTE: This is NOT used as PointEffector2D does most of the work. Good to experiment around. 

    [SerializeField] float gravityRadius = 10f;
    [SerializeField] float gravityPull = 10f;
    [SerializeField] float rotationalForce = 5f;

    void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, gravityRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.attachedRigidbody != null)
            {
                // Calculate direction towards black hole
                Vector2 direction = (Vector2)transform.position - collider.attachedRigidbody.position;
                float distance = direction.magnitude;

                if (distance > 0f)
                {
                    // Gravity pull (inward)
                    float gravityIntensity = gravityPull / distance;
                    collider.attachedRigidbody.AddForce(direction.normalized * gravityIntensity);

                    // Rotational force (tangent) for spiral effect
                    Vector2 tangentialForce = new Vector2(-direction.y, direction.x).normalized * rotationalForce;
                    collider.attachedRigidbody.AddForce(tangentialForce / distance);
                }
            }
        }
    }
}
