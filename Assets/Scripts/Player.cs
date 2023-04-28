using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Player : NetworkBehaviour
{
    public int id;
    [SyncVar] private int currentTurn = 1;
    [SyncVar] public int allids = 0;
    [SyncVar] public int round = 0;
    private Button roundButton;
    private TextMeshProUGUI roundButtonText;
    private TextMeshProUGUI roundText;
    public bool isYourTurn = false;

    [SerializeField] public Volk eigenesVolk;


    private NetworkManagerAnikani network;

    public bool isLobby = true;

    private GameObject lobbyObjects;

    void Start() {
        lobbyObjects = GameObject.Find("Lobby");
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManagerAnikani>();
        id = allids+1;
        addID();
    }

    [Command(requiresAuthority = false)]
    public void addID() {
        allids = allids+1;
        setAllIds(allids);

    }

    [ClientRpc]
    public void setAllIds(int aid) {
        allids = aid;
    }

    void Update() {
        if(GameObject.Find("InGame/Canvas/Runde") != null && isLobby) {
            roundButton = GameObject.Find("InGame/Canvas/Runde").GetComponent<Button>();
            roundButtonText = GameObject.Find("InGame/Canvas/Runde/RundeText").GetComponent<TextMeshProUGUI>();
            roundText = GameObject.Find("InGame/Canvas/RundenText").GetComponent<TextMeshProUGUI>();
            isLobby = false;
            roundButton.onClick.AddListener(OnClick);
            if(id == 1) {
                isYourTurn = true;
                roundButtonText.text = "Nächste Runde";
            }else {
                roundButtonText.text = "Warten";
            }
        }
    }

    public void OnClick() {
        if(isYourTurn) {
            isYourTurn = false;
            roundButtonText.text = "Warten";
            onRoundChange();
        }
    }

    [Command(requiresAuthority = false)]
    public void onRoundChange() {
        currentTurn++;
        if(currentTurn > network.numPlayers) {
            currentTurn = 1;
            round += 1;
        }
        RpconRoundChange(currentTurn);

    }

    [ClientRpc]
    public void RpconRoundChange(int a) {
        if(a == id) {
            isYourTurn = true;
            roundButtonText.text = "Nächste Runde";
            auffuellen();
        }
        roundText.text = "Runde " + round;
    }

    void auffuellen() {
        GetComponent<BuildingManager>().auffuellen();
        GetComponent<UnitManager>().auffuellen();
    }
    

    /*
    private GameManager gameManager;
    public int id;
    private static int allids = 0;

    private Button button;

    private bool gefaerbt = false;
    BuildingManager buildingManager;
    UnitManager unitManager;
    

    public int getID() {
        return id;
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        id = allids++;
        if(base.isOwned) addPlayer();
        //button = GameObject.Find("ButtonGebiet").GetComponent<Button>();
        //button.onClick.AddListener(OnButtonClick);
        buildingManager = GetComponent<BuildingManager>();
        unitManager = GetComponent<UnitManager>();
    }

    [Command]
    public void addPlayer() {
        gameManager.addID();
    }

    public void OnButtonClick() {
        if(buildingManager.getFirstBuilding()) {  
            Tilemap tilemap = buildingManager.getTilemap();
            if(gefaerbt) {
                Dictionary<Vector3Int, int> liste = gameManager.getList(id);
                foreach(KeyValuePair<Vector3Int, int> pair in liste) {
                    Vector3Int vec = pair.Key;
                    tilemap.SetTileFlags(vec, TileFlags.None);
                    tilemap.SetColor(vec, Color.white);
                    vec.z = 0;
                    tilemap.SetTileFlags(vec, TileFlags.None);
                    tilemap.SetColor(vec, Color.white);
                }
                gefaerbt = false;
            }else {
                Dictionary<Vector3Int, int> liste = gameManager.getList(id); //kann rein theoretisch einmal for dem if-else stehen statt in beiden. anderer gleicher code auch ausgelagert in methode oder nur einmal

                foreach(KeyValuePair<Vector3Int, int> pair in liste) {
                    Vector3Int vec = pair.Key;
                    tilemap.SetTileFlags(vec, TileFlags.None);
                    tilemap.SetColor(vec, gameManager.getColor(pair.Value));
                    vec.z = 0;
                    tilemap.SetTileFlags(vec, TileFlags.None);
                    tilemap.SetColor(vec, gameManager.getColor(pair.Value));
                }
                gefaerbt = true;
            }
        }
    }
*/

}
