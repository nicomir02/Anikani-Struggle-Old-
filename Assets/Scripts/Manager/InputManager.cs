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
    private bool showUnits = true;
    private bool showHealthbar = true;

    private MapBehaviour mapBehaviour;
    private HealthManager healthManager;
    //private BuildingManager buildingManager;

    [SerializeField] private Tilemap tilemap;

    [SerializeField] private Color durchsichtig;
    [SerializeField] private Color unsichtbar;

    [SerializeField] private GameObject tabPanel;
    [SerializeField] private TextMeshProUGUI playernameText;
    

    void Start() {
        mapBehaviour = GetComponent<MapBehaviour>();
        healthManager = GetComponent<HealthManager>();
        //buildingManager = GetComponent<BuildingManager>();

    }


    void Update()
    {
        if(Input.GetButtonDown("Toggle invisible Buildings")) {
            onShowBuildings();
        }

        if(Input.GetButtonDown("Toggle invisible Units")) {
            onShowUnits();
        }

        if(Input.GetButtonDown("Toggle healthbar")) {
            onShowHealthbar();
        }

        if(Input.GetButtonDown("Show List")) {
            showTabNames(true);
        }else if(Input.GetButtonUp("Show List")) {
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

    void onShowUnits() {
        foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
            if(showUnits) {
                //Unit
                us.GetComponent<SpriteRenderer>().color = durchsichtig;

                Vector3 v = us.GetComponent<Transform>().GetChild(0).position;
                v.z = -100f;
                us.GetComponent<Transform>().GetChild(0).position = v;
            }else {
                //Unit
                us.GetComponent<SpriteRenderer>().color = Color.white;
                
                //Healthbar
                if(showHealthbar) {
                    Vector3 v = us.GetComponent<Transform>().GetChild(0).position;
                    v.z = 0f;
                    us.GetComponent<Transform>().GetChild(0).position = v;
                }
            }
        }
        showUnits = !showUnits;
    }

    void onShowHealthbar() {
        foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
            if(showHealthbar) {
                Vector3 v = us.GetComponent<Transform>().GetChild(0).position;
                v.z = -100f;
                us.GetComponent<Transform>().GetChild(0).position = v;
            }else {
                Vector3 v = us.GetComponent<Transform>().GetChild(0).position;
                v.z = 0f;
                us.GetComponent<Transform>().GetChild(0).position = v;
            }
        }
        showHealthbar = !showHealthbar;
    }
}
