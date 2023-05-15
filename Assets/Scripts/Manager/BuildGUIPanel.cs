using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class BuildGUIPanel : MonoBehaviour
{
    [SerializeField] private List<Sprite> woodCutter = new List<Sprite>();
    [SerializeField] private List<Sprite> barracks = new List<Sprite>();

    private List<Ressource> ressourcen = new List<Ressource>();

    private Vector3Int selectedVector;

    public void generateGUI(List<Ressource> ress, Vector3Int vec) {
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOn();

        GameObject.Find("InGame/Canvas/BuildingPanel").SetActive(true);

        GameObject.Find("InGame/Canvas/BuildingPanel/Close").GetComponent<Button>().onClick.AddListener(ClosePanel);
        int zaehler = 0;

        foreach (Ressource r in ress) {
            if(r.ressName == "Wood") {
                selectedVector = vec;
                ressourcen.Add(r);
                GameObject.Find("InGame/Canvas/BuildingPanel/Wood").SetActive(true);
                GameObject.Find("InGame/Canvas/BuildingPanel/Wood/Button").GetComponent<Button>().onClick.AddListener(buyWood);
                GameObject.Find("InGame/Canvas/BuildingPanel/Wood/Background/Image").GetComponent<Image>().sprite = woodCutter[GetComponent<Player>().id-1];
                zaehler++;
            }
        }

        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks").SetActive(true);
        Debug.Log(GetComponent<Player>().id);
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Background/Image").GetComponent<Image>().sprite = barracks[GetComponent<Player>().id-1];
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Button").GetComponent<Button>().onClick.AddListener(buyBarracks);
    }

    public void buyBarracks() {

    }

    public void buyWood() {
        GUIoff();
        Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        Ressource wood = null;
        foreach(Ressource r in ressourcen) {
            if(r.ressName == "Wood") wood = r;
        }
        GetComponent<BuildingManager>().OnBuildClick(wood, selectedVector);
    }

    public void GUIoff() {
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOff();

        GameObject.Find("InGame/Canvas/BuildingPanel/Wood").SetActive(false);


        GameObject.Find("InGame/Canvas/BuildingPanel").SetActive(false);
    }

    public void ClosePanel() {
        GUIoff();
    }
}
