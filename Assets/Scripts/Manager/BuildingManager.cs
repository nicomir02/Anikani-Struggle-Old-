using System.Collections;
using System;
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
    public Volk volk; //Eigenes Volk
    [SerializeField] private TilemapHover hover; //abfragung, wo mauszeiger ist
    [SerializeField] private Tilemap tilemap; //Tilemap abfragen
    [SerializeField] private TilemapManager tilemapManager; //änderungen auf tilemap
    [SerializeField] private VolkManager volkManager; //volk id herausfinden
    [SerializeField] private MapBehaviour mapBehaviour; //Block details herausfinden, biome etc.
    [SerializeField] private HealthManager healthManager; //healthmanager für gebäudeleben 
    [SerializeField] private RoundManager roundManager;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private UnitManager unitManager;    //für testzwecke der ersten einheit

    [SerializeField] private PauseMenu pauseMenu;

    [SerializeField] private Button showArea;
    [SerializeField] private GameObject moveBuildingButtonObject;

    [SerializeField] private GameObject infoboxBuilding;


    private bool showAreaBool = false;
//isLobby wird nur benutzt bei der Spielinitialisierung in der Update Methode
    private bool isLobby = true; 

    public int maxBuildingPerRound = 1;
    //ZaehlerBuildingsbuiltInRound zählt die Anzahl an Gebäuden die in der Runde gebaut wurden(eventuell Limit setzen später)
    public int ZaehlerBuildingsBuiltInRound = 0;

    //was für ein Building an diesem Vektor sich befindet(jedes bUilding einmal in dem Dictionary)
    public Dictionary<Vector3Int, Building> buildingsVec = new Dictionary<Vector3Int, Building>();
    //da manche Buildings größer als 1 Feld sind muss man alle besetzten Felder
    //hier auflisten um mit buildingsvex zu korrespondieren
    private Dictionary<Vector3Int, Vector3Int> buildingvectors = new Dictionary<Vector3Int, Vector3Int>();
    //zeigt wie viel mehr Ressourcen pro Runde dazu kommen
    //old: private Dictionary<Ressource, int> ressourcenProRundeZaehler = new Dictionary<Ressource, int>();
    private Dictionary<Vector3Int, (Ressource, int)> ressourcenProRundeZaehler = new Dictionary<Vector3Int, (Ressource, int)>();

    //zeigt gesamtmenge an Ressourcen die man hat:
    private Dictionary<Ressource, int> ressourcenZaehler = new Dictionary<Ressource, int>();

    

    //Knopf um auszuwählen welches Gebäude man platzieren will
    //Button ressourcenButton;
    
    //ausgewählter/angeklickter Vektor
    public Vector3Int selectedVector;
    //ausgewähltes/angeklicktes Gebäude
    Building selectedBuilding;
    //Farbe
    Color oldSelectedColor;


    void Start() {
        unitManager = GetComponent<UnitManager>();//für testzwecke der ersten einheit
    }

    public bool nearBuilding(Vector3Int vec, int naehe) {
        foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in buildingvectors) {
            if(unitManager.distance(vec, kvp.Key) <= naehe) {
                return true;
            }
        }
        return false;
    }

    //methode angriffgebäude schaut ob das angegriffene Gebäude einem Spieler selber gehört
    [ClientRpc]
    public void angriffsCheckBuilding(Vector3Int vec) {
        vec.z = 1;
        if(buildingvectors.ContainsKey(vec) && healthManager.getBuildingLeben(vec) <= 0) {
            vec = buildingvectors[vec];
            Vector3Int temp = buildingvectors[vec];
            if(buildingsVec[vec] == volk.getHomeBuilding(0)) {
                disqualifyPlayer();
                GameObject.Find("GameManager").GetComponent<WinLooseScreen>().setLooseScreen();
            }else {
                
                
                //Remove Building von Listen
                buildingsVec.Remove(temp);
                
                List<Vector3Int> removeList = new List<Vector3Int>();
                foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in buildingvectors) {
                    if(kvp.Value == temp) {
                        removeList.Add(kvp.Key);
                        if(ressourcenProRundeZaehler.ContainsKey(kvp.Key)) ressourcenProRundeZaehler.Remove(kvp.Key);
                    }
                }

                

                if(removeList.Contains(selectedVector)) {
                    deselectBuilding();
                }

                foreach(Vector3Int v in removeList) {
                    buildingvectors.Remove(v);
                }

                //Algorithmus, der prüft ob in der Nähe der Vektoren noch ein Building ist, sonst rauslöschen
                Vector3Int forGameManager = vec;
                vec.x += 1;
                vec.y += 1;
                List<Vector3Int> liste = makeAreaBigger(vec, 4);

                foreach(Vector3Int v in liste) {
                    if(!nearBuilding(v, 3)) {
                        gameManager.removeTeamVec(v);
                    }
                }


                

                tilemapManager.removeBuilding(temp, 2);
                healthManager.removeBuilding(vec);
            }
        }
        if(showAreaBool) {
            reloadShowArea();
        }
    }

//methode sollte nicht im BUildingManager sein->später ändern
    public void disqualifyPlayer() {
        foreach(KeyValuePair<Vector3Int, Building> kvp in buildingsVec) {
            healthManager.removeBuilding(kvp.Key);
            tilemapManager.removeBuilding(kvp.Key, 3);
        }

        buildingsVec = new Dictionary<Vector3Int, Building>();
        buildingvectors = new Dictionary<Vector3Int, Vector3Int>();

        unitManager.disqualify();
        foreach(Player p in FindObjectsOfType<Player>()) {
            p.spielerDisqualifizieren(GameObject.Find("GameManager").GetComponent<RoundManager>().id);
        }
    }

    //Getter Methode für ZaehlerBuildingsBuiltInRound
    public int getZahlBuildInRound() {
        return ZaehlerBuildingsBuiltInRound;
    }

//Build Inkrement wenn gebaut wird
    public void buildInRoundZaehlerInkrement() {
        ZaehlerBuildingsBuiltInRound++;
    }

    

    private IEnumerator kurzRot(Vector3Int v) {
        v.z = 0;

        hover.reload();
        hover.setHoverBoolKurz(false);
        
        tilemap.SetTileFlags(v, TileFlags.None);
        tilemap.SetColor(v, Color.red);

        yield return new WaitForSeconds(0.5f);
        hover.setHoverBoolKurz(true);
        hover.reload();

        tilemap.SetColor(v, Color.white);

    }

    public void moveBuilding() {
        if(moveBuildingButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "Move") {
            moveBuildingButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Dont move";
        }else {
            moveBuildingButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Move";
        }
    }

    void Update() {
        //Pause deaktivieren
        if(pauseMenu.getPause()) return;

        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject != null) return;

        //Initialisieren des ShowArea GameObjects nach Spielstart(nur einmal!)
        if(GameObject.Find("InGame/Canvas/ShowArea") != null && isLobby) {
            volk = roundManager.eigenesVolk;
            GameObject.Find("InGame/Canvas/ShowArea").GetComponent<Button>().onClick.AddListener(OnShowAreaClick);
            moveBuildingButtonObject = GameObject.Find("InGame/Canvas/WandererMoveMainButton");
            moveBuildingButtonObject.GetComponent<Button>().onClick.AddListener(moveBuilding);
            isLobby = false;
        }

        if(volk == null && GameObject.Find("InGame/Canvas/ShowArea") != null) {
            volk = roundManager.eigenesVolk;
        }

        //Hauptgebäude aufgebaut nach Spielstart(nur einmal)
        if(Input.GetMouseButtonDown(0) && GameObject.Find("GameManager").GetComponent<RoundManager>().isYourTurn && !GameObject.Find("GameManager").GetComponent<RoundManager>().isLobby) {
            Vector3Int vec = hover.getVectorFromMouse();
            if(hover.insideField(vec)) {
                if(GameObject.Find("GameManager").GetComponent<RoundManager>().round == 0 && ZaehlerBuildingsBuiltInRound == 0) {
                    List<Vector3Int> newArea = makeAreaBigger(vec, 1);


                    //Check ob ein Main Building in der Nähe ist
                    List<Vector3Int> check = healthManager.getAllVecsOnZero();

                    bool mainBuildingNear = false;
                    int minAbstand = gameManager.getMinAbstandMainBuilding();

                    foreach(Vector3Int v in check) {
                        if(buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1)) || newArea.Contains(new Vector3Int(v.x, v.y, -1)) ) continue;
                        foreach(Vector3Int ve in newArea) {
                            if(unitManager.distance(v, new Vector3Int(ve.x, ve.y, -1)) <= minAbstand) {
                                mainBuildingNear = true;
                            }
                        }
                    }

        	        if(mainBuildingNear) StartCoroutine(kurzRot(vec));

                    if(canBuildMethod(vec) && !mainBuildingNear) { 
                        
                        //Added zu der eigenen Area
                        addFelderToTeam(vec, 6, GameObject.Find("GameManager").GetComponent<RoundManager>().id);
                        //Löschen von Gegenständen im Weg vom Hauptgebäude
                        deleteFelder(vec, 3, null);

                        //x und y Koordinaten anpassen, da Gebäude 3x3 Tiles groß ist und man auf die mittlere Tile drückt
                        vec.x = vec.x-1;
                        vec.y = vec.y-1;
                        
                        vec.z = 1; //Gebäude auf z=1 Ebene gesetzt

                        
                        Building homeBuilding = volk.getHomeBuilding(0);
                        addBuilding(newArea, homeBuilding, vec);


                        if(!homeBuilding.getCanMove()) {
                            
                            //Setzen/Speichern der Position des Hauptgebäudes für den Spieler
                            volk.setHomeBuilding(0, GameObject.Find("GameManager").GetComponent<RoundManager>().id-1, tilemap, vec);
                            //Synchronisieren mit Gegnern:
                            tilemapManager.CmdUpdateTilemap(vec, volkManager.getVolkID(volk).Item2, 0, GameObject.Find("GameManager").GetComponent<RoundManager>().id-1);

                        }else {
                            CMDsetBuildingObject(roundManager.id, volkManager.getVolkID(volk).Item2, 0, vec);
                        }

                        //4n1kan1, Nico und Alex dürfen mehr bauen, wenn Cheats aktiviert sind!s
                        if((roundManager.playername == "Nico" || roundManager.playername == "Alex" || roundManager.playername == "4n1kan1") && gameManager.getCheatsOn()) maxBuildingPerRound = 1000;
                        //Zaehler geht hoch
                        ZaehlerBuildingsBuiltInRound = maxBuildingPerRound;

                        vec.y = vec.y + 1;
                        vec.z = 2;
                        unitManager.spawnUnit(volk.getUnit(0),vec,GameObject.Find("GameManager").GetComponent<RoundManager>().id - 1);
                    }
                }
            }
        }
        //
        //Maus gedrückt und Hauptgebäude schon gesetzt(isLobby = false)
        if(Input.GetMouseButtonDown(0) && !GameObject.Find("GameManager").GetComponent<RoundManager>().isLobby) {
            

            Vector3Int vec = hover.getVectorFromMouse();
            vec.z=0;

            if(hover.insideField(vec)) {
                //Select Vector
                selectVector(vec);
            }
        }

        //ShowArea auch durch tätigen der Keyboard Taste m auswählbar
        
        /*if (Input.GetKeyDown(KeyCode.M)){
            OnShowAreaClick();
        }*/
        if(Input.GetButtonDown("Toggle ShowArea")){
            OnShowAreaClick();
        }

        //Troop Recruitment Menu öffnen (erste Barracke ausgewählt) mit Taste T default
        
    }

    [Command(requiresAuthority=false)]
    public void CMDsetBuildingObject(int playerid, int volkid, int buildingid, Vector3Int vec) {
        Debug.Log(playerid);
        Building building = volkManager.getVolk(volkid).getBuilding(buildingid, 0);
        GameObject gobject = Instantiate(building.gameObject, transform.position, transform.rotation);

        gobject.GetComponent<Transform>().position = unitManager.vec3IntToVec3(vec);

        NetworkServer.Spawn(gobject);
        setRenderer(gobject, playerid, vec);
    }

    [ClientRpc]
    public void setRenderer(GameObject gobject, int playerid, Vector3Int vec) {
        gobject.GetComponent<SpriteRenderer>().sprite = gobject.GetComponent<Building>().sprite[playerid-1];
        gobject.GetComponent<Building>().vec = vec;
    }

    public bool buildUnitPanelNextRound() {
        foreach(KeyValuePair<Vector3Int,Building> kvp in buildingsVec){
            if(kvp.Value.getName() == "Barracks"){
                if(!GetComponent<UnitGUIPanel>().howLong.ContainsKey(kvp.Key)) return false;
            }
        }
        return true;
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
        bool wantToMove = false;
        if(selectedBuilding != null && moveBuildingButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "Move" ) {
            /*selectedVector.z = 1;
            Vector3Int buildingvec = buildingvectors[selectedVector];
            foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in buildingvectors) {
                if(kvp.Value == buildingvec) {
                    if(unitManager.distance(kvp.Key, vec) <= 5) {
                        wantToMove = true;
                    }
                }
            }*/
            wantToMove = true;
        }
        

        if(!moveBuildingButtonObject.activeSelf || moveBuildingButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "Move" || wantToMove) {

            deselectBuilding();
            selectBuilding(vec);

            List<Vector3Int> vectors = makeAreaBigger(vec, 1);

            if(isMyArea(new Vector3Int(vec.x, vec.y, 0)) && showAreaBool) {
                oldSelectedColor = hover.getOldColor();
                selectedVector = vec;
                
                hover.reload();
                reloadShowArea();
                
                tilemap.SetColor(vec, hover.getSelectColor());

                if(ZaehlerBuildingsBuiltInRound < maxBuildingPerRound && !buildingvectors.ContainsKey(new Vector3Int(vec.x, vec.y, 1)) && canBuildMethod(vec) && GameObject.Find("GameManager").GetComponent<RoundManager>().isYourTurn) {
                    GameObject.Find("InGame/Canvas/BuildOrBuy").SetActive(true);
                    if(gameManager.hasVec(vec)) {
                        GameObject.Find("InGame/Canvas/BuildOrBuy/Text").GetComponent<TextMeshProUGUI>().text = "Build Building";
                        GameObject.Find("InGame/Canvas/BuildOrBuy").GetComponent<Button>().onClick.AddListener(onClickBuildField);
                    }
                }
            }else if(showAreaBool) {
                reloadShowArea();
            }

            moveBuildingButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Move";
        }else {
            foreach(Building b in FindObjectsOfType<Building>()) {
                if(b.vec.x == selectedVector.x && b.vec.y == selectedVector.y) {
                    //Vector3Int startTile, Vector3Int zielTile, Unit unit, List<Vector3Int> liste, bool onBuilding
                    List<Vector3Int> newAreaBuilding = makeAreaBigger(vec, 1);


                    int groesse = 3;
                    if(b.getName() == "Main Animal") {
                        groesse = 5;
                    }
                    selectedVector.z = 1;

                    //Vector3Int middlePoint = buildingvectors[selectedVector];

                    List<Vector3Int> theArea = makeAreaBigger(new Vector3Int(buildingvectors[selectedVector].x+1, buildingvectors[selectedVector].y+1, 0), groesse);

                    foreach(Vector3Int v in newAreaBuilding) {
                        if(!canBuildMethod(v, false)) return;
                    }
                    Vector3Int buildingvec = buildingvectors[selectedVector];

                    vec.x -= 1;
                    vec.y -= 1;
                    vec.z = 1;

                    if(b.getName() == "Barracks") {
                        foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in buildingvectors) {
                            if(kvp.Value == buildingvec) {
                                if(GetComponent<UnitGUIPanel>().howLong.ContainsKey(kvp.Key)) {
                                    int howlong = GetComponent<UnitGUIPanel>().howLong[kvp.Key];
                                    Unit trainUnit = GetComponent<UnitGUIPanel>().trainedUnits[kvp.Key];

                                    GetComponent<UnitGUIPanel>().howLong.Remove(kvp.Key);
                                    GetComponent<UnitGUIPanel>().trainedUnits.Remove(kvp.Key);

                                    GetComponent<UnitGUIPanel>().howLong.Add(vec, howlong);
                                    GetComponent<UnitGUIPanel>().trainedUnits.Add(vec, trainUnit);
                                }
                            }
                        }
                    }

                    
                    selectedVector.z = 1;
                    
                    List<Vector3Int> removeList = new List<Vector3Int>();
                    
                    

                    foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in buildingvectors) {
                        if(kvp.Value == buildingvec) {
                            removeList.Add(kvp.Key);
                        }
                    }

                    tilemapManager.resetFelder(removeList);

                    foreach(Vector3Int v in removeList) {
                        buildingvectors.Remove(v);
                    }


                    vec.x += 1;
                    vec.y += 1;
                    vec.z = 0;
                    foreach(Vector3Int v in theArea) {
                        if(buildingvectors.Count > 0) {
                            bool remove = true;
                            foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in buildingvectors){
                                if(unitManager.distance(v, kvp.Key) <= 3) {
                                    remove = false;
                                }
                            }
                            if(remove) {
                                gameManager.removeTeamVec(v);
                            }
                        }else {
                            gameManager.removeTeamVec(v);
                        }
                    }
                    
                    theArea = makeAreaBigger(vec, groesse);
                    foreach(Vector3Int v in theArea) {
                        gameManager.addVec(v, roundManager.id);
                    }


                    vec.x -= 1;
                    vec.y -= 1;
                    vec.z = 1;

                    buildingsVec.Remove(buildingvec);
                    buildingsVec.Add(vec, b);
                    foreach(Vector3Int v in newAreaBuilding) {
                        buildingvectors.Add(new Vector3Int(v.x, v.y, 1), vec);
                    }
                
                    List<Vector3Int> liste = new List<Vector3Int>();
                    tilemapManager.deleteFelderUnterBuilding(newAreaBuilding);
                    liste.Add(selectedVector);
                    liste.Add(vec);

                    healthManager.moveBuilding(selectedVector, newAreaBuilding);

                    cmdMoveUnit(selectedVector, vec, liste, roundManager.id);
                    
                    moveBuildingButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Move";
                    deselectBuilding();
                }
            }
        }
    }


    [Command(requiresAuthority = false)]
    public void cmdMoveUnit(Vector3Int from, Vector3Int to, List<Vector3Int> shortestPath, int id) {
        from.z = 2;
        Building[] unitSprites = FindObjectsOfType<Building>();
        foreach(Building b in unitSprites) {
            if(b.vec.x == selectedVector.x && b.vec.y == selectedVector.y) {
                to.z = 2;
                b.vec = to;
                to.z = 0;
                StartCoroutine(MoveToPosition(b.GetComponent<Transform>(), shortestPath, 0.5f, from, id));
            }
        }
    }



    public IEnumerator MoveToPosition(Transform transform, List<Vector3Int> positions, float timeToMove, Vector3Int from, int id)
    {
        for(int i=-1; i<positions.Count-1; i++) {
            float elapsedTime = 0f;
            Vector3 startingPosition;
            Vector3 position;
            if(i >= 0) {
                startingPosition = unitManager.vec3IntToVec3(positions[i]);
                position = unitManager.vec3IntToVec3(positions[i+1]);
            }else {
                startingPosition = unitManager.vec3IntToVec3(from);
                position = unitManager.vec3IntToVec3(positions[0]);
            }
            
            

            while (elapsedTime < timeToMove)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / timeToMove);
                transform.position = Vector3.Lerp(startingPosition, position, t);
                yield return null;
            }
        }
        transform.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }



    //BuildField Button Click
    public void onClickBuildField() {

        Vector3Int vec = selectedVector;
        vec.z = 0;
        if(hover.insideField(vec)) {

            List<Vector3Int> liste = makeAreaBigger(vec, 1);
            List<Ressource> ressourcen = new List<Ressource>();
            Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();

            if(ZaehlerBuildingsBuiltInRound >= maxBuildingPerRound || !GameObject.Find("GameManager").GetComponent<RoundManager>().isYourTurn || !teamVectors.ContainsKey(vec) || teamVectors[vec] != GameObject.Find("GameManager").GetComponent<RoundManager>().id) return;
            foreach(Vector3Int v in liste) {
                if(buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1))) return;
                if(mapBehaviour.getBlockDetails(v).Item3 != null) {
                    ressourcen.Add(mapBehaviour.getBlockDetails(v).Item3);
                }
            }

            GetComponent<BuildGUIPanel>().generateGUI(ressourcen, vec);

        }

        deselectBuilding();
        if(showAreaBool) reloadShowArea();
    }

    //Gehört Feld zum eigenen Feld?
    //Wenn Feld zu Gegner gehört returned false
    bool isMyArea(Vector3Int vec) {
        Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();

        return teamVectors.ContainsKey(vec) && teamVectors[vec] == GameObject.Find("GameManager").GetComponent<RoundManager>().id;
    }

    //Kann man hier bauen? 3x3
    public bool canBuildMethod(Vector3Int vec) {
        vec.z = 0;
        List<Vector3Int> list = makeAreaBigger(vec, 1);
        
        foreach(Vector3Int v in list) {
            if(hover.insideField(v)) {
                if(gameManager.isEnemyArea(v, GameObject.Find("GameManager").GetComponent<RoundManager>().id) || buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1)) || mapBehaviour.getBlockDetails(v).Item2.getBuildable() == false) return false;
            }else {
                return false;
            }
        }
        return true;
    }

    public bool canBuildMethod(Vector3Int vec, bool mustBeOwnArea) {
        vec.z = 0;
        List<Vector3Int> list = makeAreaBigger(vec, 1);
        
        foreach(Vector3Int v in list) {
            if(hover.insideField(v)) {
                if((gameManager.isEnemyArea(v, GameObject.Find("GameManager").GetComponent<RoundManager>().id) && mustBeOwnArea)|| buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1)) || mapBehaviour.getBlockDetails(v).Item2.getBuildable() == false) return false;
            }else {
                return false;
            }
        }
        return true;
    }

    //Ist auf Vektor Building vom eigenen Spieler
    public bool isOwnBuilding(Vector3Int vec) {
        vec.z = 1;
        //Debug.Log(buildingvectors.ContainsKey(vec));
        return buildingvectors.ContainsKey(vec);
    }

//Setzen von Ressourcengebäuden
    public void OnBuildClick(Ressource r, Vector3Int vec) {
        if(volk.getRessourceBuilding(r, 0) != null) {
            Building b = volk.getRessourceBuilding(r, 0);
            List<Vector3Int> nachbarcheck = makeAreaBigger(vec, 1); //wenn building groesser dann andere zahl

            bool gebaeudeSetzbar = true;

            foreach(Vector3Int v in nachbarcheck) {
                if(buildingvectors.ContainsKey(v) || (hover.insideField(v) && !mapBehaviour.getBlockDetails(v).Item2.getBuildable()) || 
                buildingvectors.ContainsKey(new Vector3Int(v.x, v.y, 1)) || !hover.insideField(v) || gameManager.isEnemyArea(v, GameObject.Find("GameManager").GetComponent<RoundManager>().id)) gebaeudeSetzbar = false;
            }
            if(gebaeudeSetzbar) {
                int zaehler = deleteFelder(vec, 3, r); //wenn building groesser dann andere zahl

                //Zaehler geht hoch
                ZaehlerBuildingsBuiltInRound++;

                addFelderToTeam(vec, 4, GameObject.Find("GameManager").GetComponent<RoundManager>().id);//1 groesser als buildinggroesse

                vec.x -= 1;
                vec.y -= 1;

                //standard Dictionary hinzufügen
                addBuilding(nachbarcheck, b, vec);

                vec.z = 1;
                ressourcenProRundeZaehler.Add(vec, (r, zaehler));
                //Vector3Int vec, int b, int playerID, int volkID, int lvl

                Debug.Log(b.getCanMove());

                if(!b.getCanMove()) {
                    //Synchronisieren mit Gegnern:
                    tilemapManager.CmdUpdateTilemapBuilding(vec, volkManager.getBuildingID(volk, b), GameObject.Find("GameManager").GetComponent<RoundManager>().id, volkManager.getVolkID(volk).Item2, 0);

                }else {
                    CMDsetBuildingObject(roundManager.id, volkManager.getVolkID(volk).Item2, volkManager.getBuildingID(volk, b), vec);
                }

                

                b.setTile(tilemap, vec, GameObject.Find("GameManager").GetComponent<RoundManager>().id-1); //später ändern auf generisch, durch Methode vielleicht
                //synchronisieren für alle Spieler
                if(showAreaBool) reloadShowArea();
            }
        }
    }

//Synchronisieren der Felder der einzelnen Spieler
    [Command(requiresAuthority = false)]
    public void addFelderToTeam(Vector3Int vec, int groesse, int id) {//für 2x2. groesse = 3
        List<Vector3Int> allefelder = makeAreaBigger(vec, groesse-1);
        foreach(Vector3Int v in allefelder) {
            if(!gameManager.hasVec(new Vector3Int(v.x, v.y, 0)) || hover.insideField(new Vector3Int(v.x, v.y, 0))) {
                gameManager.addVec(new Vector3Int(v.x, v.y, 0), id);
            }
        }
        AllReloadShowArea();
    }

    [Command (requiresAuthority = false)]
    public void CMDallReloadArea() {
        AllReloadShowArea();
    }

    //Reload ShowArea auf allen Clients
    [ClientRpc]
    public void AllReloadShowArea() {
        if(showAreaBool) reloadShowArea();
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
    public void selectBuilding(Vector3Int vec) {
        vec.z = 1;
        if(buildingvectors.ContainsKey(vec)) {
            
            selectedVector = buildingvectors[vec];
            selectedBuilding = buildingsVec[selectedVector];
            selectedVector.z = 1;
            if(!selectedBuilding.getCanMove()) {
                tilemap.SetTileFlags(selectedVector, TileFlags.None);
                tilemap.SetColor(selectedVector, hover.getSelectColor());
            }else {
                moveBuildingButtonObject.SetActive(true);
            }
            if(selectedBuilding.getName() == "Barracks") { //Setzt Troops Button
                GameObject.Find("InGame/Canvas/TroopsButton").SetActive(true);
                GameObject.Find("InGame/Canvas/TroopsButton").GetComponent<Button>().onClick.AddListener(openUnitPanel);
            }
            
            activatePanel(selectedVector);
        }else if(healthManager.isBuilding(vec) && gameManager.isAufgedeckt(vec)) {
            activateEnemyPanel(vec);
        }
    }

    public Vector3Int getSelectedVector() {
        return selectedVector;
    }

    //Activate Unit Panel Click Event
    public void openUnitPanel() {
        Vector3Int v = selectedVector;
        v.z = 1;
        GetComponent<UnitGUIPanel>().generateGUI(buildingvectors[v]);
    }

//Infoxbox Gameobjekt aktiviert
    public void activatePanel(Vector3Int vec) {
        infoboxBuilding.SetActive(true);
        //NOTIZÄNDERUNG Leben zu Health damit es auf englisch ist
        GameObject.Find("InGame/Canvas/InfoboxBuilding/Infotext").
        GetComponent<TextMeshProUGUI>().text = 
        "<b><u>Infobox</u></b> \n Name: " + buildingsVec[vec].getName() +
        "\n Health: " +healthManager.getBuildingLeben(vec);
    }

    public void activateEnemyPanel(Vector3Int vec) {
        infoboxBuilding.SetActive(true);
        //NOTIZÄNDERUNG Leben zu Health damit es auf englisch ist
        GameObject.Find("InGame/Canvas/InfoboxBuilding/Infotext").
        GetComponent<TextMeshProUGUI>().text = 
        "Health: " +healthManager.getBuildingLeben(vec);
    }

//Rückgängig machen von selectBuilding Methode/ Rücksetzen der verschiedenen Sachen
    private void deselectBuilding() {

        tilemap.SetColor(selectedVector, Color.white);

        moveBuildingButtonObject.SetActive(false);
        
        //Vektor Selektierung
        

        GameObject.Find("InGame/Canvas/TroopsButton").SetActive(false);
        
        hover.reload();
        GameObject.Find("InGame/Canvas/BuildOrBuy").SetActive(false);

        selectedVector.z = 1;

        selectedVector = new Vector3Int(mapBehaviour.mapWidth()+2,mapBehaviour.mapHeight()+2,-1); //selected Vektor außerhalb der Map gesetzt, da nicht auf null setzbar
        selectedBuilding = null;
        infoboxBuilding.SetActive(false);

        if(showAreaBool) reloadShowArea();
    }

//einfügen in das Dictionary für gebaute Gebäude, einfügen in den HealthManager, alle Vektoren des Gebäudes gespeichert(da oft größer als 1 Tile)
    public void addBuilding(List<Vector3Int> vecs, Building b, Vector3Int vec) {
        GetComponent<AudioSource>().Play();
        vec.z = 1;
        tilemapManager.deleteFelderUnterBuilding(vecs);
        if(buildingsVec.ContainsKey(vec)) {
            //Debug.Log(buildingsVec[vec]);
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
        deleteSelection();
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

                if(gameManager.isAufgedeckt(vec)) tilemap.SetColor(vec, Color.white);
            }
            showAreaBool = false;//Gebiet wird nicht mehr angezeigt
        }
    }

    //Reload Area auf nochmale Farben
    public void deleteSelection() {
        for(int x=0; x<mapBehaviour.mapWidth(); x++) {
            for(int y=0; y<mapBehaviour.mapHeight(); y++) {
                tilemap.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(x, y, 0), Color.white);
                tilemap.SetTileFlags(new Vector3Int(x, y, 1), TileFlags.None);
                tilemap.SetColor(new Vector3Int(x, y, 1), Color.white);
            }
        }
    }

    //Methode getter methode ob show Area aktiv ist
    public bool getShowArea() {
        return showAreaBool;
    }


//Passt Spielerfelder an nach Gebäudebau und synchronisiert für alle Spieler
    public void reloadShowArea() {
        for (int x=0; x<mapBehaviour.mapWidth(); x++) {
            for (int y=0; y<mapBehaviour.mapHeight(); y++) {
                tilemap.SetColor(new Vector3Int(x, y, 0), Color.white);
            }
        }

        showAreaBool = true;
        Dictionary<Vector3Int, int> teamVectors = gameManager.getDictionary();
        foreach(KeyValuePair<Vector3Int, int> kvp in teamVectors) {
            Vector3Int vec = kvp.Key;
            vec.z = 0;

            tilemap.SetTileFlags(vec, TileFlags.None);
            tilemap.SetColor(vec, gameManager.getColorByID(kvp.Value));

            //if(gameManager.isAufgedeckt(vec)) {
                vec.z = 1;

                tilemap.SetTileFlags(vec, TileFlags.None);
                tilemap.SetColor(vec, gameManager.getColorByID(kvp.Value));
            //}
        }

        
    }

//Bei Rundenstart zählt diese Methode neue Ressourcen dazu in das entsprechende Dictionary
//Setzt Zaehler für gebaute Gebaute in dieser Runde auf 0
//Leitet Änderungen an die Leiste(Gameobjekt im Spiel oben im Bildschirm) weiter um aktuellen Stand zu sehen
    public void auffuellen() {
        ZaehlerBuildingsBuiltInRound = 0;
        foreach(KeyValuePair<Vector3Int, (Ressource, int)> kvp in ressourcenProRundeZaehler) {
            if(ressourcenZaehler.ContainsKey(kvp.Value.Item1)) {
                ressourcenZaehler[kvp.Value.Item1] += kvp.Value.Item2;
            }else {
                ressourcenZaehler.Add(kvp.Value.Item1, kvp.Value.Item2);
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