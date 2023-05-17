using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

public class UnitGUIPanel : MonoBehaviour
{

    private Dictionary<Vector3Int, Unit> trainedUnits = new Dictionary<Vector3Int, Unit>();
    private Dictionary<Vector3Int, int> howLong = new Dictionary<Vector3Int, int>();
    
    private Vector3Int selectedVector;

    public void generateGUI(Vector3Int vec) {

        selectedVector = vec;

        GameObject.Find("InGame/Canvas/UnitPanel").SetActive(true);
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOn();
        GameObject.Find("GameManager").GetComponent<PauseMenu>().setCanPause(false);

        GameObject.Find("InGame/Canvas/UnitPanel/Close").GetComponent<Button>().onClick.AddListener(ClosePanel);

        //Melee Unit
        GameObject.Find("InGame/Canvas/UnitPanel/MeleeUnit").SetActive(true);
        Unit meleeUnit = GetComponent<Player>().eigenesVolk.getUnit(0);
        TileBase tile = meleeUnit.getTile(GetComponent<Player>().id-1);
        Sprite sprite = null;
        if (tile != null && tile is Tile tileComponents)
        {
            sprite = tileComponents.sprite;
        }
        GameObject.Find("InGame/Canvas/UnitPanel/MeleeUnit/Button").GetComponent<Button>().onClick.AddListener(ButtonBuyMelee);
        GameObject.Find("InGame/Canvas/UnitPanel/MeleeUnit/Text").GetComponent<TextMeshProUGUI>().text = meleeUnit.getName() + "\n\n Price: "+meleeUnit.getPrice() + " Wood";
        GameObject.Find("InGame/Canvas/UnitPanel/MeleeUnit/Background/Image").GetComponent<Image>().sprite = sprite;

        //Welche Truppe wird gerade hier trainiert?
        if(howLong.ContainsKey(vec) && trainedUnits.ContainsKey(vec) && trainedUnits[vec] != null) {
            tile = trainedUnits[vec].getTile(GetComponent<Player>().id-1);
            sprite = null;
            if (tile != null && tile is Tile tileComponent)
            {
                sprite = tileComponent.sprite;
            }
            GameObject.Find("InGame/Canvas/UnitPanel/CurTrainedUnit/Background/Image").GetComponent<Image>().sprite = sprite;
            GameObject.Find("InGame/Canvas/UnitPanel/CurTrainedUnit").SetActive(true);
            GameObject.Find("InGame/Canvas/UnitPanel/CurTrainedUnit/Text").GetComponent<TextMeshProUGUI>().text = "Currently training:\n" + trainedUnits[vec].getName();
            if(howLong[vec] > 0) {
                GameObject.Find("InGame/Canvas/UnitPanel/CurTrainedUnit/BG/Text").GetComponent<TextMeshProUGUI>().text = howLong[vec] + " Round";
            }else {
                GameObject.Find("InGame/Canvas/UnitPanel/CurTrainedUnit/BG/Text").GetComponent<TextMeshProUGUI>().text = "Cannot spawn";
            }
            

            if(howLong[vec] > 0) {
                GameObject.Find("InGame/Canvas/UnitPanel/CurTrainedUnit/BG/Text").GetComponent<TextMeshProUGUI>().text += "s";
            }
        }

        
    }

    //Bei Rundenanfang aufgerufene Methode
    //Soll Rundenanzahl für trainierte Units verringern
    //Soll wenn Rundenanzahl für trainierte Units bei null ist Unit spawnen lassen
    public void auffuellen() {
        UnitManager unitManager = GetComponent<UnitManager>();
        Player player = GetComponent<Player>();

        List<Vector3Int> removeTemp = new List<Vector3Int>();

        foreach(KeyValuePair<Vector3Int, Unit> kvp in trainedUnits) {
            howLong[kvp.Key] -= 1;
            Vector3Int vec = new Vector3Int(kvp.Key.x, kvp.Key.y, 2);
            if(howLong[kvp.Key] <= 0 && !unitManager.hasUnitOnVec(vec)) {
                unitManager.spawnUnit(kvp.Value, vec, player.id-1);
                removeTemp.Add(kvp.Key);
            }
        }

        foreach(Vector3Int v in removeTemp) {
            howLong.Remove(v);
            trainedUnits.Remove(v);
        }
    }

    //Button Click Buy Melee
    public void ButtonBuyMelee() {
        if(trainedUnits.ContainsKey(selectedVector) && trainedUnits[selectedVector] != null) return;

        BuildingManager buildingManager = GetComponent<BuildingManager>();
        MapBehaviour mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();

        Ressource ress = null;
        foreach(Ressource r in mapBehaviour.getAllRessourcen()) {
            if(r.ressName == "Wood") {
                ress = r;
            }
        }
        Unit a = GetComponent<Player>().eigenesVolk.getUnit(0);
        if(buildingManager.ressourcenZaehlerRechner(ress, a.getPrice())) {
            
            trainedUnits.Add(selectedVector, a);
            howLong.Add(selectedVector, a.getHowManyTrainRounds());

            generateGUI(selectedVector);
        }
    }

    //Methode um GUI wieder auszumachen
    public void GUIoff() {
        GameObject.Find("GameManager").GetComponent<PauseMenu>().setCanPause(true);
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOff();

        GameObject.Find("InGame/Canvas/UnitPanel/CurTrainedUnit").SetActive(false);


        GameObject.Find("InGame/Canvas/UnitPanel").SetActive(false);
    }

    //Methode Button Click Close GUI
    public void ClosePanel() {
        GUIoff();
    }

}