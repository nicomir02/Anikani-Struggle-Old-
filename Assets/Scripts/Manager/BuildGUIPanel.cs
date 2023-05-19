using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

public class BuildGUIPanel : MonoBehaviour
{

    private List<Ressource> ressourcen = new List<Ressource>();

    private Vector3Int selectedVector;
    private int priceWood = 0;
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

        TileBase tile = null;
        Sprite sprite = null;

        foreach (Ressource r in ress) {
            howMany = 0;
            if(r.ressName == "Wood") {
                ressourcen.Add(r);
                GameObject.Find("InGame/Canvas/BuildingPanel/Wood").SetActive(true);
                GameObject.Find("InGame/Canvas/BuildingPanel/Wood/Button").GetComponent<Button>().onClick.AddListener(buyWood);
                tile = GetComponent<Player>().eigenesVolk.getTreeBuilding(0).getTile(GetComponent<Player>().id-1);
                sprite = null;
                if (tile != null && tile is Tile tileComponents)
                {
                    sprite = tileComponents.sprite;
                }

                GameObject.Find("InGame/Canvas/BuildingPanel/Wood/Background/Image").GetComponent<Image>().sprite = sprite; //GetComponent<Player>().eigenesVolk.getTreeBuilding(0).getTile(GetComponent<Player>().id).sprite;
                

                for(int i=0; i<GetComponent<Player>().eigenesVolk.getBuildings(1); i++) {
                    howMany += GetComponent<BuildingManager>().howManyBuildings(GetComponent<Player>().eigenesVolk.getTreeBuilding(i));
                }

                priceWood = howMany*2;
                GameObject.Find("InGame/Canvas/BuildingPanel/Wood/Text").GetComponent<TextMeshProUGUI>().text = "Woodcutter\n\nPrice: " + priceWood + " Wood";
            }
        }

        howMany = 0;

        for(int i=0; i<GetComponent<Player>().eigenesVolk.getBuildings(2); i++) {
            howMany += GetComponent<BuildingManager>().howManyBuildings(GetComponent<Player>().eigenesVolk.getBarrackBuilding(i));
        }

        tile = GetComponent<Player>().eigenesVolk.getBarrackBuilding(0).getTile(GetComponent<Player>().id-1);
        sprite = null;
        if (tile != null && tile is Tile tileComponent)
        {
            sprite = tileComponent.sprite;
        }

        priceBarracks = howMany*2;
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Text").GetComponent<TextMeshProUGUI>().text = "Barracks\n\nPrice: " + priceBarracks + " Wood";

        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks").SetActive(true);
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Background/Image").GetComponent<Image>().sprite = sprite;
        GameObject.Find("InGame/Canvas/BuildingPanel/Barracks/Button").GetComponent<Button>().onClick.AddListener(buyBarracks);

        GameObject.Find("InGame/Canvas/BuildingPanel/AreaExtension").SetActive(true);
        GameObject.Find("InGame/Canvas/BuildingPanel/AreaExtension/Button").GetComponent<Button>().onClick.AddListener(buyAreaExtension);
        GameObject.Find("InGame/Canvas/BuildingPanel/AreaExtension/Text").GetComponent<TextMeshProUGUI>().text = "Area Extension\n\n4x4\n\nPrice: " + priceArea + " Wood";
    }
    
    //Area Extension;
    public void buyAreaExtension() {
        Ressource ressource = getRessource("Wood");

        if(GetComponent<BuildingManager>().ressourcenZaehlerRechner(ressource, priceArea)) {
            BuildingManager buildingManager = GetComponent<BuildingManager>();

            buildingManager.addFelderToTeam(selectedVector, 3, GetComponent<Player>().id); //Felder zum Team hinzufügen
            if (priceArea < 10) priceArea++; //max Preis für Area soll 10 Wood momentan sein

            buildingManager.reloadShowArea();
            GUIoff();
        }
    }

   

    //Methode Button Click Barracks Bau
    public void buyBarracks() {
        Ressource ressource = getRessource("Wood");

        if(GetComponent<BuildingManager>().ressourcenZaehlerRechner(ressource, priceBarracks)) {
            BuildingManager buildingManager = GetComponent<BuildingManager>();
            TilemapManager tilemapManager = GameObject.Find("GameManager").GetComponent<TilemapManager>();

            List<Vector3Int> vectors = buildingManager.makeAreaBigger(selectedVector, 1);
            
            buildingManager.deleteFelder(selectedVector, 3, null);

            buildingManager.buildInRoundZaehlerInkrement();

            buildingManager.addBuilding(vectors, GetComponent<Player>().eigenesVolk.getBarrackBuilding(0), selectedVector);
            tilemapManager.CmdUpdateTilemapBuilding(new Vector3Int(selectedVector.x, selectedVector.y, 1), 3, GetComponent<Player>().id, GameObject.Find("GameManager").GetComponent<VolkManager>().getVolkID(GetComponent<Player>().eigenesVolk).Item2, 0);

            buildingManager.addFelderToTeam(selectedVector, 4, GetComponent<Player>().id);

            buildingManager.reloadShowArea();

            GUIoff();
        }
    }

    //Methode Button Click Holz Bau
    public void buyWood() {
        Ressource ressource = getRessource("Wood");

        if(GetComponent<BuildingManager>().ressourcenZaehlerRechner(ressource, priceWood)) {
            GUIoff();
            GetComponent<BuildingManager>().OnBuildClick(ressource, selectedVector);
        }
    }

    //Methode um GUI wieder auszumachen
    public void GUIoff() {
        GameObject.Find("GameManager").GetComponent<PauseMenu>().setCanPause(true);
        guiOn = false;
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOff();

        GameObject.Find("InGame/Canvas/BuildingPanel/Wood").SetActive(false);
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
