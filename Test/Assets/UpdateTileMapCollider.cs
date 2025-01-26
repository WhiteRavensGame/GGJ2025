using UnityEngine;
using UnityEngine.Tilemaps;

public class UpdateTilemapCollider : MonoBehaviour
{
    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        TilemapCollider2D tilemapCollider = GetComponent<TilemapCollider2D>();

        if (tilemapCollider != null)
        {
            tilemapCollider.ProcessTilemapChanges();
        }
    }
}