using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.EventSystems;

public class BuildGUIPanel : MonoBehaviour
{

    private List<Ressource> ressourcen = new List<Ressource>();

    private Vector3Int selectedVector;
    private Dictionary<Ressource, (Ressource, int)> priceRessource = new Dictionary<Ressource, (Ressource, int)>();
    private int priceBarracks = 0;
    private int priceArea = 1;

    private bool guiOn = false;


    

    //Methode um Buildings Buy GUI zu generieren
    public void generateGUI(List<Ressource> ress, Vector3Int vec) {
        guiOn = true;

        GameObject.Find("GameManager").GetComponent<PauseMenu>().setCanPause(false);

        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOn();

        GameObject.Find("InGame/Canvas/BuildingPanel").SetActive(true);

        GameObject.Find("InGame/Canvas/BuildingPanel/Close").GetComponent<Button>().onClick.AddListener(ClosePanel);

        selectedVector = vec;

        int howMany = 0;
        Sprite sprite = null;
    
        foreach (Ressource r in ress) {
            howMany = 0;

            if(GameObject.Find("InGame/Canvas/BuildingPanel/"+r.ressName) != null) {
                ressourcen.Add(r);
                GameObject.Find("InGame/Canvas/BuildingPanel/"+r.ressName).SetActive(true);

                Building building = GetComponent<Player>().eigenesVolk.getRessourceBuilding(r, 0);

                AnimatedTile animatedtile = (AnimatedTile) building.getTile(GameObject.Find("GameManager").GetComponent<RoundManager>().id-1);
                sprite = animatedtile.m_AnimatedSprites[0];

                

                if(!priceRessource.ContainsKey(r)) {
                    priceRessource.Add(r, (r, 0));
                }else {
                    for(int i=0; i<GetComponent<Player>().eigenesVolk.getBuildings(1); i++) {
                        howMany += GetComponent<BuildingManager>().howManyBuildings(building);
                    }
                    priceRessource[r] = (r, 1*howMany);
                }

                GameObject.Find("InGame/Canvas/BuildingPanel/"+r.ressName+"/Text").GetComponent<TextMeshProUGUI>().text = building.getName() + "\n\nPrice: " + priceRessource[r].Item2 + " "+ priceRessource[r].Item1.ressName;

                GameObject.Find("InGame/Canvas/BuildingPanel/"+r.ressName+"/Background/Image").GetComponent<Image>().sprite = sprite;
                
                GameObject.Find("InGame/Canvas/BuildingPanel/"+r.ressName+"/"+r.ressName+"Button").GetComponent<Button>().onClick.AddListener(buy);
            }
        }

        howMany = 0;

        for(int i=0; i<GetComponent<Player>().eigenesVolk.getBuildings(2); i++) {
            howMany += GetComponent<BuildingManager>().howManyBuildings(GetComponent<Player>().eigenesVolk.getBarrackBuilding(i));
        }

        AnimatedTile animated = (AnimatedTile) GetComponent<Player>().eigenesVolk.getBarrackBuilding(0).getTile(GameObject.Find("GameManager").GetComponent<RoundManager>().id-1);
        sprite = animated.m_AnimatedSprites[0];

        priceBarracks = howMany*2;
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Text").GetComponent<TextMeshProUGUI>().text = "Barracks\n\nPrice: " + priceBarracks + " Wood";

        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks").SetActive(true);
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Background/Image").GetComponent<Image>().sprite = sprite;
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Button").GetComponent<Button>().onClick.AddListener(buyBarracks);

        GameObject.Find("InGame/Canvas/BuildingPanel/AreaExtension").SetActive(true);
        GameObject.Find("InGame/Canvas/BuildingPanel/AreaExtension/Button").GetComponent<Button>().onClick.AddListener(buyAreaExtension);
        GameObject.Find("InGame/Canvas/BuildingPanel/AreaExtension/Text").GetComponent<TextMeshProUGUI>().text = "Area Extension\n\n4x4\n\nPrice: " + priceArea + " Wood";
    }

    //damit durch Esc auch GUI geschlossen werden kann
     void Update() {
        if(!guiOn) return;
        if(Input.GetKeyDown(KeyCode.Escape)) GUIoff();
    }


    //Buy Ressource Methode, generisch für alle Ressourcen
    public void buy() {
        Ressource ressource = getRessource(EventSystem.current.currentSelectedGameObject.name.Replace("Button", ""));

        if(GetComponent<BuildingManager>().ressourcenZaehlerRechner(priceRessource[ressource].Item1, priceRessource[ressource].Item2)) {
            GameObject.Find("InGame/Canvas/BuildingPanel/Wood").SetActive(false);
            GUIoff();
            GetComponent<BuildingManager>().OnBuildClick(ressource, selectedVector);
        }
    }
    
    //Area Extension;
    public void buyAreaExtension() {
        Ressource ressource = getRessource("Wood");
        if(GetComponent<BuildingManager>().ressourcenZaehlerRechner(ressource, priceArea)) {
           
            BuildingManager buildingManager = GetComponent<BuildingManager>();

            buildingManager.addFelderToTeam(selectedVector, 3, GameObject.Find("GameManager").GetComponent<RoundManager>().id); //Felder zum Team hinzufügen
            if (priceArea < 10) priceArea++; //max Preis für Area soll 10 Wood momentan sein

            buildingManager.reloadShowArea();
            GUIoff();
        }
    }

   

    //Methode Button Click Barracks Bau
    public void buyBarracks() {
        Ressource ressource = getRessource("Wood");

        if(GetComponent<BuildingManager>().ressourcenZaehlerRechner(ressource, priceBarracks)) {
            
            GameObject.Find("InGame/Canvas/BuildingPanel/Barracks").SetActive(false);
            BuildingManager buildingManager = GetComponent<BuildingManager>();
            TilemapManager tilemapManager = GameObject.Find("GameManager").GetComponent<TilemapManager>();

            List<Vector3Int> vectors = buildingManager.makeAreaBigger(selectedVector, 1);
            
            buildingManager.deleteFelder(selectedVector, 3, null);

            buildingManager.buildInRoundZaehlerInkrement();

            buildingManager.addBuilding(vectors, GetComponent<Player>().eigenesVolk.getBarrackBuilding(0), selectedVector);
            tilemapManager.CmdUpdateTilemapBuilding(new Vector3Int(selectedVector.x, selectedVector.y, 1), 3, GameObject.Find("GameManager").GetComponent<RoundManager>().id, GameObject.Find("GameManager").GetComponent<VolkManager>().getVolkID(GetComponent<Player>().eigenesVolk).Item2, 0);

            buildingManager.addFelderToTeam(selectedVector, 4, GameObject.Find("GameManager").GetComponent<RoundManager>().id);

            GUIoff();
        }
    }

    //Methode um GUI wieder auszumachen
    public void GUIoff() {
        
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Button").GetComponent<Button>().onClick.RemoveListener(buyBarracks);
        GameObject.Find("InGame/Canvas/BuildingPanel/AreaExtension/Button").GetComponent<Button>().onClick.RemoveListener(buyAreaExtension);

        GameObject.Find("GameManager").GetComponent<PauseMenu>().setCanPause(true);
        guiOn = false;
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOff();


        //Alle Objects wie inaktiv setzen
        foreach(Ressource r in ressourcen) {
            GameObject.Find("InGame/Canvas/BuildingPanel/" + r.ressName).SetActive(false);
            GameObject.Find("InGame/Canvas/BuildingPanel/" + r.ressName +"/" + r.ressName +"Button").GetComponent<Button>().onClick.RemoveListener(buy);
        }
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks").SetActive(false);


        GameObject.Find("InGame/Canvas/BuildingPanel").SetActive(false);
    }

    //Methode Button Click Close GUI
    public void ClosePanel() {
        GUIoff();
    }

    public bool getGUI() {
        return guiOn;
    }

    //Hilfsmethode um Ressourcenkosten zu kriegen
     public Ressource getRessource(string name) {
        foreach(Ressource r in GameObject.Find("GameManager").GetComponent<MapBehaviour>().getAllRessourcen()) {
            if(r.ressName == name) return r;
        }
        return null;
    }
}