using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Handbuch : MonoBehaviour
{
    [SerializeField] private GameObject fullpanel;
    [SerializeField] private GameObject hotkeypanel;
    [SerializeField] private GameObject tribepanel;

    [SerializeField] private Button close;
    [SerializeField] private Button start;

    [SerializeField] private Button hotkeyButton;
    [SerializeField] private Button tribeButton;

    // Start is called before the first frame update
    void Start()
    {
        close.onClick.AddListener(onClose);
        start.onClick.AddListener(onStartPanel);
        hotkeyButton.onClick.AddListener(hotkeyPanelStart);
        tribeButton.onClick.AddListener(tribePanelStart);
    }

    void hotkeyPanelStart() {
        closePanels();
        hotkeypanel.SetActive(true);
    }

    void tribePanelStart() {
        closePanels();
        tribepanel.SetActive(true);
    }

    void onStartPanel() {
        fullpanel.SetActive(true);
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOn();
    }

    //Schließt alles
    void onClose() {
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOff();
        closePanels();
        fullpanel.SetActive(false);
    }

    //Schließt alle Untermenü Panels
    void closePanels() {
        hotkeypanel.SetActive(false);
        tribepanel.SetActive(false);
    }
}
