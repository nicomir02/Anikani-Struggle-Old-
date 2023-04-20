using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerAnikani : NetworkManager
{
    [SerializeField] private MapBehaviour map;
    [SerializeField] private Transform start;

    public override void OnStartServer()
    {
        Debug.Log("Server started!");
        map.createTerrain();
    }

    public override void OnStartClient()
    {
        base.OnStartServer();
        Debug.Log("Client connected to the Server!");
        //map.buildTerrain();
    }
    /*
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log(numPlayers);
        if(numPlayers == 1) {
            //Debug.Log("create");
            map.createTerrain();
        }else {
            //Debug.Log("build");
            //map.buildTerrain();
        }
    }
    */
}
