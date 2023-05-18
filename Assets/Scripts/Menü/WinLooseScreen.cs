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

    public void setLooseScreen() {
        winLooseCanvas.SetActive(true);
        winLooseCanvasBackground.color = Color.red;
        winLooseCanvasText.text = "You loose";
        //mainMenu.onClick.AddListener(goMainMenu);
    }

    public void setWinScreen() {
        winLooseCanvas.SetActive(true);
        winLooseCanvasBackground.color = Color.green;
        winLooseCanvasText.text = "You win";
        //mainMenu.onClick.AddListener(goMainMenu);
    }

    public void goMainMenu() {
        SceneManager.LoadScene(0);
    }
}
