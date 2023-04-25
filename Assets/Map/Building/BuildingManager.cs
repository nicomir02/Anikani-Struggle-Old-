using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class BuildingManager : NetworkBehaviour
{
    /*
    [SerializeField] private TilemapHover hover;
    private MapBehaviour map;

    [SerializeField] private Building[] building;
    [SerializeField] private TileBase tile;
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private Unit startUnit;

    //SyncDictionary Value int für building id aus array
    private readonly SyncDictionary<Vector3Int, int> save = new SyncDictionary<Vector3Int, int>();
    private List<int> list = new List<int>();

    private GameManager gameManager;

    private bool firstBuilding = false;

    

    public bool getFirstBuilding() {
        return firstBuilding; 
    }

    public Tilemap getTilemap() {
        return tilemap;
    }

    void Start() {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        hover = GameObject.Find("GameManager").GetComponent<TilemapHover>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        map = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3Int vec = hover.getVectorFromMouse();
            if(hover.insideField(vec) && base.isOwned && !firstBuilding) {
                List<Vector3Int> veclist = makeAreaBigger(vec, 1);
                vec.x = vec.x-1;
                vec.y = vec.y-1;
                bool canBuild = true;
                foreach(Vector3Int v in veclist) {
                    if(!hover.insideField(v) || !map.getBlockDetails(v).Item2.getBuildable()) {
                        canBuild = false;
                    }
                }
                if(canBuild) {
                    localAddTile(vec, GetComponent<Player>().getID());
                    firstBuilding = true;
                }else {
                    //Debug.Log("Not possible");
                }
                
            }else if(firstBuilding) {
                
            }
        }
        if(save.Count > list.Count) {
            foreach(KeyValuePair<Vector3Int, int> kvp in save) {
                tilemap.SetTile(kvp.Key, building[kvp.Value].getTile());
                list.Add(1);
            }
        }
    }

    public bool isBuildingVec(Vector3Int vec) {
        vec.z = 1;
        vec.x = vec.x-1;
        vec.y = vec.y-1;
        List<Vector3Int> liste = makeAreaBigger(vec, 1);
        bool a = false;
        foreach(Vector3Int v in liste) {
            if(save.ContainsKey(v)) {
                a = true;
                break;
            }
        }
        return a;
    }


    public List<Vector3Int> makeAreaBigger(Vector3Int vec, int groesse) {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for(int x=-groesse; x<=groesse; x++) {
            for(int y=-groesse; y<=groesse; y++) {
                Vector3Int vector = new Vector3Int(x, y, 0) + vec;
                neighbors.Add(vector);
            }
        }
        return neighbors;

    }

    [Command]
    public void localAddTile(Vector3Int vec, int id) {
        vec = new Vector3Int(vec.x, vec.y, vec.z+1);
        tilemap.SetTile(vec, building[id].getTile());
        save.Add(vec, id);
        list.Add(id);

        vec.x = vec.x+1;
        vec.y = vec.y+1;
        List<Vector3Int> veclist = makeAreaBigger(vec, 4);

        foreach(Vector3Int vect in veclist) {
            if(hover.insideField(vect) && !gameManager.teamVecs.ContainsKey(vect)) {
                gameManager.addVec(vect, id);
            }
            
        }
    }
    */
}