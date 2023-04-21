using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TilemapHover : NetworkBehaviour
{
    public Tilemap tilemap;
    
    private Vector3Int oldVec;
    private Color oldColor;
    private Color select = Color.grey;

    [SerializeField] private TileBase tile;

    MapBehaviour mapSettings;

    // Start is called before the first frame update
    void Start()
    {
        mapSettings = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        mouseWorldPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

        if(insideField(cellPosition) && oldVec != cellPosition) {
            
            if(oldVec != null) {
                tilemap.SetColor(oldVec, oldColor);
            }

            if(insideField(cellPosition)) {
                oldVec = cellPosition;
                tilemap.SetTileFlags(oldVec, TileFlags.None);
                oldColor = tilemap.GetColor(oldVec);
                tilemap.SetColor(oldVec, select);
            }
            
        }

        
    }


    public bool insideField(Vector3Int vec) {
        int width = mapSettings.mapWidth();
        int height = mapSettings.mapHeight();
        if(vec.x >= 0 && vec.x <= width && vec.y >= 0 && vec.y <= height) {
            return true;
        }
        return false;
    }
}