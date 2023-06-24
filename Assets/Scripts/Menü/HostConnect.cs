using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;


public class HostConnect : MonoBehaviour{
    
    NetworkManagerAnikani manager;
    public TMP_InputField ip_InputField;
    public GameObject HostConnect_go;

    [SerializeField] private Transport transport;

    [SerializeField] private GameObject NetworkCanvas;

    [SerializeField] TMP_InputField playername;
    [SerializeField] private Button Host;
    [SerializeField] private Button Connect;

    void Awake (){
        Host.onClick.AddListener(HostFunction);
        Connect.onClick.AddListener(ConnectFunction);
        manager = GetComponent<NetworkManagerAnikani>();
        
    }
    public void HostFunction(){
        manager.playername = playername.text;
        manager.transport = transport;
        GetComponent<PlayerInfo>().playername = playername.text;
        manager.StartHost();
        HostConnect_go.SetActive(false);
        NetworkCanvas.SetActive(false);
    }

    public void ConnectFunction(){
        manager.transport = transport;
        GetComponent<PlayerInfo>().playername = playername.text;
        manager.networkAddress = ip_InputField.text;
        manager.StartClient();
        HostConnect_go.SetActive(false);
        NetworkCanvas.SetActive(false);
    }
    
}
