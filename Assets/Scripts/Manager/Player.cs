using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Player : NetworkBehaviour
{
    //Deklarieren von benötigten Klassen
    [SerializeField] public Volk eigenesVolk;
    private NetworkManagerAnikani network;
    //Deklarieren von GameObjekten
    private GameObject lobbyObjects;
    public GameObject infobox;
    public GameObject infoboxBuilding;

    public int id = -1;
    
    private LobbyManager lobbyManager;
    private Button roundButton;     //Knopf für Runde beenden
    private TextMeshProUGUI roundButtonText;    //Knopf zeigt an ob man gerade dran ist oder man warten muss
    private TextMeshProUGUI roundText;  //Text oben in der Leiste, welcher die Rundenanzahl angibt

    public TMP_InputField ip_InputFieldName; //Input Feld der Lobby für den Namen
    [SyncVar] public new string name;    //Spielername; später bei der Lobby einstellbar für das Spiel(Cheats damit verbunden?)
    public bool isLobby = true;
    
    //Spieler wird rausgeschmissen 
    //Methode aufruf wenn ein hauptgebäude zerstört wird, checked ob mehr als 1 noch drin sind 
    [Command(requiresAuthority = false)]
    public void spielerDisqualifizieren(int id) {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        RoundManager roundManager = GameObject.Find("GameManager").GetComponent<RoundManager>();

        gameManager.spielerDisqualifizieren(id);

        if(roundManager.reihenfolge[roundManager.currentTurn] == id) roundManager.onRoundChange();

        int temp = 0;
        foreach(int i in roundManager.reihenfolge) {
            if(!gameManager.isDisqualified(i)) temp++;
        }

        if(temp == 1) {
            roundManager.lastPlayerWinScreen();
        }
        
        UnitSprite[] units = FindObjectsOfType<UnitSprite>();

        foreach(UnitSprite u in units) {
            if(u.id == id) NetworkServer.Destroy(u.gameObject);
        }
    }



    void Start() {
        //ip_InputFieldName = GameObject.Find("NetworkCanvas/InputFieldName").GetComponent<TMP_InputField>();
        //name = ip_InputFieldName.text;
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        lobbyObjects = GameObject.Find("Lobby");
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManagerAnikani>();
        name = network.GetComponent<PlayerInfo>().playername;
        GameObject.Find("GameManager").GetComponent<RoundManager>().playername = name;
        GameObject.Find("LobbyManager").GetComponent<LobbyManager>().addName(name);
        lobbyObjects.SetActive(true);
    }

     void Update() {
        if(GameObject.Find("InGame/Canvas/Runde") != null && isLobby) { //schaut ob Spiel anegfangen hat
            //Initialisieren von GameObjects die vorher nicht geladen werden konnten
            infobox = GameObject.Find("InGame/Canvas/Infobox");
            infoboxBuilding = GameObject.Find("InGame/Canvas/InfoboxBuilding");

            roundButton = GameObject.Find("InGame/Canvas/Runde").GetComponent<Button>();
            roundButtonText = GameObject.Find("InGame/Canvas/Runde/RundeText").GetComponent<TextMeshProUGUI>();
            roundText = GameObject.Find("InGame/Canvas/Leiste/RundenText").GetComponent<TextMeshProUGUI>();
            isLobby = false;
            id = GameObject.Find("GameManager").GetComponent<RoundManager>().id;
        }else {
            if(lobbyManager == null) return;
            if(lobbyManager.volk != eigenesVolk) {
                eigenesVolk = lobbyManager.volk;
            }
        }
    }

    //Aufrufen von auffüllen Methoden von verschiedenen Klassen
    //Beispielsweise neue Ressourcen, verfügbare Schritte pro Einheit...
    public void auffuellen() {
        GameObject.Find("PlayerManager").GetComponent<BuildingManager>().auffuellen();
        GameObject.Find("PlayerManager").GetComponent<UnitManager>().auffuellen();
    }
    
}