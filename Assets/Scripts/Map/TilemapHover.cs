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
    [SerializeField] public Color select = Color.grey;

    private MapBehaviour mapSettings;
    //Nur ausschalten solange nicht anderer Vektor
    bool hoverOnKurz = true;
    //private BuildingManager buildingManager;

    // Start is called before the first frame update
    void Start()
    {
        mapSettings = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        //buildingManager = NetworkClient.localPlayer.gameObject;
    }

    //Get ausgewählte Selection Farbe
    public Color getSelectColor() {
        return select;
    }

    public void setHoverBoolKurz(bool h) {
        hoverOnKurz = h;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int cellPosition = getVectorFromMouse();
        if(oldVec != cellPosition) hoverOnKurz = true;
        if(GameObject.Find("GameManager").GetComponent<PauseMenu>().getPause() || !hoverOnKurz) return;
        
        
        if(insideField(cellPosition) && oldVec != cellPosition) {
            cellPosition.z = 0;
            
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

    public Color getOldColor() {
        return oldColor;
    }

    public void reload() {
        tilemap.SetColor(oldVec, Color.white);
        oldVec.x += 1;
        oldColor = tilemap.GetColor(oldVec);
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