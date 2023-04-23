using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class BuildingManager : NetworkBehaviour
{
    [SerializeField] private TilemapHover hover;
    private MapBehaviour map;

    [SerializeField] private TileBase tile;
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private Unit startUnit;

    private readonly SyncList<Vector3Int> save = new SyncList<Vector3Int>();
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
                bool canBuild = true;
                //Debug.Log(map.getBlockDetails(vec).Item2.getBuildable());

                foreach(Vector3Int v in veclist) {
                    if(!map.getBlockDetails(v).Item2.getBuildable()) {
                        canBuild = false;
                    }else if(!hover.insideField(v)) {
                        canBuild = false;
                    }
                }
                if(canBuild) {
                    localAddTile(vec, GetComponent<Player>().getID());
                    firstBuilding = true;
                }else {
                    //Debug.Log("Not possible");
                }
                
            }
        }
        if(save.Count > list.Count) {
            for(int i=list.Count; i<save.Count; i++) {
                tilemap.SetTile(save[i], tile);
            }
        }
    }


    public List<Vector3Int> makeAreaBigger(Vector3Int vec, int groesse) {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for(int x=-groesse; x<=groesse; x++) {
            for(int y=-groesse; y<=groesse; y++) {
                Vector3Int vector = new Vector3Int(x, y, 0) + vec;
                
                if(!gameManager.containsVec(vector) && hover.insideField(vector)) {
                    neighbors.Add(vector);
                }
                vector.z = 0;
                if(!gameManager.containsVec(vector) && hover.insideField(vector)) {
                    neighbors.Add(vector);
                }
            }
        }
        return neighbors;

    }

    [Command]
    public void localAddTile(Vector3Int vec, int id) {
        vec = new Vector3Int(vec.x, vec.y, vec.z-1);
        tilemap.SetTile(vec, tile);
        save.Add(vec);
        list.Add(id);

        List<Vector3Int> veclist = makeAreaBigger(vec, 4);

        foreach(Vector3Int vect in veclist) {
            gameManager.addVec(vect, id);
        }


    }
    

    public void hostAddTile(Vector3Int vec) {
        vec = new Vector3Int(vec.x, vec.y, vec.z-1);
        tilemap.SetTile(vec, tile);
        save.Add(vec);
        list.Add(1);
    }
}