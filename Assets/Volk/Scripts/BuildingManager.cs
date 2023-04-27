using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Mirror;

public class BuildingManager : NetworkBehaviour
{
    private Volk volk;
    private Player player;
    private TilemapHover hover;
    private Tilemap tilemap;
    private TilemapManager tilemapManager;
    private VolkManager volkManager;
    private MapBehaviour mapBehaviour;

    private GameManager gameManager;

    private Button showArea;

    private bool showAreaBool = false;
    private bool isLobby = true;

    //private int maxBuildingPerRound = 1;
    private int buildInRound = 0;


    Color getColorByID(int id) {
        if(id==1) {
            return Color.blue;
        }else if(id==2) {
            return Color.red;
        }
        return Color.white;
    }

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hover = GameObject.Find("GameManager").GetComponent<TilemapHover>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        player = GetComponent<Player>();
        volk = player.eigenesVolk;
        tilemapManager = GameObject.Find("GameManager").GetComponent<TilemapManager>();
        volkManager = GameObject.Find("GameManager").GetComponent<VolkManager>();
        mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
    }

    void Update() {
        if(Input.GetMouseButtonDown(0) && player.isYourTurn && !player.isLobby) {
            Vector3Int vec = hover.getVectorFromMouse();
            if(hover.insideField(vec)) {
                if(player.round == 0 && base.isOwned && buildInRound == 0) {
                    bool canBuild = true;
                    List<Vector3Int> newArea = makeAreaBigger(vec, 1);
                    foreach(Vector3Int v in newArea) {
                        if(!hover.insideField(v) || !mapBehaviour.getBlockDetails(v).Item2.getBuildable()) canBuild = false;
                    }
                    if(canBuild) {
                        newArea = makeAreaBigger(vec, 4);

                        add(newArea, player.id);
                        
                        vec.z = 1;
                        vec.x = vec.x-1;
                        vec.y = vec.y-1;
                        volk.setBuilding(0, player.id-1, tilemap, vec);
                        buildInRound++;

                        tilemapManager.CmdUpdateTilemap(vec, volkManager.getVolkID(volk).Item2, 0, player.id-1);
                    }
                }
            }
        }

        if(GameObject.Find("InGame/Canvas") != null && isLobby) {
            showArea = GameObject.Find("InGame/Canvas/ShowArea").GetComponent<Button>();
            showArea.onClick.AddListener(OnShowAreaClick);
            isLobby = false;
        }
    }

    public void OnShowAreaClick() {
        Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();
        if(!showAreaBool) {
            foreach(KeyValuePair<Vector3Int, int> kvp in teamVectors) {
                tilemap.SetTileFlags(kvp.Key, TileFlags.None);
                tilemap.SetColor(kvp.Key, getColorByID(kvp.Value));
            }
            showAreaBool = true;
        }else {
            foreach(KeyValuePair<Vector3Int, int> kvp in teamVectors) {
                tilemap.SetTileFlags(kvp.Key, TileFlags.None);
                tilemap.SetColor(kvp.Key, Color.white);
            }
            showAreaBool = false;
        }
    }

    public void auffuellen() {
        buildInRound = 0;
    }

    [Command]
    public void add(List<Vector3Int> vecs, int id) {
        foreach(Vector3Int vec in vecs) {
            if(!gameManager.hasVec(vec)) {
                gameManager.addVec(vec, id);
            }
        }
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
}