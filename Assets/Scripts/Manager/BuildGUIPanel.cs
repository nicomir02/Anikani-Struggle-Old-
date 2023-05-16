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
    

    //Methode um Buildings Buy GUI zu generieren
    public void generateGUI(List<Ressource> ress, Vector3Int vec) {
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOn();

        GameObject.Find("InGame/Canvas/BuildingPanel").SetActive(true);

        GameObject.Find("InGame/Canvas/BuildingPanel/Close").GetComponent<Button>().onClick.AddListener(ClosePanel);

        int howMany = 0;

        TileBase tile = null;
        Sprite sprite = null;

        foreach (Ressource r in ress) {
            howMany = 0;
            if(r.ressName == "Wood") {
                selectedVector = vec;
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
    }

    public void buyBarracks() {
        Ressource wood = null;
        foreach(Ressource r in GameObject.Find("GameManager").GetComponent<MapBehaviour>().getAllRessourcen()) {
            if(r.ressName == "Wood") wood = r;
        }

        if(GetComponent<BuildingManager>().ressourcenZaehlerRechner(wood, priceWood)) {
            BuildingManager buildingManager = GetComponent<BuildingManager>();
            TilemapManager tilemapManager = GameObject.Find("GameManager").GetComponent<TilemapManager>();

            List<Vector3Int> vectors = buildingManager.makeAreaBigger(selectedVector, 1);

            buildingManager.addBuilding(vectors, GetComponent<Player>().eigenesVolk.getBarrackBuilding(0), selectedVector);
            tilemapManager.CmdUpdateTilemapBuilding(selectedVector, 3, GetComponent<Player>().id, GameObject.Find("GameManager").GetComponent<VolkManager>().getVolkID(GetComponent<Player>().eigenesVolk).Item2, 0);
        }
    }

    public void buyWood() {
        Ressource wood = null;
        foreach(Ressource r in ressourcen) {
            if(r.ressName == "Wood") wood = r;
        }

        if(GetComponent<BuildingManager>().ressourcenZaehlerRechner(wood, priceWood)) {
            GUIoff();
            GetComponent<BuildingManager>().OnBuildClick(wood, selectedVector);
        }
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
