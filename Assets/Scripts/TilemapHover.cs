using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TilemapHover : MonoBehaviour
{
    private Tilemap tilemap;
    
    private Vector3Int oldVec;
    private Color oldColor;
    private Color select = Color.grey;

    private MapBehaviour mapSettings;
    //private BuildingManager buildingManager;

    // Start is called before the first frame update
    void Start()
    {
        mapSettings = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        //buildingManager = NetworkClient.localPlayer.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int cellPosition = getVectorFromMouse();
        
        if(insideField(cellPosition) && oldVec != cellPosition) {
            
            if(oldVec != null) {
                tilemap.SetColor(oldVec, oldColor);
            }

            if(insideField(cellPosition)) {
                oldVec = cellPosition;
                tilemap.SetTileFlags(oldVec, TileFlags.None);
                oldColor = tilemap.GetColor(oldVec);
                tilemap.SetColor(oldVec, select);
                //if(buildingManager.isBuildingVec(cellPosition))
            }
        }
        /*if(Input.GetMouseButtonDown(0)) {
            Debug.Log(mapSettings.getBlockDetails(cellPosition));
        }*/
    }

    public Vector3Int getVectorFromMouse() {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        mouseWorldPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        return tilemap.WorldToCell(mouseWorldPos);
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