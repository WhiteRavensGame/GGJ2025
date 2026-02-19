using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] private int objHealth = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            objHealth--;

            if(objHealth <= 0)
                Destroy(this.gameObject);
        }
    }
}
