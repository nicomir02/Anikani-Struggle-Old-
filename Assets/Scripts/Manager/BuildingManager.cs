using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using Mirror;
using UnityEngine.EventSystems;

public class BuildingManager : NetworkBehaviour
{
    //Deklarieren von benötigten Klassen
    private Volk volk; //Eigenes Volk
    private Player player; //Spielerscript
    private TilemapHover hover; //abfragung, wo mauszeiger ist
    private Tilemap tilemap; //Tilemap abfragen
    private TilemapManager tilemapManager; //änderungen auf tilemap
    private VolkManager volkManager; //volk id herausfinden
    private MapBehaviour mapBehaviour; //Block details herausfinden, biome etc.
    private HealthManager healthManager; //healthmanager für gebäudeleben 

    private GameManager gameManager;
    private UnitManager unitManager;    //für testzwecke der ersten einheit

    private PauseMenu pauseMenu;

    private Button showArea;

    private bool showAreaBool = false;
//isLobby wird nur benutzt bei der Spielinitialisierung in der Update Methode
    private bool isLobby = true; 

    private int maxBuildingPerRound = 1;
    //ZaehlerBuildingsbuiltInRound zählt die Anzahl an Gebäuden die in der Runde gebaut wurden(eventuell Limit setzen später)
    private int ZaehlerBuildingsBuiltInRound = 0;

    //was für ein Building an diesem Vektor sich befindet(jedes bUilding einmal in dem Dictionary)
    private Dictionary<Vector3Int, Building> buildingsVec = new Dictionary<Vector3Int, Building>();
    //da manche Buildings größer als 1 Feld sind muss man alle besetzten Felder
    //hier auflisten um mit buildingsvex zu korrespondieren
    private Dictionary<Vector3Int, Vector3Int> buildingvectors = new Dictionary<Vector3Int, Vector3Int>();
    //zeigt wie viel mehr Ressourcen pro Runde dazu kommen
    private Dictionary<Ressource, int> ressourcenProRundeZaehler = new Dictionary<Ressource, int>();
    //zeigt gesamtmenge an Ressourcen die man hat:
    private Dictionary<Ressource, int> ressourcenZaehler = new Dictionary<Ressource, int>();

    //Knopf um auszuwählen welches Gebäude man platzieren will
    //Button ressourcenButton;
    
    //ausgewählter/angeklickter Vektor
    Vector3Int selectedVector;
    //ausgewähltes/angeklicktes Gebäude
    Building selectedBuilding;
    //Farbe
    Color oldSelectedColor;
    //gibts einen selected Vektor für Feld Erweiterung
    bool selection = false;

    //methode angriffgebäude schaut ob das angegriffene Gebäude einem Spieler selber gehört
    [ClientRpc]
    public void angriffsCheckBuilding(Vector3Int vec) {
        vec.z = 1;
        
        if(buildingvectors.ContainsKey(vec) && healthManager.getBuildingLeben(vec) <= 0) {
            vec = buildingvectors[vec];
            Debug.Log("1");
            Vector3Int temp = buildingvectors[vec];
            if(buildingsVec[vec] == volk.getHomeBuilding(0)) {
                Debug.Log("Game Over"); //Später mit einem Canvas auf dem Bildschirm des Spielers 
                player.spielerDisqualifizieren(player.id);                Vector3Int tmp = new Vector3Int(vec.x, vec.y, 1);
                tilemapManager.removeBuilding(temp, 3);

                foreach(KeyValuePair<Vector3Int, Building> kvp in buildingsVec) {
                    healthManager.removeBuilding(kvp.Key);
                    tilemapManager.removeBuilding(kvp.Key, 3);
                }

                buildingsVec = new Dictionary<Vector3Int, Building>();
                buildingvectors = new Dictionary<Vector3Int, Vector3Int>();

                GameObject.Find("GameManager").GetComponent<WinLooseScreen>().setLooseScreen();

                unitManager.disqualify();
            }else {
                tilemapManager.removeBuilding(temp, 2);
                healthManager.removeBuilding(vec);

                //Remove Building von Listen
                
                buildingsVec.Remove(temp);
                List<Vector3Int> removeList = new List<Vector3Int>();
                foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in buildingvectors) {
                    if(kvp.Value == temp) removeList.Add(kvp.Key);
                }

                if(removeList.Contains(selectedVector)) {
                    deselectBuilding();
                }


                foreach(Vector3Int v in removeList) {
                    buildingvectors.Remove(v);
                }
            }
        }
        reloadShowArea();
    }

    //Getter Methode für ZaehlerBuildingsBuiltInRound
    public int getZahlBuildInRound() {
        return ZaehlerBuildingsBuiltInRound;
    }

//Build Inkrement wenn gebaut wird
    public void buildInRoundZaehlerInkrement() {
        ZaehlerBuildingsBuiltInRound++;
    }

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hover = GameObject.Find("GameManager").GetComponent<TilemapHover>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        player = GetComponent<Player>();
        volk = player.eigenesVolk;
        tilemapManager = GameObject.Find("GameManager").GetComponent<TilemapManager>();
        volkManager = GameObject.Find("GameManager").GetComponent<VolkManager>();
        mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
        healthManager = GameObject.Find("GameManager").GetComponent<HealthManager>();
        pauseMenu = GameObject.Find("GameManager").GetComponent<PauseMenu>();
        unitManager = GetComponent<UnitManager>();//für testzwecke der ersten einheit
    }

    void Update() {
        //Pause deaktivieren
        if(pauseMenu.getPause()) return;

        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject != null) return;

        //Initialisieren des ShowArea GameObjects nach Spielstart(nur einmal!)
        if(GameObject.Find("InGame/Canvas/ShowArea") != null && isLobby) {
            GameObject.Find("InGame/Canvas/ShowArea").GetComponent<Button>().onClick.AddListener(OnShowAreaClick);
            isLobby = false;
        }
        //Hauptgebäude aufgebaut nach Spielstart(nur einmal)
        if(Input.GetMouseButtonDown(0) && player.isYourTurn && !player.isLobby) {
            Vector3Int vec = hover.getVectorFromMouse();
            if(hover.insideField(vec)) {
                if(player.round == 0 && ZaehlerBuildingsBuiltInRound == 0) {
                    bool canBuild = canBuildMethod(vec);
                    List<Vector3Int> newArea = makeAreaBigger(vec, 1);
                    if(canBuild) {
                        //Added zu der eigenen Area
                        add(makeAreaBigger(vec, 4), player.id);
                        //Löschen von Gegenständen im Weg vom Hauptgebäude
                        deleteFelder(vec, 3, null);
                        //x und y Koordinaten anpassen, da Gebäude 3x3 Tiles groß ist und man auf die mittlere Tile drückt
                        vec.x = vec.x-1;
                        vec.y = vec.y-1;
                        vec.z = 1; //Gebäude auf z=1 Ebene gesetzt
                        addBuilding(newArea, volk.getHomeBuilding(0), vec);
                        //Setzen/Speichern der Position des Hauptgebäudes für den Spieler
                        volk.setHomeBuilding(0, player.id-1, tilemap, vec);
                        //Synchronisieren mit Gegnern:
                        tilemapManager.CmdUpdateTilemap(vec, volkManager.getVolkID(volk).Item2, 0, player.id-1);
                        //Zaehler geht hoch
                        ZaehlerBuildingsBuiltInRound = maxBuildingPerRound;
                        if((player.name == "Nico" || player.name == "Alex" || player.name == "4n1kan1") && gameManager.getCheatsOn()) maxBuildingPerRound = 1000; //4n1kan1, Nico und Alex dürfen mehr bauen, wenn Cheats aktiviert sind!
                        //testen für erste einheit direkt am Eingang des hauptgebäudes
                        vec.y = vec.y + 1;
                        vec.z = 2;
                        unitManager.spawnUnit(volk.getUnit(0),vec,player.id - 1);
                    }
                }
            }
        }
        //
        //Maus gedrückt und Hauptgebäude schon gesetzt(isLobby = false)
        if(Input.GetMouseButtonDown(0) && !player.isLobby) {
            Vector3Int vec = hover.getVectorFromMouse();
            vec.z=0;
            if(hover.insideField(vec)) {

                //Select Vector
                if(isMyArea(vec)) {                    
                    selectVector(vec);
                }else {
                    deselectBuilding();
                }
            }else {
                deselectBuilding(); //Infobox weggeblendet
            }
        }


    }

    //minus Ressourcen fürs kaufen
    public bool ressourcenZaehlerRechner(Ressource r, int zaehler) {
        if(ressourcenZaehler.ContainsKey(r) && zaehler <= ressourcenZaehler[r]) {
            ressourcenZaehler[r] -= zaehler;
            reloadLeiste();
            return true;
        }else if(!ressourcenZaehler.ContainsKey(r) && zaehler == 0) return true;
        return false;
    }

    //Wie viele buildings von der Art hat man
    public int howManyBuildings(Building b) {
        if(buildingsVec.ContainsValue(b)) {
            int zaehler = 0;
            foreach(KeyValuePair<Vector3Int, Building> kvp in buildingsVec) {
                if(kvp.Value == b) zaehler++;
            }
            return zaehler;
        }else return 0;
    }

    //Vector Selection
    public void selectVector(Vector3Int vec) {

        deselectBuilding();

        bool canbuild = canBuildMethod(vec);

        List<Vector3Int> vectors = makeAreaBigger(vec, 1);
        

        if(isMyArea(new Vector3Int(vec.x, vec.y, 0)) && ZaehlerBuildingsBuiltInRound < maxBuildingPerRound && !buildingvectors.ContainsKey(new Vector3Int(vec.x, vec.y, 1)) && canbuild) {
            oldSelectedColor = hover.getOldColor();
            selection = true;
            selectedVector = vec;
            tilemap.SetColor(vec, hover.getSelectColor());
            hover.reload();
            GameObject.Find("InGame/Canvas/BuildOrBuy").SetActive(true);
            if(gameManager.hasVec(vec)) {
                GameObject.Find("InGame/Canvas/BuildOrBuy/Text").GetComponent<TextMeshProUGUI>().text = "Build Building";
                GameObject.Find("InGame/Canvas/BuildOrBuy").GetComponent<Button>().onClick.AddListener(onClickBuildField);
            }
        }
    }

    //BuildField Button Click
    public void onClickBuildField() {

        Vector3Int vec = selectedVector;
        vec.z = 0;
        if(hover.insideField(vec)) {

            List<Vector3Int> liste = makeAreaBigger(vec, 1);
            List<Ressource> ressourcen = new List<Ressource>();
            Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();

            if(ZaehlerBuildingsBuiltInRound >= maxBuildingPerRound || !player.isYourTurn || !teamVectors.ContainsKey(vec) || teamVectors[vec] != player.id) return;
            foreach(Vector3Int v in liste) {
                if(buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1))) return;
                if(mapBehaviour.getBlockDetails(v).Item3 != null && teamVectors.ContainsKey(v) && teamVectors[v] == player.id) {
                    ressourcen.Add(mapBehaviour.getBlockDetails(v).Item3);
                }
            }

            GetComponent<BuildGUIPanel>().generateGUI(ressourcen, vec);

        }

        deselectBuilding();
        reloadShowArea();
    }

    //Gehört Feld zum eigenen Feld?
    //Wenn Feld zu Gegner gehört returned false
    bool isMyArea(Vector3Int vec) {
        Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();

        return teamVectors.ContainsKey(vec) && teamVectors[vec] == player.id;
    }

    //Kann man hier bauen? 3x3
    public bool canBuildMethod(Vector3Int vec) {
        vec.z = 0;
        List<Vector3Int> list = makeAreaBigger(vec, 1);
        
        foreach(Vector3Int v in list) {
            if(gameManager.isEnemyArea(v, player.id) || buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1)) || mapBehaviour.getBlockDetails(v).Item2.getBuildable() == false) return false;
        }
        return true;
    }

    //Ist auf Vektor Building vom eigenen Spieler
    public bool isOwnBuilding(Vector3Int vec) {
        //Debug.Log(buildingvectors.ContainsKey(vec));
        return buildingvectors.ContainsKey(vec);
    }

//Setzen von Ressourcengebäuden
    public void OnBuildClick(Ressource r, Vector3Int vec) {
        if(r.ressourceName == "Tree") { //ändern später damit generisch und nicht spezifisch für Tree
            List<Vector3Int> nachbarcheck = makeAreaBigger(vec, 1); //wenn building groesser dann andere zahl
            bool gebaeudeSetzbar = true;
            //schaut nach ob auf den Nachbarfeldern sich andere Gebäude befinden
            foreach(Vector3Int v in nachbarcheck) {
                if(buildingvectors.ContainsKey(v) || (hover.insideField(v) && !mapBehaviour.getBlockDetails(v).Item2.getBuildable()) || 
                buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1)) || !hover.insideField(v) || gameManager.isEnemyArea(v, player.id)) gebaeudeSetzbar = false;
            }
            if(gebaeudeSetzbar) {
                int zaehler = deleteFelder(vec, 3, r);//wenn building groesser dann andere zahl
                //Anpassen des Ressourcenzaehlers pro Runde
                if(ressourcenProRundeZaehler.ContainsKey(r)) {
                    ressourcenProRundeZaehler[r] += zaehler;
                }else {//erstes Ressourcengebäude gesetzt und Dict enthält die Ressource noch nicht
                    ressourcenProRundeZaehler.Add(r, zaehler);
                }
                //Zaehler geht hoch
                ZaehlerBuildingsBuiltInRound++; 
                
                
                addFelderToTeam(vec, 4, player.id);//1 groesser als buildinggroesse


                vec.x -= 1;
                vec.y -= 1;
                //standard Dictionary hinzufügen
                Building treeBuilding = volk.getTreeBuilding(0);
                addBuilding(nachbarcheck, treeBuilding, vec);

                vec.z = 1;
                //Vector3Int vec, int b, int playerID, int volkID, int lvl
                tilemapManager.CmdUpdateTilemapBuilding(vec, 2, player.id, volkManager.getVolkID(volk).Item2, 0);

                treeBuilding.setTile(tilemap, vec, player.id-1); //später ändern auf generisch, durch Methode vielleicht
                //synchronisieren für alle Spieler
                if(showAreaBool) reloadShowArea();
            }
        }
    }

//Synchronisieren der Felder der einzelnen Spieler
    [Command(requiresAuthority = false)]
    public void addFelderToTeam(Vector3Int vec, int groesse, int id) {//für 3x3. groesse = 3
        List<Vector3Int> allefelder = makeAreaBigger(vec, groesse-2);
        foreach(Vector3Int v in allefelder) {
            if(!gameManager.hasVec(new Vector3Int(v.x, v.y, 0)) || hover.insideField(new Vector3Int(v.x, v.y, 0))) {
                gameManager.addVec(new Vector3Int(v.x, v.y, 0), id);
            }
        }
    }

//Löschen von Tiles auf Ebene z = 1 damit dort Gebäude platziert werden können
//zaehlt außerdem wie viele Felder die entsprechede Ressource des zu bauenden Gebäudes waren(für RessourcenzaehlerproRunde)
    public int deleteFelder(Vector3Int vec, int groesse, Ressource r) { //für 3x3. groesse = 3
        vec.z = 1;
        int zaehler = 0;
        List<Vector3Int> allefelder = makeAreaBigger(vec, groesse-2);
        foreach(Vector3Int v in allefelder) {
            tilemap.SetTile(v, null);
            if(mapBehaviour.getBlockDetails(new Vector3Int(v.x, v.y, 0)).Item3 == r) zaehler++;
        }
        return zaehler;
    }

//setzt Variable selected Building + selected Vector und aktiviert Infobox
    private void selectBuilding(Vector3Int vec) {
        vec.z = 1;
        if(buildingvectors.ContainsKey(vec)) {
            selectedVector = buildingvectors[vec];
            selectedBuilding = buildingsVec[selectedVector];
            selectedVector.z = 1;
            tilemap.SetTileFlags(selectedVector, TileFlags.None);
            tilemap.SetColor(selectedVector, hover.getSelectColor());

            if(selectedBuilding.getName() == "Barracks") { //Setzt Troops Button
                GameObject.Find("InGame/Canvas/TroopsButton").SetActive(true);
                GameObject.Find("InGame/Canvas/TroopsButton").GetComponent<Button>().onClick.AddListener(openUnitPanel);
            }
            
            activatePanel(selectedVector);
        }
    }

    //Activate Unit Panel Click Event
    public void openUnitPanel() {
        GetComponent<UnitGUIPanel>().generateGUI(buildingvectors[selectedVector]);
    }

//Infoxbox Gameobjekt aktiviert
    public void activatePanel(Vector3Int vec) {
        player.infoboxBuilding.SetActive(true);
        GameObject.Find("InGame/Canvas/InfoboxBuilding/Infotext").GetComponent<TextMeshProUGUI>().text = "<b><u>Infobox</u></b> \n Name: "+buildingsVec[vec].getName()+"\n Leben: " +healthManager.getBuildingLeben(vec);
    }

//Rückgängig machen von selectBuilding Methode/ Rücksetzen der verschiedenen Sachen
    private void deselectBuilding() {
        //Vektor Selektierung
        if(selection) tilemap.SetColor(selectedVector, Color.white);
        selection = false;

        GameObject.Find("InGame/Canvas/TroopsButton").SetActive(false);
        
        hover.reload();
        GameObject.Find("InGame/Canvas/BuildOrBuy").SetActive(false);

        selectedVector.z = 1;
        
        reloadShowArea();

        selectedVector = new Vector3Int(mapBehaviour.mapWidth()+2,mapBehaviour.mapHeight()+2,-1); //selected Vektor außerhalb der Map gesetzt, da nicht auf null setzbar
        selectedBuilding = null;
        player.infoboxBuilding.SetActive(false);
        
    }

//einfügen in das Dictionary für gebaute Gebäude, einfügen in den HealthManager, alle Vektoren des Gebäudes gespeichert(da oft größer als 1 Tile)
    public void addBuilding(List<Vector3Int> vecs, Building b, Vector3Int vec) {
        vec.z = 1;
        if(buildingsVec.ContainsKey(vec)) {
            Debug.Log(buildingsVec[vec]);
            buildingsVec[vec] = b;
        }else {
            buildingsVec.Add(vec, b);
        }
        
        healthManager.addBuilding(vecs, b.getHealth(), vec);
        foreach(Vector3Int v in vecs) {
            if(buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1))) {
                buildingvectors[new Vector3Int(v.x, v.y, 1)] = vec;
            }else {
                //Debug.Log("BuildingManagerSave: " + new Vector3Int(v.x, v.y, 1) + " with: "+vec);
                buildingvectors.Add(new Vector3Int(v.x, v.y, 1), vec);
            }
        }

        int buildingid = volkManager.getBuildingID(volk, b);
        //HomeBuilding wird so synchronisiert
        if(volk.isHomeBuilding(b)) return;
    }

//Anzeigen der Spielerfelder
    public void OnShowAreaClick() {
        if(GameObject.Find("GameManager").GetComponent<PauseMenu>().getPause()) return;
        Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();
        if(!showAreaBool) {//falls vor Knopfdruck Gebiet nicht angezeigt wurde, soll jetzt angezeigt werden und Farben sich ändern
            reloadShowArea();
            showAreaBool = true;//Gebiet wird angezeigt
        }else {//Gebiet wurde angezeigt und soll nicht mehr angezeigt werden, da Knopf wieder gedrückt wurde
            foreach(KeyValuePair<Vector3Int, int> kvp in teamVectors) {
                Vector3Int vec = kvp.Key;
                vec.z = 0;

                tilemap.SetColor(vec, Color.white);

                vec.z = 1;

                tilemap.SetColor(vec, Color.white);
            }
            showAreaBool = false;//Gebiet wird nicht mehr angezeigt
        }
    }

//Passt Spielerfelder an nach Gebäudebau und synchronisiert für alle Spieler
    public void reloadShowArea() {
        for(int x=0; x<mapBehaviour.mapWidth(); x++) {
            for(int y=0; y<mapBehaviour.mapHeight(); y++) {
                tilemap.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(x, y, 0), Color.white);
                tilemap.SetTileFlags(new Vector3Int(x, y, 1), TileFlags.None);
                tilemap.SetColor(new Vector3Int(x, y, 1), Color.white);
            }
        }

        showAreaBool = true;
        Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();
        foreach(KeyValuePair<Vector3Int, int> kvp in teamVectors) {
            Vector3Int vec = kvp.Key;
            vec.z = 0;

            tilemap.SetTileFlags(vec, TileFlags.None);
            tilemap.SetColor(vec, gameManager.getColorByID(kvp.Value));

            vec.z = 1;

            tilemap.SetTileFlags(vec, TileFlags.None);
            tilemap.SetColor(vec, gameManager.getColorByID(kvp.Value));
        }
    }

//Bei Rundenstart zählt diese Methode neue Ressourcen dazu in das entsprechende Dictionary
//Setzt Zaehler für gebaute Gebaute in dieser Runde auf 0
//Leitet Änderungen an die Leiste(Gameobjekt im Spiel oben im Bildschirm) weiter um aktuellen Stand zu sehen
    public void auffuellen() {
        ZaehlerBuildingsBuiltInRound = 0;
        foreach(KeyValuePair<Ressource, int> kvp in ressourcenProRundeZaehler) {
            if(ressourcenZaehler.ContainsKey(kvp.Key)) {
                ressourcenZaehler[kvp.Key] += kvp.Value;
            }else {
                ressourcenZaehler.Add(kvp.Key, kvp.Value);
            }
            //GameObject.Find("InGame/Canvas/Leiste/"+kvp.Key.ressourceName).GetComponent<TextMeshProUGUI>().text = ressourcenZaehler[kvp.Key] + " " + kvp.Key.ressName;
        }
        reloadLeiste();
    }

    //Obere Leiste reload methode
    public void reloadLeiste() {
        foreach(KeyValuePair<Ressource, int> kvp in ressourcenZaehler) {
            GameObject.Find("InGame/Canvas/Leiste/"+kvp.Key.ressourceName).GetComponent<TextMeshProUGUI>().text = ressourcenZaehler[kvp.Key] + " " + kvp.Key.ressName;
        }
    }

//Synchronisieren von Gebäuden
    [Command(requiresAuthority = false)]
    public void add(List<Vector3Int> vecs, int id) {
        foreach(Vector3Int vec in vecs) {
            if(!gameManager.hasVec(vec) || hover.insideField(new Vector3Int(vec.x, vec.y, 0))) {
                gameManager.addVec(new Vector3Int(vec.x, vec.y, 0), id);
            }
        }
    }
    
//Methode zur erstellung einer Vektorenliste um den angeklickten Vektor(Nachbarsfelder in bestimmtem Radius)
    public List<Vector3Int> makeAreaBigger(Vector3Int vec, int groesse) {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        //Schleifen für alle Nachbarsfelder in bestimmtem Radius
        for(int x=-groesse; x<=groesse; x++) {
            for(int y=-groesse; y<=groesse; y++) {
                Vector3Int vector = new Vector3Int(x, y, 0) + vec;
                neighbors.Add(vector);
            }
        }
        return neighbors;
    }
}