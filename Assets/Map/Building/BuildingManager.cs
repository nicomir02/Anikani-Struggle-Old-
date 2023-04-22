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

    private GameManager gameManager;

    int id;

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
        id = GetComponent<Player>().getID();
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
    public void localAddTile(Vector3Int vec) {
        vec = new Vector3Int(vec.x, vec.y, vec.z-1);
        tilemap.SetTile(vec, tile);
        save.Add(vec);
        list.Add(id);

        Debug.Log(id);
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