using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkManagerAnikani : NetworkManager
{   
    //Benötigt um Spieler hinzufügen zu können
    [SerializeField] Transform start;
    
    public string playername;

//alexänderung
    [SerializeField] public List<string> spielerNamen = new List<string>();

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

    public override void OnStopClient() {
        SceneManager.LoadScene("Menü");

        Destroy(gameObject);
    }

    //Auf Server Spieler hinzufügen
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        player.name = "player";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    //Spieler disconnected
    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        base.OnServerDisconnect(conn);
    }

//Alexänderung
    public void AddToSpielerNameList(string name){
        spielerNamen.Add(name);
        GameObject.Find("GameManager").GetComponent<GameManager>().AddToSpielerNameList(name);
    }
}