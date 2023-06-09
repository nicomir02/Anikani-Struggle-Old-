using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinLooseScreen : MonoBehaviour
{
    [SerializeField] GameObject winLooseCanvas;
    [SerializeField] RawImage winLooseCanvasBackground;
    [SerializeField] TextMeshProUGUI winLooseCanvasText;
    [SerializeField] Button mainMenu;
    [SerializeField] Button spectateButton;
    [SerializeField] GameObject inGameObjects;


    public void setLooseScreen() {
        winLooseCanvas.SetActive(true);
        winLooseCanvasBackground.color = Color.red;
        winLooseCanvasText.text = "You loose";
        mainMenu.onClick.AddListener(goMainMenu);
        spectateButton.onClick.AddListener(spectate);
    }

    public void spectate() {
        mainMenu.onClick.RemoveListener(goMainMenu);
        spectateButton.onClick.RemoveListener(spectate);
        GameObject.Find("InGame/Canvas/Runde").SetActive(false);
        GameObject.Find("InGame/Canvas/ShowArea").SetActive(false);
        GameObject.Find("InGame/Canvas/Leiste/Tree").SetActive(false);
        GameObject.Find("InGame/Canvas/Leiste/Stone").SetActive(false);
        GameObject.Find("InGame/Canvas/Infobox").SetActive(false);
        GameObject.Find("InGame/Canvas/InfoboxBuilding").SetActive(false);
        GameObject.Find("InGame/Canvas/BuildOrBuy").SetActive(false);
        GameObject.Find("InGame/Canvas/BuildingPanel").SetActive(false);
        GameObject.Find("InGame/Canvas/TroopButton").SetActive(false);
        GameObject.Find("InGame/Canvas/UnitPanel").SetActive(false);
        winLooseCanvas.SetActive(false);
    }

    public void setWinScreen() {
        winLooseCanvas.SetActive(true);
        winLooseCanvasBackground.color = Color.green;
        winLooseCanvasText.text = "You win";
        mainMenu.onClick.AddListener(goMainMenu);
        spectateButton.onClick.AddListener(spectate);
    }

    public void goMainMenu() {
        GetComponent<PauseMenu>().buttonBackToMainMenu();
    }
}
