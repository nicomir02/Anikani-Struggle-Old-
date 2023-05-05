using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartMYGame() {
        SceneManager.LoadScene(1);
    }

    public void CloseMYGame() {
        Application.Quit();
    }       
}
