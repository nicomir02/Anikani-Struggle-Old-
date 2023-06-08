using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    //Variable ob Gebäude angezeigt werden:
    private bool showBuildings = true;

    private MapBehaviour mapBehaviour;
    private HealthManager healthManager;

    [SerializeField] private Tilemap tilemap;

    [SerializeField] private Color durchsichtig;

    [SerializeField] private GameObject tabPanel;
    [SerializeField] private TextMeshProUGUI playernameText;
    

    void Start() {
        mapBehaviour = GetComponent<MapBehaviour>();
        healthManager = GetComponent<HealthManager>();
    }


    void Update()
    {

        if(Input.GetKeyDown(KeyCode.B)) {
            onShowBuildings();
        }

        if(Input.GetKeyDown(KeyCode.Tab)) {
            
            showTabNames(true);
        }else if(Input.GetKeyUp(KeyCode.Tab)) {
            showTabNames(false);
        }
    }

    void showTabNames(bool show) {
        string result = "";
        foreach(KeyValuePair<int, string> kvp in GetComponent<GameManager>().playernames) {
            result += "<color=#"+ ColorUtility.ToHtmlStringRGB(GetComponent<GameManager>().spielFarben[kvp.Key-1]) +">"+kvp.Value + "</color>\n";
        }
        playernameText.text = result;
        
        tabPanel.SetActive(show);
    }

    void onShowBuildings() {
        int height = mapBehaviour.mapHeight();
        int width = mapBehaviour.mapWidth();

        if(showBuildings) {
            Vector3Int vec;
            for(int x=0; x<width; x++) {
                for(int y=0; y<height; y++) {
                    vec = new Vector3Int(x, y, 1);
                    if(healthManager.isBuilding(vec)) {
                        tilemap.SetTileFlags(vec, TileFlags.None);
                        tilemap.SetColor(vec, durchsichtig);
                    }
                }
            }
            showBuildings = false;
        }else {
            Vector3Int vec;
            for(int x=0; x<width; x++) {
                for(int y=0; y<height; y++) {
                    vec = new Vector3Int(x, y, 1);
                    if(healthManager.isBuilding(vec)) {
                        tilemap.SetTileFlags(vec, TileFlags.None);
                        tilemap.SetColor(vec, Color.white);
                    }
                }
            }
            showBuildings = true;
        }
    }
}
