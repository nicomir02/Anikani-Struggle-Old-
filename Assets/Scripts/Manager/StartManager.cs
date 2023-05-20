using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StartManager : NetworkBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject lobbyObjects;
    [SerializeField] private GameObject lobbyManager;

    public override void OnStartClient() {
        lobbyObjects.SetActive(true);
        gameManager.SetActive(true);
        lobbyManager.SetActive(true);
    }
}
