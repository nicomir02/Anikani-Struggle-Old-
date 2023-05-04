using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Pause;
    bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        Pause.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
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

    public void zuruckZumStartmenü()
    {
        SceneManager.LoadScene(0);
        //Spiel muss im Hintergrund noch beendeet werden 
    }
}
