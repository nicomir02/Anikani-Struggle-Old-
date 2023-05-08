using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    ///Deklarieren von benötigten Klassen und intialisieren in LobbyManager bei Unity statt bei Sart()-Methode
    [SerializeField] private GameObject lobbyObjects;
    [SerializeField] private NetworkManagerAnikani network;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI readyButtonText;

    [SerializeField] private Volk volk;

    [SerializeField] private GameObject ingameObjects;

    [SerializeField] private MapBehaviour mapBehaviour;

    //Instanzvariablen

//Variable für Knopf ready der einzelnen Spieler
    private bool ready = false;

//Zaehlt wwie viele Spieler Ready gedrückt haben
    [SyncVar] private int AllPlayerready = 0;

//mindest Spieler Anzahl damit Spiel startet
    private int minPlayers = 1; //aus Testzwekcen nur 1 momentan

//Nach Lobby erschient der ReadyButton mit Listener
    public override void OnStartClient() {
        
        lobbyObjects.SetActive(true);

        readyButton.onClick.AddListener(OnReadyClick);
    }

//Klicken auf Ready Button setzt ready Variable auf true
    public void OnReadyClick() {
        if(GameObject.Find("GameManager").GetComponent<PauseMenu>().getPause()) return;
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
    }

}
