using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    private HealthManager healthManager;

    private GameManager gameManager;
    private UnitManager unitManager;    //für testzwecke der ersten einheit

    private Button showArea;

    private bool showAreaBool = false;
    private bool isLobby = true;

    private int maxBuildingPerRound = 1;
    private int buildInRound = 0;

    private Dictionary<Vector3Int, Building> buildingsVec = new Dictionary<Vector3Int, Building>();
    private Dictionary<Vector3Int, Vector3Int> buildingvectors = new Dictionary<Vector3Int, Vector3Int>();

    Button ressourcenButton;
    Vector3Int ressourcenVec;

    Vector3Int selectedVector;
    Building selectedBuilding;


    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hover = GameObject.Find("GameManager").GetComponent<TilemapHover>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        player = GetComponent<Player>();
        volk = player.eigenesVolk;
        tilemapManager = GameObject.Find("GameManager").GetComponent<TilemapManager>();
        volkManager = GameObject.Find("GameManager").GetComponent<VolkManager>();
        mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
        healthManager = GameObject.Find("GameManager").GetComponent<HealthManager>();
        unitManager = GetComponent<UnitManager>();//für testzwecke der ersten einheit
    }

    void Update() {
        if(GameObject.Find("InGame/Canvas/ShowArea") != null && isLobby) {
            GameObject.Find("InGame/Canvas/ShowArea").GetComponent<Button>().onClick.AddListener(OnShowAreaClick);
            isLobby = false;
        }

        if(Input.GetMouseButtonDown(0) && player.isYourTurn && !player.isLobby) {
            Vector3Int vec = hover.getVectorFromMouse();
            if(hover.insideField(vec)) {
                if(player.round == 0 && base.isOwned && buildInRound == 0) {
                    bool canBuild = true;
                    List<Vector3Int> newArea = makeAreaBigger(vec, 1);
                    foreach(Vector3Int v in newArea) {
                        if(!hover.insideField(v) || !mapBehaviour.getBlockDetails(new Vector3Int(v.x, v.y, 0)).Item2.getBuildable() || gameManager.getDictionary().ContainsKey(v)) canBuild = false;
                    }
                    if(canBuild) {
                        vec.x = vec.x-1;
                        vec.y = vec.y-1;
                        vec.z = 1;
                        addBuilding(newArea, volk.getHomeBuilding(0), vec);
                        vec.x = vec.x+1;
                        vec.y = vec.y+1;
                        vec.z = 0;
                        newArea = makeAreaBigger(vec, 4);

                        add(newArea, player.id);
                        
                        vec.z = 1;
                        vec.x = vec.x-1;
                        vec.y = vec.y-1;
                        volk.setHomeBuilding(0, player.id-1, tilemap, vec);
                        buildInRound++;
                        
                        
                        tilemapManager.CmdUpdateTilemap(vec, volkManager.getVolkID(volk).Item2, 0, player.id-1);
                        //testen für erste einheit direkt mit hauptgebäude
                        vec.y = vec.y + 1;
                        vec.z = 2;
                        unitManager.spawnUnit(volk.getUnit(0),vec,player.id - 1);
                    }
                }
            }
        }

        if(Input.GetMouseButtonDown(0) && !player.isLobby) {
            Vector3Int vec = hover.getVectorFromMouse();
            vec.z = 1;
            if(selectedBuilding == null && hover.insideField(vec)) {
                selectBuilding(vec);


                vec.z = 0;
                
                if(mapBehaviour.getBlockDetails(vec).Item3 != null && buildInRound < maxBuildingPerRound && player.isYourTurn) {
                    Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();
                    if(teamVectors.ContainsKey(vec) && teamVectors[vec] == player.id) {
                        if(mapBehaviour.getBlockDetails(vec).Item3.ressourceName == "Tree") {
                            //GameObject ress = GameObject.Find("InGame/Canvas/RessourceAbbauen");
                            //ress.SetActive(true);
                            //ressourcenButton = ress.GetComponent<Button>();
                            //ressourcenButton.onClick.AddListener(OnBuildClick);
                            ressourcenVec = vec;
                        }
                    }
                }
            }else {
                deselectBuilding();
            }
        }
    }

    public void OnBuildClick() {
        //string ressname = mapBehaviour.getBlockDetails(ressourcenVec).Item3.ressourceName;
        //Debug.Log(ressname);
    }

    private void selectBuilding(Vector3Int vec) {
        if(buildingvectors.ContainsKey(vec)) {
            selectedVector = buildingvectors[vec];
            selectedBuilding = buildingsVec[selectedVector];
            selectedVector.z = 1;
            tilemap.SetTileFlags(selectedVector, TileFlags.None);
            tilemap.SetColor(selectedVector, Color.grey);
            
            activatePanel(selectedVector);
        }
    }

    public void activatePanel(Vector3Int vec) {
        player.infoboxBuilding.SetActive(true);
        GameObject.Find("InGame/Canvas/InfoboxBuilding/Infotext").GetComponent<TextMeshProUGUI>().text = "<b><u>Infobox</u></b> \n Name: "+buildingsVec[vec].getName()+"\n Leben: " +healthManager.getBuildingLeben(vec);
    }

    private void deselectBuilding() {
        //GameObject.Find("InGame/Canvas/RessourceAbbauen").SetActive(false);
        selectedVector.z = 1;
        tilemap.SetColor(selectedVector, Color.white);
        selectedVector = new Vector3Int(mapBehaviour.mapWidth()+2,mapBehaviour.mapHeight()+2,-1);
        selectedBuilding = null;
        player.infoboxBuilding.SetActive(false);
    }

    public void addBuilding(List<Vector3Int> vecs, Building b, Vector3Int vec) {
        vec.z = 1;
        buildingsVec.Add(vec, b);
        healthManager.addBuilding(vecs, b.getHealth(), vec);
        foreach(Vector3Int v in vecs) {
            buildingvectors.Add(new Vector3Int(v.x, v.y, 1), vec);
        }
    }

    public void OnShowAreaClick() {
        Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();
        if(!showAreaBool) {
            foreach(KeyValuePair<Vector3Int, int> kvp in teamVectors) {
                tilemap.SetTileFlags(kvp.Key, TileFlags.None);
                tilemap.SetColor(kvp.Key, gameManager.getColorByID(kvp.Value));

                Vector3Int vec = kvp.Key;
                vec.z = 1;

                tilemap.SetTileFlags(vec, TileFlags.None);
                tilemap.SetColor(vec, gameManager.getColorByID(kvp.Value));
            }
            showAreaBool = true;
        }else {
            foreach(KeyValuePair<Vector3Int, int> kvp in teamVectors) {
                tilemap.SetTileFlags(kvp.Key, TileFlags.None);
                tilemap.SetColor(kvp.Key, Color.white);

                Vector3Int vec = kvp.Key;
                vec.z = 1;

                tilemap.SetColor(vec, Color.white);
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