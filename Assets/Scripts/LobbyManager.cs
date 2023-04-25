using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private GameObject lobbyObjects;
    [SerializeField] private NetworkManagerAnikani network;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI readyButtonText;

    [SerializeField] private MapBehaviour mapBehaviour;

    private bool ready = false;

    [SyncVar] public int AllPlayerready = 0;

    private int minPlayers = 1;

    public override void OnStartClient() {
        lobbyObjects.SetActive(true);

        readyButton.onClick.AddListener(OnReadyClick);
    }

    public void OnReadyClick() {
        if(ready) {
            ready = false;
        }else  ready = true;
        readyPlayer(ready);
    }

    [Command(requiresAuthority = false)]
    public void readyPlayer(bool readyornot) {
        if(readyornot) {
            AllPlayerready++;
        }else {
            AllPlayerready--;
        }
        if(network.numPlayers >= minPlayers && AllPlayerready == network.numPlayers) {
            mapBehaviour.createTerrain();
            onStartGame();
        }
    }

    [ClientRpc]
    public void onStartGame() {
        lobbyObjects.SetActive(false);
        mapBehaviour.buildTerrain();
    }

}
