using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using Mirror.FizzySteam;

public class NetworkManagerAnikani : NetworkManager
{   
    public string playername;

    //Stop Server Methode
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("Server stopped!");
    }

    //Start Server Methode
    public override void OnStartServer()
    {
        Debug.Log("Server started!");
        base.OnStartServer();
    }

    //Start von Client
    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        SceneManager.LoadScene("Menü");
        Destroy(gameObject);
        NetworkClient.Disconnect();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        SceneManager.LoadScene("Menü");
        Destroy(gameObject);
        NetworkClient.Disconnect();
    }

    public void changeTransport(Transport t) {
        transport = t;
        Transport.active = t;
    }

    //Auf Server Spieler hinzufügen
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        GameObject player = Instantiate(playerPrefab, transform.position, transform.rotation);
        player.GetComponent<Player>().name = playername;
        player.name = playername;
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    //Spieler disconnected
    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        base.OnServerDisconnect(conn);
    }
}