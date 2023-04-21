using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    private GameManager gameManager;
    private int id;

    

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(base.isOwned) addPlayer();

    }

    void Update() {
        
    }

    [Command]
    public void addPlayer() {
        id = gameManager.playerid.Count;
        gameManager.playerid.Add(gameManager.playerid.Count);
    }
}
