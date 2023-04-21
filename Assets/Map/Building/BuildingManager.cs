using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class BuildingManager : NetworkBehaviour
{
    [SerializeField] private TilemapHover hover;

    [SerializeField] private TileBase tile;
    [SerializeField] private Tilemap tilemap;

    private readonly SyncList<Vector3Int> save = new SyncList<Vector3Int>();
    private List<int> list = new List<int>();

    bool firstBuilding = false;

    void Start() {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        hover = GameObject.Find("GameManager").GetComponent<TilemapHover>();
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3Int vec = hover.getVectorFromMouse();
            if(hover.insideField(vec) && base.isOwned && !firstBuilding) {
                localAddTile(vec);
                firstBuilding = true;
            }
        }
        if(save.Count > list.Count) {
            for(int i=list.Count; i<save.Count; i++) {
                tilemap.SetTile(save[i], tile);
            }
        }
    }

    [Command]
    public void localAddTile(Vector3Int vec) {
        vec = new Vector3Int(vec.x, vec.y, vec.z-1);
        tilemap.SetTile(vec, tile);
        save.Add(vec);
        list.Add(1);
    }
    

    public void hostAddTile(Vector3Int vec) {
        vec = new Vector3Int(vec.x, vec.y, vec.z-1);
        tilemap.SetTile(vec, tile);
        save.Add(vec);
        list.Add(1);
    }
}