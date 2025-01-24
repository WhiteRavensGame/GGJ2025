using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTile : MonoBehaviour
{
    private Tilemap destructibleTilemap;
    public GameObject g;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destructibleTilemap = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Vector3 hitPosition = collision.contacts[0].point;
            //destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(hitPosition), null);

            Vector3 hitPosition = Vector3.zero;
            foreach (ContactPoint2D hit in collision.contacts)
            {
                hitPosition.x = hit.point.x - 0.1f * hit.normal.x;
                hitPosition.y = hit.point.y - 0.1f * hit.normal.y;
                destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(hitPosition), null);
            }
        }
    }
}
