using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Mirror;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    //Aus Tutorial:
    //https://www.youtube.com/watch?v=7Eoc8U8TWa8

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    [SerializeField] private GameObject networkCanvas;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private NetworkManagerAnikani manager;

    public GameObject HostButton;
    //public Text LobbyNameText;

    void Start() {
        Debug.Log("Start");
        if(!SteamManager.Initialized) {
            Debug.Log("Starte mit dem anderen Network Manager");
            return;
        }

        HostButton.GetComponent<Button>().onClick.AddListener(HostLobby);

        Debug.Log("Steam initialized");

        manager = GetComponent<NetworkManagerAnikani>();

        GetComponent<PlayerInfo>().playername = SteamFriends.GetPersonaName().ToString();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby() {
        Debug.Log("lobby creation");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if(callback.m_eResult != EResult.k_EResultOK) {return;}
        Debug.Log("Lobby created");
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");

    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback) {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        Debug.Log("Request To Join Lobby");
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        networkCanvas.SetActive(false);
        HostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        
        //LobbyNameText.gameObject.SetActive(true);
        //LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        if(NetworkServer.active) {return;} 

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        manager.StartClient();
    }


}
