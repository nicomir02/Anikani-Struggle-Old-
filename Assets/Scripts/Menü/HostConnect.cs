using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using kcp2k;

public class HostConnect : MonoBehaviour{
    
    NetworkManagerAnikani manager;
    public TMP_InputField ip_InputField;
    public GameObject HostConnect_go;

    [SerializeField] private Transport hosttransport;

    [SerializeField] private GameObject networkCanvas;

    [SerializeField] TMP_InputField playername;
    [SerializeField] private Button Host;
    [SerializeField] private Button Connect;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";

    [SerializeField] private GameObject HostButton;

    [SerializeField] private Transport steamtransport;

    void Start() {
        Debug.Log("Start");
        
        if(!SteamManager.Initialized) {
            Debug.Log("Steam ist nicht initialisiert");
            return;
        }else {
            manager = GetComponent<NetworkManagerAnikani>();
            if(steamtransport != null) manager.transport = steamtransport;

            HostButton.GetComponent<Button>().onClick.AddListener(HostLobby);

            Debug.Log("Steam initialized");

            GetComponent<PlayerInfo>().playername = SteamFriends.GetPersonaName().ToString();
            playername.text = GetComponent<PlayerInfo>().playername;
            playername.readOnly = true;

            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
    }

    public void HostLobby() {
        Debug.Log("lobby creation");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if(callback.m_eResult != EResult.k_EResultOK) {return;}
        Debug.Log("Lobby created");
        GetComponent<NetworkManagerAnikani>().StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");

    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback) {
        if(steamtransport != null) manager.transport = steamtransport;
        Destroy(hosttransport);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        Debug.Log(manager.transport);
        Debug.Log("Request To Join Lobby");
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        Debug.Log("test");
        networkCanvas.SetActive(false);
        if(HostButton != null) HostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        
        //LobbyNameText.gameObject.SetActive(true);
        //LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        if(NetworkServer.active) {return;} 

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        
        manager.StartClient();
    }

    void Awake (){
        Host.onClick.AddListener(HostFunction);
        Connect.onClick.AddListener(ConnectFunction);
        manager = GetComponent<NetworkManagerAnikani>();
    }

    public void HostFunction(){
        manager.playername = playername.text;
        manager.transport = hosttransport;
        GetComponent<PlayerInfo>().playername = playername.text;
        manager.StartHost();
        HostConnect_go.SetActive(false);
        networkCanvas.SetActive(false);
    }

    public void ConnectFunction(){
        manager.transport = hosttransport;
        GetComponent<PlayerInfo>().playername = playername.text;
        manager.networkAddress = ip_InputField.text;
        manager.StartClient();
        HostConnect_go.SetActive(false);
        networkCanvas.SetActive(false);
    }
    
}
