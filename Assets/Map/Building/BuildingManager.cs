using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class BuildingManager : NetworkBehaviour
{
    /*
    [SerializeField] private List<Building> buildings = new List<Building>();
    [SerializeField] private Tilemap tilemap;

    private readonly SyncList<Vector3Int> vec = new SyncList<Vector3Int>();
    private readonly SyncList<int> buildingIndex = new SyncList<int>();
    private List<bool> build = new List<bool>();

    [SerializeField] private Building firstBuilding;
    private bool first = false;
    [SerializeField] private TilemapHover tilemapInfos;


    void Start() {
        if(isLocalPlayer) {
            for(int i=0; i<vec.Count; i++) {
                buildings[buildingIndex[i]].setTile(tilemap, vec[i]);
            }
        }
    }

    void Update() {
        if(buildingIndex.Count > build.Count) {
            for(int i=build.Count; i<buildingIndex.Count; i++) {
                buildings[buildingIndex[i]].setTile(tilemap, vec[i]);
                //
                build.Add(true);
            }
        }
        
        if(Input.GetMouseButtonDown(0) && !first) {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            mouseWorldPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
            if(tilemapInfos.insideField(cellPosition)) {
                first = true;
                if(isLocalPlayer) {
                    Debug.Log("test");
                    addBuilding(getBuildingIndex(firstBuilding), cellPosition);
                }
                
            }
        }
    }

    [Command]
    public void addBuilding(int indexbuilding, Vector3Int vector) {
        vec.Add(vector);
        buildingIndex.Add(indexbuilding);
        buildings[indexbuilding].setTile(tilemap, vector);
    }

    int getBuildingIndex(Building b) {
        int i=0;
        foreach (Building building in buildings)
        {
            if(b == building) {
                return i;
            }
            i++;
        }
        return i;
    }

    */
}
