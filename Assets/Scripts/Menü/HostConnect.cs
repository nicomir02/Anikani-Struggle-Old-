using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HostConnect : MonoBehaviour{
 
    NetworkManagerAnikani manager;
    public TMP_InputField ip_InputField;
    public GameObject HostConnect_go;

    [SerializeField] TMP_InputField playername;

    //Alexänderung
    public int count = 1;

    void Awake (){
        manager = GetComponent<NetworkManagerAnikani>();
    }
    public void HostFunction(){
        manager.playername = playername.text;
        manager.StartHost();
        HostConnect_go.SetActive(false);
        //Alexänderung
        //Debug.Log("Im HostConnectManagerHostName= " + manager.playername);
        //manager.AddToSpielerNameList(playername.text);
        //da bei networkmanager nicht geklappt:
        //GameObject.Find("GameManager").GetComponent<GameManager>().AddToSpielerNameList(playername.text);
        GameObject.Find("LobbyManager").GetComponent<LobbyManager>().AddToSpielerNameList(playername.text);

        GetComponent<PlayerInfo>().playername = playername.text;
    }

    public void ConnectFunction(){
        manager.playername = playername.text;
        
        manager.networkAddress = ip_InputField.text;
        manager.StartClient();
        HostConnect_go.SetActive(false);
        GetComponent<PlayerInfo>().playername = playername.text;
        //Alexänderung
        //Debug.Log("Im HostConnectManagerClientName= " + manager.playername);
        //manager.AddToSpielerNameList(playername.text);
        //da bei networkmanager nicht geklappt:
        //wirft auch fehler
        //GameObject.Find("GameManager").GetComponent<GameManager>().AddToSpielerNameList(playername.text);
        //GameObject.Find("LobbyManager").GetComponent<LobbyManager>().AddToSpielerNameList(playername.text);
    }
}
