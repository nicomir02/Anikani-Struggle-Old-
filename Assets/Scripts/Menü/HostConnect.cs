using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HostConnect : MonoBehaviour{
    
    NetworkManagerAnikani manager;
    public TMP_InputField ip_InputField;
    public GameObject HostConnect_go;

    [SerializeField] private GameObject NetworkCanvas;

    [SerializeField] TMP_InputField playername;

    void Awake (){
        manager = GetComponent<NetworkManagerAnikani>();
    }
    public void HostFunction(){
        manager.playername = playername.text;
        GetComponent<PlayerInfo>().playername = playername.text;
        manager.StartHost();
        HostConnect_go.SetActive(false);
        NetworkCanvas.SetActive(false);
    }

    public void ConnectFunction(){
        GetComponent<PlayerInfo>().playername = playername.text;
        manager.networkAddress = ip_InputField.text;
        manager.StartClient();
        HostConnect_go.SetActive(false);
        NetworkCanvas.SetActive(false);
    }
    
}
