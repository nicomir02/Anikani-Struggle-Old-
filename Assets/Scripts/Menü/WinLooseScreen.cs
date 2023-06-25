using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;
using System.Threading.Tasks;

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
        warten();
        winLooseCanvas.SetActive(true);
        winLooseCanvasBackground.color = Color.red;
        winLooseCanvasText.text = "You loose";
        mainMenu.onClick.AddListener(goMainMenu);
        spectateButton.onClick.AddListener(spectate);
        transform.GetComponent<winLoseAudio>().startAudio(1); //1 ist loose

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
        warten();
        winLooseCanvas.SetActive(true);
        winLooseCanvasBackground.color = Color.green;
        winLooseCanvasText.text = "You win";
        mainMenu.onClick.AddListener(goMainMenu);
        spectateButton.onClick.AddListener(spectate);
        transform.GetComponent<winLoseAudio>().startAudio(0); // 0 ist win
    }

    async void warten() {
        await Task.Delay(2000);
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
