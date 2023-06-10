using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class LobbySettings : NetworkBehaviour
{
    [SerializeField] private Button close;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button settingsStart;
    
    [SerializeField] TMP_Dropdown xValue;
    [SerializeField] TMP_Dropdown yValue;

    [SerializeField] TMP_Dropdown distanceMainBuilding;

    [SerializeField] MapBehaviour mapBehaviour;
    [SerializeField] GameManager gameManager;

    void Start() {
        close.onClick.AddListener(onClose);
        settingsStart.onClick.AddListener(onSettings);
        xValue.onValueChanged.AddListener(delegate {
            xValueChange(xValue);
        });
        yValue.onValueChanged.AddListener(delegate {
            xValueChange(yValue);
        });

        distanceMainBuilding.onValueChanged.AddListener(delegate {
            distanceValueChange(distanceMainBuilding);
        });
    }


    void distanceValueChange(TMP_Dropdown change) {
        changeDistanceMainBuilding(int.Parse(change.options[change.value].text));
    }

    [Command(requiresAuthority=false)]
    public void changeDistanceMainBuilding(int distance) {
        gameManager.setMinAbstandMainBuilding(distance);
        RPCchangeDistanceMainBuilding(distance);
    }

    [ClientRpc]
    public void RPCchangeDistanceMainBuilding(int distance) {
        gameManager.setMinAbstandMainBuilding(distance);
    }
    
    void xValueChange(TMP_Dropdown change) {
        changeMapSize(int.Parse(change.options[change.value].text), 0);
    }
    void yValueChange(TMP_Dropdown change) {
        changeMapSize(0, int.Parse(change.options[change.value].text));
    }

    [Command(requiresAuthority=false)]
    public void changeMapSize(int x, int y) {
        if(x > 0) mapBehaviour.width = x;
        if(y > 0) mapBehaviour.height = y;
        RPCchangeMapSize(x, y);
    }

    [ClientRpc]
    public void RPCchangeMapSize(int x, int y) {
        if(x > 0) mapBehaviour.width = x;
        if(y > 0) mapBehaviour.height = y;
    }

    void onClose() {
        settingsPanel.SetActive(false);
    }

    void onSettings() {
        settingsPanel.SetActive(true);
    }
}
