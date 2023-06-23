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

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private NetworkManagerAnikani manager;

    public GameObject HostButton;
    //public Text LobbyNameText;

    void Start() {
        if(!SteamManager.Initialized) {return;}

        manager = GetComponent<NetworkManagerAnikani>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby() {
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
        HostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        
        //LobbyNameText.gameObject.SetActive(true);
        //LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        if(NetworkServer.active) {return;} 

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        manager.StartClient();
    }


}
