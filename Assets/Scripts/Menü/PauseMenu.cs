using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using System.Threading;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject Pause;
    [SerializeField] private Button weiter;
    [SerializeField] private Button backToMainMenu;

    private Player player;

    bool isPaused = false;

    bool canPause = true;

    public bool getPause() {
        return isPaused;
    }

    // Start is called before the first frame update
    void Start()
    {
        Pause.SetActive(false);

        weiter.onClick.AddListener(buttonWeiter);
        backToMainMenu.onClick.AddListener(buttonBackToMainMenu);
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
        //
        //Spiel muss im Hintergrund noch beendeet werden 
        
        
        NetworkClient.localPlayer.GetComponent<BuildingManager>().disqualifyPlayer();
        
        
        SceneManager.LoadScene(0);
        
        Thread delayThread = new Thread(disconnect);
        delayThread.Start();
        
    }

    public void disconnect(){
        Thread.Sleep(500);
        NetworkManager.singleton.StopClient();
        
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
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume() 
    {
        Pause.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    // public void zuruckZumStartmenü()
    // {
    //     SceneManager.LoadScene(0);
        
    // }
}
