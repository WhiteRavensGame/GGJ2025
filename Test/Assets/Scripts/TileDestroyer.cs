using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDestroyer : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    Vector3 mousePos;
    Vector3Int tilePos;
    [SerializeField] GridLayout grid;

    void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        tilePos = grid.LocalToCell(mousePos); //LocalTiCell?
        Debug.Log($"{mousePos}  {tilePos}");
        if (Input.GetKeyDown(KeyCode.F))
        {
            tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), null);
        }
    }
}