using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;

public class WinLooseScreen : NetworkBehaviour
{
    [SerializeField] GameObject winLooseCanvas;
    [SerializeField] RawImage winLooseCanvasBackground;
    [SerializeField] TextMeshProUGUI winLooseCanvasText;
    [SerializeField] Button mainMenu;
    [SerializeField] Button spectateButton;
    [SerializeField] GameObject inGameObjects;
    [SerializeField] NetworkManager networkManager;

    [SerializeField] List<GameObject> objects;


    public void setLooseScreen() {
        winLooseCanvas.SetActive(true);
        winLooseCanvasBackground.color = Color.red;
        winLooseCanvasText.text = "You loose";
        transform.GetComponent<AudioUnit>().startAudio(1); //1 ist loose
        mainMenu.onClick.AddListener(goMainMenu);
        spectateButton.onClick.AddListener(spectate);
    }

    public void spectate() {
        mainMenu.onClick.RemoveListener(goMainMenu);
        spectateButton.onClick.RemoveListener(spectate);

        foreach(GameObject o in objects) {
            o.SetActive(false);
        }
        
        winLooseCanvas.SetActive(false);
    }

    public void setWinScreen() {
        winLooseCanvas.SetActive(true);
        winLooseCanvasBackground.color = Color.green;
        winLooseCanvasText.text = "You win";
        transform.GetComponent<AudioUnit>().startAudio(0); // 0 ist win
        mainMenu.onClick.AddListener(goMainMenu);
        spectateButton.onClick.AddListener(spectate);
    }

    public void goMainMenu() {
        StartCoroutine(disconnectDelay());
    }

    IEnumerator disconnectDelay()
    {
        yield return new WaitForSeconds(0.001f);

        if (NetworkServer.active && NetworkClient.isConnected) {
            networkManager.StopHost();
        }else if (NetworkClient.isConnected) {
            networkManager.StopClient();
        }
    }
}
