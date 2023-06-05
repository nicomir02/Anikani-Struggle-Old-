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
    
    private bool generated = false;

    public void generateGUI(Vector3Int vec) {
        generated = true;

        selectedVector = vec;

        GameObject.Find("InGame/Canvas/UnitPanel").SetActive(true);
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOn();
        GameObject.Find("GameManager").GetComponent<PauseMenu>().setCanPause(false);

        GameObject.Find("InGame/Canvas/UnitPanel/Close").GetComponent<Button>().onClick.AddListener(ClosePanel);

        //Melee Unit
        GameObject.Find("InGame/Canvas/UnitPanel/MeleeUnit").SetActive(true);
        Unit unit = GetComponent<Player>().eigenesVolk.getUnit(0);

        Sprite sprite = unit.getSprite(GameObject.Find("GameManager").GetComponent<RoundManager>().id);
        
        GameObject.Find("InGame/Canvas/UnitPanel/MeleeUnit/Button").GetComponent<Button>().onClick.AddListener(ButtonBuyMelee);
        GameObject.Find("InGame/Canvas/UnitPanel/MeleeUnit/Text").GetComponent<TextMeshProUGUI>().text = unit.getName() + "\n\n Price: "+ getPricing(unit)  + " Wood";
        GameObject.Find("InGame/Canvas/UnitPanel/MeleeUnit/Background/Image").GetComponent<Image>().sprite = sprite;

        unit = GetComponent<Player>().eigenesVolk.getUnit(1);
        sprite = unit.getSprite(GameObject.Find("GameManager").GetComponent<RoundManager>().id);

        GameObject.Find("InGame/Canvas/UnitPanel/SpecialUnit/Button").GetComponent<Button>().onClick.AddListener(ButtonBuySpecial);
        GameObject.Find("InGame/Canvas/UnitPanel/SpecialUnit/Text").GetComponent<TextMeshProUGUI>().text = unit.getName() + "\n\n Price: "+ getPricing(unit) + " Stone";
        GameObject.Find("InGame/Canvas/UnitPanel/SpecialUnit/Background/Image").GetComponent<Image>().sprite = sprite;

        //Welche Truppe wird gerade hier trainiert?
        if(howLong.ContainsKey(vec) && trainedUnits.ContainsKey(vec) && trainedUnits[vec] != null) {
            sprite = trainedUnits[vec].getSprite(GameObject.Find("GameManager").GetComponent<RoundManager>().id);
            
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

    public int getPricing(Unit u) {
        int m = getHowManyTroops(u);

        if(m > 20) {
            m = 20;
        }
        
        if(m > 0) {
            m--;
        }

        return m + u.getPrice();
    } 

    public int getHowManyTroops(Unit u) {
        int i = 0;
        foreach(KeyValuePair<Vector3Int, Unit> kvp in GetComponent<UnitManager>().spawnedUnits) {
            if(kvp.Value.getName() == u.getName()) i++;
        }
        return i;
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
                unitManager.spawnUnit(kvp.Value, vec, GameObject.Find("GameManager").GetComponent<RoundManager>().id-1);
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
        if(trainedUnits.ContainsKey(selectedVector) && trainedUnits[selectedVector] != null || !GameObject.Find("GameManager").GetComponent<RoundManager>().isYourTurn) return;

        BuildingManager buildingManager = GetComponent<BuildingManager>();
        MapBehaviour mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();

        Ressource ress = getRessource("Wood");
        
        Unit a = GetComponent<Player>().eigenesVolk.getUnit(0);

        if(buildingManager.ressourcenZaehlerRechner(ress, getPricing(a))) {
            
            trainedUnits.Add(selectedVector, a);
            howLong.Add(selectedVector, a.getHowManyTrainRounds());

            generateGUI(selectedVector);
        }
    }

    //Button Click Buy Special
    public void ButtonBuySpecial() {
        if(trainedUnits.ContainsKey(selectedVector) && trainedUnits[selectedVector] != null || !GameObject.Find("GameManager").GetComponent<RoundManager>().isYourTurn) return;

        BuildingManager buildingManager = GetComponent<BuildingManager>();
        MapBehaviour mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();

        Ressource ress = getRessource("Stone");
        
        Unit a = GetComponent<Player>().eigenesVolk.getUnit(1);
        int m = getHowManyTroops(a);

        if(m > 17) m = 17;

        if(buildingManager.ressourcenZaehlerRechner(ress, getPricing(a))) {
            
            trainedUnits.Add(selectedVector, a);
            howLong.Add(selectedVector, a.getHowManyTrainRounds());

            generateGUI(selectedVector);
        }
    }

    //Methode um GUI wieder auszumachen
    public void GUIoff() {
        generated = false;
        GameObject.Find("GameManager").GetComponent<PauseMenu>().setCanPause(true);
        GameObject.Find("GameManager").GetComponent<PauseMenu>().togglePauseOff();

        GameObject.Find("InGame/Canvas/UnitPanel/CurTrainedUnit").SetActive(false);


        GameObject.Find("InGame/Canvas/UnitPanel").SetActive(false);
    }

    void Update() {
        if(!generated) return;
        if(Input.GetKeyDown(KeyCode.Escape)) GUIoff();
    }

    //Methode Button Click Close GUI
    public void ClosePanel() {
        GUIoff();
    }

    //Hilfsmethode um Ressourcenkosten zu kriegen
    public Ressource getRessource(string name) {
        foreach(Ressource r in GameObject.Find("GameManager").GetComponent<MapBehaviour>().getAllRessourcen()) {
            if(r.ressName == name) return r;
        }
        return null;
    }
}