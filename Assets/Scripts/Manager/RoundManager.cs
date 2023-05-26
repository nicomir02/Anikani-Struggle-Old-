using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class RoundManager : NetworkBehaviour
{

    //Deklarieren von benötigten Klassen
    [SerializeField] public Volk eigenesVolk;
    private NetworkManagerAnikani network;
    //Deklarieren von GameObjekten
    private GameObject lobbyObjects;
    
    //Instanzvariablen
    public int id;  //eigene Spieler-ID, bei Start initialisiert
    [SyncVar] public int currentTurn = 1;  //Angabe welche ID gerade am Turn ist
    [SyncVar] public int allids = 0;    //Zählen von Anzahl an Gesamtid's
    [SyncVar] public int round = 0;     //Zählen der Rundenzahl
    private Button roundButton;     //Knopf für Runde beenden
    private TextMeshProUGUI roundButtonText;    //Knopf zeigt an ob man gerade dran ist oder man warten muss
    private TextMeshProUGUI roundText;  //Text oben in der Leiste, welcher die Rundenanzahl angibt
    public bool isYourTurn = false;     //sagt ob dieser Spieler gerade am Turn ist
    //public string name = "Player";    //Spielername; später bei der Lobby einstellbar für das Spiel(Cheats damit verbunden?)
    public bool isLobby = true;

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
        id = allids+1;
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
            

            network = GameObject.Find("NetworkManager").GetComponent<NetworkManagerAnikani>();
            roundButton = GameObject.Find("InGame/Canvas/Runde").GetComponent<Button>();
            roundButtonText = GameObject.Find("InGame/Canvas/Runde/RundeText").GetComponent<TextMeshProUGUI>();
            roundText = GameObject.Find("InGame/Canvas/Leiste/RundenText").GetComponent<TextMeshProUGUI>();
            isLobby = false;
            roundButton.onClick.AddListener(OnClick);
            //Änderungen auf dem Button, für nächste Runde, je nachdem ob du dran bist
            if(id == 1) {
                isYourTurn = true;
                roundButtonText.text = "Next Round";
            }else {
                roundButtonText.text = "Wait";
            }
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
            //if(round == 0 && GetComponent<BuildingManager>().getZahlBuildInRound() == 0) return;
            isYourTurn = false;
            roundButtonText.text = "Wait";
            onRoundChange();
        }
    }

    //Rundenveränderung auf dem Server
    [Command(requiresAuthority = false)]
    public void onRoundChange() {
        currentTurn++;
        while(isDisqualified(currentTurn)) { //damit rausgeschmissene Spieler nicht den Spielfluss beeinflussen, da deren ID sonst eventuell aufgerufen wird
            currentTurn++;
        }

        if(currentTurn > network.numPlayers) { //Sollte es größer sein, ist erster Spieler wieder dran
            currentTurn = 1;
            round += 1;
        }
        RpconRoundChange(currentTurn);
    }

    //Rundenveränderung auf dem Client
    [ClientRpc]
    public void RpconRoundChange(int a) {
        if(a == id) {
            isYourTurn = true;
            roundButtonText.text = "Next Round";
            Player[] players = FindObjectsOfType<Player>();

            foreach(Player p in players) {
                if(p.id == id) {
                    p.auffuellen();
                }
            }

            //auffuellen();
        }
        roundText.text = "Turn " + round;
    }
    
    
}
