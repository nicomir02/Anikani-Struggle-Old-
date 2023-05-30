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
    //public string name = "Player";    //Spielername; später bei der Lobby einstellbar für das Spiel(Cheats damit verbunden?)
    public bool isLobby = true;
    
    //Spieler wird rausgeschmissen 
    //Methode aufruf wenn ein hauptgebäude zerstört wird, checked ob mehr als 1 noch drin sind 
    [Command(requiresAuthority = false)]
    public void spielerDisqualifizieren(int id) {
        if(GameObject.Find("GameManager").GetComponent<RoundManager>().currentTurn == id) GameObject.Find("GameManager").GetComponent<RoundManager>().onRoundChange();
        
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.spielerDisqualifizieren(id);

        int temp = 0;
        for(int i=1; i<=GameObject.Find("GameManager").GetComponent<RoundManager>().allids; i++) {
            if(gameManager.isDisqualified(i)) temp++;
        }
        if(temp+1 >= GameObject.Find("GameManager").GetComponent<RoundManager>().allids) {
            GameObject.Find("GameManager").GetComponent<RoundManager>().lastPlayerWinScreen();
        }
        
        UnitSprite[] units = FindObjectsOfType<UnitSprite>();

        foreach(UnitSprite u in units) {
            if(u.id == id) NetworkServer.Destroy(u.gameObject);
        }
    }



    void Start() {

        name = "Player";
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        lobbyObjects = GameObject.Find("Lobby");
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManagerAnikani>();

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
        GetComponent<BuildingManager>().auffuellen();
        GetComponent<UnitManager>().auffuellen();
        GetComponent<UnitGUIPanel>().auffuellen();
    }
    
}