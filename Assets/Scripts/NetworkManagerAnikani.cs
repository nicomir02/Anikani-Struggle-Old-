using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerAnikani : NetworkManager
{
    [SerializeField] private MapBehaviour map;

    public override void OnStartServer()
    {
        Debug.Log("Server started!");
        map.createTerrain();
    }

    public override void OnStartClient()
    {
        Debug.Log("Client connected to the Server!");
        map.createTerrain();
    }
}
