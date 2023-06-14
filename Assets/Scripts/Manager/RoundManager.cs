using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class RoundManager : NetworkBehaviour
{

    //Deklarieren von benötigten Klassen
    [SerializeField] public Volk eigenesVolk;
    [SerializeField] private NetworkManagerAnikani network;

    [SerializeField] private TextMeshProUGUI curTurnText;
    
    //Deklarieren von GameObjekten
    private GameObject lobbyObjects;
    
    //Instanzvariablen
    public int id;  //eigene Spieler-ID, bei Start initialisiert
    [SyncVar] public int currentTurn = 1;  //Angabe welche ID gerade am Turn ist
    [SyncVar] public int allids = 0;    //Zählen von Anzahl an Gesamtid's
    [SyncVar] public int round = 0;     //Zählen der Rundenzahl
    [SerializeField] private Button roundButton;     //Knopf für Runde beenden
    [SerializeField] private TextMeshProUGUI roundButtonText;    //Knopf zeigt an ob man gerade dran ist oder man warten muss
    [SerializeField] private TextMeshProUGUI roundText;  //Text oben in der Leiste, welcher die Rundenanzahl angibt
    [SerializeField] private GameObject nextrounderr;
    public bool isYourTurn = false;     //sagt ob dieser Spieler gerade am Turn ist

    [SerializeField] private TextMeshProUGUI spielername; //Text oben in Leiste für den Spielernamen
    public string playername = "Player";    //Spielername; später bei der Lobby einstellbar für das Spiel(Cheats damit verbunden?)
    public bool isLobby = true;

    public readonly SyncList<int> reihenfolge = new SyncList<int>(); //Für veränderte Reihenfolge

    //Methode um letzten Spieler zu ermitteln
    [ClientRpc]
    public void lastPlayerWinScreen() {
        if(isYourTurn) GameObject.Find("GameManager").GetComponent<WinLooseScreen>().setWinScreen();
        if(GameObject.Find("GameManager").GetComponent<GameManager>().isDisqualified(id)) GameObject.Find("GameManager").GetComponent<WinLooseScreen>().setLooseScreen();
    }

    public bool isDisqualified(int id) {
        return GameObject.Find("GameManager").GetComponent<GameManager>().isDisqualified(id);
    }

    //Initialisieren beim Start und hinzufügen von ID
    public override void OnStartClient() {        
        id = -1;
        addID();
    }

    //Hinzufügen von ID auf dem Server als Synchronisation
    [Command(requiresAuthority = false)]
    public void addID() {
        allids = allids+1;
        setAllIds(allids);
    }  

    //Setzen von wie viele IDs es derzeit auf dem Server gibt
    [ClientRpc]
    public void setAllIds(int aid) {
        allids = aid;
    }


    
    void Update() {
        if(GameObject.Find("InGame/Canvas/Runde") != null && isLobby) { //schaut ob Spiel anegfangen hat
            //Initialisieren von GameObjects die vorher nicht geladen werden konnten
            isLobby = false;
            eigenesVolk = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().volk;
            roundButton.onClick.AddListener(OnClick);
            spielername.text = playername;
            GetComponent<GameManager>().syncNames();
        }

        if(EventSystem.current.currentSelectedGameObject == null && nextrounderr.activeSelf) {
            nextrounderr.SetActive(false);
        } 

        
    }

    //Runden button Click methode
    public void OnClick() {
        if(GameObject.Find("GameManager").GetComponent<PauseMenu>().getPause()) return;

        Player[] players = FindObjectsOfType<Player>();

        foreach(Player p in players) {
            if(p.id == id) {
                BuildingManager buildingManager = p.gameObject.GetComponent<BuildingManager>();
                if(round == 0 && buildingManager.getZahlBuildInRound() == 0) return;
            }
        }
        
        if(isYourTurn) { // nur wenn du dran bist
            bool canNextRound = false;
            if(!nextrounderr.activeSelf) {
                bool unitsRound = false;
                foreach(UnitManager um in FindObjectsOfType<UnitManager>()) {
                    if(um.canNextRound() && um.isLocalPlayer) unitsRound = true;
                }

                bool buildsRound = true;

                bool unitTrainer = false;
                foreach(BuildingManager bm in FindObjectsOfType<BuildingManager>()) {
                    //if(bm.ZaehlerBuildingsBuiltInRound < bm.maxBuildingPerRound && bm.isLocalPlayer) buildsRound = false; Immer nach Buildings zu checken ist glaub ich unnötig
                    if(bm.buildUnitPanelNextRound() && bm.isLocalPlayer) unitTrainer = true;
                }

                canNextRound = unitsRound && buildsRound && unitTrainer;
            }else {
                canNextRound = true;
            }

            if(canNextRound) {
                //if(round == 0 && GetComponent<BuildingManager>().getZahlBuildInRound() == 0) return;
                isYourTurn = false;
                roundButtonText.text = "Wait";
                nextrounderr.SetActive(false);
                onRoundChange();
            }else {
                nextrounderr.SetActive(true);
            }
        }
    }

    //Damit es veränderte Reihenfolge gibt
    [Command(requiresAuthority = false)]
    public void onStartGame() {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(allids == 1) { //Nur für Testzwecke 1 Spieler erlaubt
            for(int i=1; i<allids; i++) {
                if(!gameManager.isDisqualified(i)) { 
                    reihenfolge.Add(i);
                }
            }
        }else {
            if(reihenfolge.Count == 0) {
                List<int> temp = new List<int>();
                for(int i=1; i<=allids; i++) {
                    if(!gameManager.isDisqualified(i)) { 
                        temp.Add(i);
                        reihenfolge.Add(-1);
                    }
                }

                System.Random rand = new System.Random();

                for(int i = 0; i<reihenfolge.Count; i++) {
                    int rndm = rand.Next(temp.Count);
                    int rndmtemp = temp[rndm];
                    reihenfolge[i] = rndmtemp;
                    temp.Remove(rndmtemp);
                }
            }
        }

        currentTurn = -1;

        onRoundChange();
    }

    //Rundenveränderung auf dem Server
    [Command(requiresAuthority = false)]
    public void onRoundChange() {
        currentTurn += 1;
        int turn = -1;
        if(currentTurn > reihenfolge.Count-1) { //Sollte es größer sein, ist erster Spieler wieder dran
            currentTurn = 0;
            turn = reihenfolge[currentTurn];
            round += 1;
        }else {
            turn = reihenfolge[currentTurn];
            while(isDisqualified(turn)) {
                turn = reihenfolge[currentTurn++];
                if(currentTurn > reihenfolge.Count-1) {
                    currentTurn = 0;
                }
            }
        }
        RpconRoundChange(turn);
    }

    //Rundenveränderung auf dem Client
    [ClientRpc]
    public void RpconRoundChange(int a) {
        string tryname = "";
        foreach(Player p in FindObjectsOfType<Player>()) {
            if(p.id == a) {
                p.auffuellen();
                tryname = p.name;
                isYourTurn = true;
                roundButtonText.text = "Next Round";
            }else {
                roundButtonText.text = "Wait";
            }
        }
        
        curTurnText.text = "Spieler: " + a; //später in Namen umwandeln
        roundText.text = "Turn " + round;

        if(tryname != "") turnName(tryname, a);
    }

    [Command(requiresAuthority = false)]
    public void turnName(string turn, int a) {
        curTurnText.text = "Spieler: " + turn;
        RpcturnName(turn, a);
    }

    [ClientRpc]
    public void RpcturnName(string turn, int a) {
        curTurnText.text = "Spieler: " + turn;
        curTurnText.color = GetComponent<GameManager>().spielFarben[a-1];
    }
    
    
}
