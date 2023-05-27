using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using UnityEngine.EventSystems;

public class LobbyManager : NetworkBehaviour
{
    ///Deklarieren von benötigten Klassen und intialisieren in LobbyManager bei Unity statt bei Sart()-Methode
    [SerializeField] private GameObject lobbyObjects;
    [SerializeField] private NetworkManagerAnikani network;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI readyButtonText;
    [SerializeField] private RoundManager roundManager;

    [SerializeField] private Volk volk;

    [SerializeField] private GameObject ingameObjects;

    [SerializeField] private MapBehaviour mapBehaviour;
    [SerializeField] private GameObject gameManager;

    [SerializeField] private Button[] buttons;

    private bool ausgewaehlt = false;

    //Instanzvariablen

//Variable für Knopf ready der einzelnen Spieler
    private bool ready = false;

//Zaehlt wwie viele Spieler Ready gedrückt haben
    [SyncVar] private int AllPlayerready = 0;

//mindest Spieler Anzahl damit Spiel startet
    private int minPlayers = 1; //aus Testzwekcen nur 1 momentan

//Nach Lobby erschient der ReadyButton mit Listener
    public override void OnStartClient() {
        if(mapBehaviour.getTerrainBuild()) network.StopClient();
        lobbyObjects.SetActive(true);

        gameManager.SetActive(true);
    }

    public void Start() {
        readyButton.onClick.AddListener(OnReadyClick);

        foreach(Button b in buttons) {
            b.onClick.AddListener(farbewaehlen);
        }
    }


    //Button Click um eigene Farbe zu wählen
    public void farbewaehlen() {
        //Debug.Log(.name);
        if(ausgewaehlt) return;
        GameObject buttonGameObject = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI textMesh = buttonGameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        if(!textMesh.text.Contains("-")) {
            int i = 0;
            foreach(Button b in buttons) {
                if(b.gameObject == buttonGameObject) break;
                i++;
            }

            string playername = "Player";//Später Spielername draufschreiben

            textMesh.text = buttonGameObject.name + " - " +playername; 
            ausgewaehlt = true;
            roundManager.id = i+1;
            CMDfarbewaehlen(i, playername);
        }
    }

    [Command(requiresAuthority=false)]
    public void CMDfarbewaehlen(int i, string playername) {
        buttons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = buttons[i].name + " - " + playername;
        RPCfarbewaehlen(i, playername);
    }

    [ClientRpc]
    public void RPCfarbewaehlen(int i, string playername) {
        buttons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = buttons[i].name + " - " + playername;
    }

//Klicken auf Ready Button setzt ready Variable auf true
    public void OnReadyClick() {
        if(!ausgewaehlt) return;
        gameManager.SetActive(true);
        if(gameManager.GetComponent<PauseMenu>().getPause()) return;
        if(ready) {
            ready = false;
            readyButtonText.text = "Ready";
        }else {
            readyButtonText.text = "Not ready";
            ready = true;
        } 
        readyPlayer(ready);
    }

//Synchronisieren der Ready-Variablen der verschiednen Spieler
//Schauen ob mindest-Spieleranzahl erreicht
//schauen ob alle connected players auch ready sind
    [Command(requiresAuthority = false)]
    public void readyPlayer(bool readyornot) {
       //Counter for Ready Players
        if(readyornot) {
            AllPlayerready++;
        }else {
            AllPlayerready--;
        }
        //Prüfung ob Spiel starten kann
        if(network.numPlayers >= minPlayers && AllPlayerready == network.numPlayers) {
            int i = 0;
            roundManager.allids = 4;
            foreach(Button b in buttons) {
                if(!b.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text.Contains("-")) gameManager.GetComponent<GameManager>().spielerDisqualifizieren(i+1);
                i++;
            }

            mapBehaviour.createTerrain();
            onStartGame();
        }
    }

//Bei jedem Client startet Spiel und ingameobjekte generiert, Lobby ausgeschaltet, Map erstellt
    [ClientRpc]
    public void onStartGame() {
        lobbyObjects.SetActive(false);
        ingameObjects.SetActive(true);
        mapBehaviour.buildTerrain();
        roundManager.onStartGame();
    }

}
