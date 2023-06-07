using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Threading;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject Pause;
    [SerializeField] private Button weiter;
    [SerializeField] private Button backToMainMenu;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject gameManager;

    bool isPaused = false;

    bool canPause = true;

    public bool getPause() {
        return isPaused;
    }

    // Start is called before the first frame update
    void Start()
    {
        Pause.SetActive(false);
    }

    public void setCanPause(bool can) {
        canPause = can;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && canPause) 
        {
            if(isPaused)
            {
                Resume();
            } 
            else
            {
                PauseGame();
            } 
        }
    }

    public void buttonWeiter() {
        Resume();
    }

    public void buttonBackToMainMenu() {
        NetworkClient.localPlayer.GetComponent<BuildingManager>().disqualifyPlayer();
        //NetworkClient.localPlayer.GetComponent<UnitManager>().deleteAllUnits();
        StartCoroutine(disconnectDelay());

        gameManager.SetActive(true);
    }

//benötigter delay, damit alles fertig geladen ist
    IEnumerator disconnectDelay()
    {
        yield return new WaitForSeconds(0.001f);

        if (NetworkServer.active && NetworkClient.isConnected) {
            networkManager.StopHost();
        }else if (NetworkClient.isConnected) {
            networkManager.StopClient();
        }
        
            
    }

//Setter für isPaused Variable
    public void togglePauseOn() {
        isPaused = true;
    }

//Setter für isPaused Variable
    public void togglePauseOff() {
        isPaused = false;
    }

    public void PauseGame()
    {  
        Pause.SetActive(true);
        //Time.timeScale = 0f; wegen IEnumerator
        isPaused = true;

        weiter.onClick.AddListener(buttonWeiter);
        backToMainMenu.onClick.AddListener(buttonBackToMainMenu);
    }

    public void Resume() 
    {
        Pause.SetActive(false);
        //Time.timeScale = 1; wegen IEnumerator
        isPaused = false;

        weiter.onClick.RemoveListener(buttonWeiter);
        backToMainMenu.onClick.RemoveListener(buttonBackToMainMenu);
    }

    // public void zuruckZumStartmenü()
    // {
    //     SceneManager.LoadScene(0);
        
    // }
}
