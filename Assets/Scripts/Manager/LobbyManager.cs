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

    [SerializeField] public Volk volk;

    [SerializeField] private GameObject ingameObjects;

    [SerializeField] private MapBehaviour mapBehaviour;
    [SerializeField] private GameObject gameManager;

    [SerializeField] private VolkManager volkManager;
    [SerializeField] private TMP_Dropdown chooseTribe;

    [SerializeField] private Button[] buttons;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private Button backToMainMenu;
    [SerializeField] private NetworkManager networkManager;

    public string playername = "Player";

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

        foreach(Volk v in volkManager.volkList) {
            chooseTribe.options.Add(new TMP_Dropdown.OptionData(v.name));
        }

        chooseTribe.onValueChanged.AddListener(delegate {
            valueChange();
        });
        

        gameManager.SetActive(true);
        startClient();
    }

    //Value Change im Dropdown für Choose a Tribe
    public void valueChange() {
        volk = volkManager.getVolkByString(chooseTribe.options[chooseTribe.value].text);
        if(volk != null) {
            title.text = volk.name;
            description.text = volk.getDescription();
        }else {
            title.text = "";
            description.text = "";
        }
    }


    //Wenn Spieler disconnected muss er wieder ausgetragen werden, sollte lobby noch aktiv sein
    /*
    public void OnServerDisconnect() {
        
    }

    [Command(requiresAuthority=false)]
    public void stopClient(int id) {
        if(id < 0) return; 
        RpcstopClient(id);
    }

    [ClientRpc]
    public void RpcstopClient(int id) {
        buttons[id].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = buttons[id].name;
    }
    */

    //Beim starten von Clients müssen die Buttons den richtigen Schriftzug haben
    //Wird gespeichert und an ClientRpc changeButtons übergeben
    [Command(requiresAuthority=false)]
    public void startClient() {
        List<string> texte = new List<string>();
        foreach(Button b in buttons) {
            texte.Add(b.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text);
        }

        changeButtons(texte);
    }

    

    //Buttons werden aktualisiert auf den Clients
    [ClientRpc]
    public void changeButtons(List<string> texte) {
        for(int i=0; i<texte.Count; i++) {
            buttons[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = texte[i];
        }
    }


    public void Start() {
        readyButton.onClick.AddListener(OnReadyClick);
        backToMainMenu.onClick.AddListener(backToMainMenuClick);

        foreach(Button b in buttons) {
            b.onClick.AddListener(farbewaehlen);
        }
    }

    public void backToMainMenuClick() {
        GameObject buttonGameObject = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI textMesh = buttonGameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        if(ausgewaehlt) {
            
            CMDfarbewaehlen(roundManager.id-1, "");
        }

        StartCoroutine(disconnectDelay());
    }

    IEnumerator disconnectDelay()
    {
        yield return new WaitForSeconds(0.001f);

        if (NetworkServer.active && NetworkClient.isConnected) {
            networkManager.StopHost();
        }else if (NetworkClient.isConnected) {
            networkManager.StopClient();
        }
        
            
    }


    //Button Click um eigene Farbe zu wählen
    public void farbewaehlen() {
        //Debug.Log(.name);

        GameObject buttonGameObject = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI textMesh = buttonGameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        //Für Deselect
        if(ausgewaehlt) {
            int i = 0;
            foreach(Button b in buttons) {
                if(b.gameObject == buttonGameObject) break;
                i++;
            }
            
            if(roundManager.id == i+1) {
                textMesh.text = buttonGameObject.name;
                ausgewaehlt = false;
                roundManager.id = -1;
                CMDfarbewaehlen(i, "");
            }
            return;
        }
        
        //Für Select
        if(!textMesh.text.Contains("-")) {
            int i = 0;
            foreach(Button b in buttons) {
                if(b.gameObject == buttonGameObject) break;
                i++;
            }

            textMesh.text = buttonGameObject.name + " - " +playername; 
            ausgewaehlt = true;
            roundManager.id = i+1;
            CMDfarbewaehlen(i, playername);
        }
    }

    //Farbe wählen auf Server
    [Command(requiresAuthority=false)]
    public void CMDfarbewaehlen(int i, string playername) {
        TextMeshProUGUI textgui = buttons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if(playername != "") {
            textgui.text = buttons[i].name + " - " + playername;
            
        }else {
            textgui.text = buttons[i].name;
        }
        RPCfarbewaehlen(i, playername);
        
    }

    //Farbe wählen Übertragung auf Client
    [ClientRpc]
    public void RPCfarbewaehlen(int i, string playername) {
        TextMeshProUGUI textgui = buttons[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if(playername != "") {
            textgui.text = buttons[i].name + " - " + playername;
        }else {
            textgui.text = buttons[i].name;
        }
    }

//Klicken auf Ready Button setzt ready Variable auf true
    public void OnReadyClick() {
        if(!ausgewaehlt || volk == null) return;
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
