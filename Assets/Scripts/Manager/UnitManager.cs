using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;


public class UnitManager : NetworkBehaviour
{
    public Volk volk;
    [SerializeField] private TilemapHover hover;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private VolkManager volkManager;
    [SerializeField] private MapBehaviour mapBehaviour;
    [SerializeField] private BuildingManager buildingManager;
    
    [SerializeField] private GameObject infobox;

    [SerializeField] private GameManager gameManager;

    [SerializeField] private HealthManager healthManager;

    [SerializeField] private RoundManager roundManager;

    [SerializeField] private InputManager inputManager;

    //private int buildInRound = 0;
    //bewegungsreichweite fehlt/angriffsbegrenzung fehlt/begrenzte blöcke fehlen
    public Dictionary<Vector3Int, Unit> spawnedUnits = new Dictionary<Vector3Int, Unit>();

    //Bewegungsreichweite
    Dictionary<Vector3Int, int> reichweite = new Dictionary<Vector3Int, int>();

    private Unit selectedUnit;
    private Vector3Int selectedVector;


    //Benötigt um zu markieren wohin die Einheit kann
    private List<Vector3Int> weite = new List<Vector3Int>();

    private bool isLobby = true;
    
    
    void Start() {
        volk = roundManager.eigenesVolk;
        buildingManager = GetComponent<BuildingManager>();
    }

    //Nächste Runde abfrage
    public bool canNextRound() {
        foreach(KeyValuePair<Vector3Int, int> kvp in reichweite) {
            if(kvp.Value > 0) return false;
        }
        return true;
    }

    
    public void deleteAllUnits() {
        foreach(KeyValuePair<Vector3Int, Unit> kvp in spawnedUnits) {
            cmddeleteUnit(kvp.Key);
        }
        
    }

    public void disqualify() {
        foreach(KeyValuePair<Vector3Int, Unit> kvp in spawnedUnits) {
            healthManager.removeUnit(kvp.Key);
            tilemapManager.removeUnit(kvp.Key);
        }
        deleteAllUnits();
        spawnedUnits = new Dictionary<Vector3Int, Unit>();
        reichweite = new Dictionary<Vector3Int, int>();
    }

    public bool hasUnitOnVec(Vector3Int vec) {
        vec.z = 2;
        if(spawnedUnits.ContainsKey(vec)) {
            return true;
        }
        return false;
    }

    List<Vector3Int> spawnedUnitsToVectorList(int z) { //liste mit spawnedUnits Vektoren, normalisiert auf einen Z-Wert(weil bspw. Pathfinding braucht z=-1)
        List<Vector3Int> result = new List<Vector3Int>();
        foreach(KeyValuePair<Vector3Int, Unit> kvp in spawnedUnits) {
            result.Add(new Vector3Int(kvp.Key.x, kvp.Key.y, z));
        }
        return result;
    }
    
    private void Update(){
        if(GameObject.Find("GameManager").GetComponent<PauseMenu>().getPause()) return;

        if(GameObject.Find("InGame/Canvas/ShowArea") != null && isLobby) {
            volk = roundManager.eigenesVolk;
            isLobby = false;
        }

        if(volk == null && GameObject.Find("InGame/Canvas/ShowArea") != null) {
            volk = roundManager.eigenesVolk;
        }

        if(Input.GetMouseButtonDown(0)) {
            Vector3Int vec = hover.getVectorFromMouse();
            vec.z = 2;
            if(hover.insideField(vec)){
                

                if (selectedUnit == null){
                    selectUnit(vec);
                }else{
                    if(reichweite[selectedVector] > 0) {
                        moveUnit(selectedUnit, vec); 
                    }
                    deselectUnit();
                }

            }
        }

        if(spawnedUnits.ContainsKey(healthManager.angegriffenVec)) {
            if(healthManager.getLeben(healthManager.angegriffenVec) <= 0) {
                if(selectedVector == healthManager.angegriffenVec) deselectUnit();
                spawnedUnits.Remove(healthManager.angegriffenVec);
                reichweite.Remove(healthManager.angegriffenVec);
            } else if(selectedVector == healthManager.angegriffenVec) activatePanel(selectedVector);
        }


        if(selectedUnit != null) {
            if(Input.GetKeyDown(KeyCode.LeftArrow)) {
                Vector3Int last = new Vector3Int(-1,-1,-1);
                Vector3Int next = new Vector3Int(-1,-1,-1);

                bool vorbei = false;

                foreach(KeyValuePair<Vector3Int, Unit> kvp in spawnedUnits) {
                    if(kvp.Key == selectedVector) {
                        vorbei = true;
                    }else {
                        if(vorbei) {
                            next = kvp.Key;
                        }else {
                            last = kvp.Key;
                        }
                    }
                }

                if(last.z != -1) {
                    deselectUnit();
                    selectUnit(last);
                    GameObject.Find("GameManager").GetComponent<MapBehaviour>().cameraChange(last.x, last.y);
                }else if(next.z != -1) {
                    deselectUnit();
                    selectUnit(next);
                    GameObject.Find("GameManager").GetComponent<MapBehaviour>().cameraChange(next.x, next.y);
                }
                
            }

            if(Input.GetKeyDown(KeyCode.RightArrow)) {
                Vector3Int first = new Vector3Int(-1,-1,-1);
                Vector3Int next = new Vector3Int(-1,-1,-1);

                bool vorbei = false;
                foreach(KeyValuePair<Vector3Int, Unit> kvp in spawnedUnits) {
                    if(kvp.Key == selectedVector) {
                        vorbei = true;
                    }else {
                        if(vorbei && next.z == -1) {
                            next = kvp.Key;
                        }else {
                            if(first.z == -1) first = kvp.Key;
                        }
                    }
                }

                if(next.z != -1) {
                    deselectUnit();
                    selectUnit(next);
                    GameObject.Find("GameManager").GetComponent<MapBehaviour>().cameraChange(next.x, next.y);
                }else if(first.z != -1) {
                    deselectUnit();
                    selectUnit(first);
                    GameObject.Find("GameManager").GetComponent<MapBehaviour>().cameraChange(first.x, first.y);
                }
            }
        }

        Vector3Int vector = hover.getVectorFromMouse();
        einheitReichweiteMarkierer();       
    }
    
    //Methode um zu markieren wo Einheit hin kann
    private Vector3Int old = new Vector3Int(-1, -1, -1);
    void einheitReichweiteMarkierer() {
        Vector3Int vec = hover.getVectorFromMouse();

        if (selectedUnit != null) {
            Pathfinding pathfinding = new Pathfinding(selectedVector, vec, selectedUnit, spawnedUnitsToVectorList(-1), healthManager.isBuilding(selectedVector));
            if (old == vec || !pathfinding.canWalk(vec) || healthManager.isHealth(vec)) return;
            old = vec;

            int curReichweite = reichweite[selectedVector];

            if (hover.insideField(vec) && distance(selectedVector, vec) <= curReichweite && selectedVector != vec) {
                ClearWeite();
                List<Vector3Int> tempweite = pathfinding.shortestPath();

                if (weite != null && tempweite != null && weite == tempweite) return;

                if (weite != null && tempweite != null) {
                    foreach (Vector3Int vector in weite) {
                        Vector3Int v = new Vector3Int(vector.x, vector.y, 0);
                        if (!tempweite.Contains(v)) {
                            if (buildingManager.getShowArea() && gameManager.teamVecs.ContainsKey(v) && gameManager.teamVecs[v] == roundManager.id) {
                                tilemap.SetColor(v, gameManager.getColorByID(roundManager.id));
                            } else {
                                tilemap.SetColor(v, Color.white);
                            }
                        }
                    }
                }
                if(tempweite == null) return;
                weite = tempweite.ToList();

                if (weite != null && weite.Count <= curReichweite) {
                    int i = 0;
                    foreach (Vector3Int v in weite) {
                        if (i > curReichweite) break;
                        i++;
                        tilemap.SetTileFlags(new Vector3Int(v.x, v.y, 0), TileFlags.None);
                        tilemap.SetColor(new Vector3Int(v.x, v.y, 0), hover.getSelectColor());
                    }
                }
            }else {
                ClearWeite();
            }
        }else {
            ClearWeite();
        }
    }

    void ClearWeite() {
        for (int x=0; x<mapBehaviour.mapWidth(); x++) {
            for (int y=0; y<mapBehaviour.mapHeight(); y++) {
                tilemap.SetColor(new Vector3Int(x, y, 0), Color.white);
            }
        }
        if (buildingManager.getShowArea()) buildingManager.reloadShowArea();

        
        weite.Clear();
        
    }

    public void deselectUnit(){
        if(getGameObject(selectedVector) != null) {
            getGameObject(selectedVector).GetComponent<SpriteRenderer>().color = Color.white;
        }
        Vector3Int vec = new Vector3Int(mapBehaviour.mapWidth()+2,mapBehaviour.mapHeight()+2,-1);
        selectedUnit = null;
        
        selectedVector = vec;

        infobox.SetActive(false);
    }

    public void selectUnit(Vector3Int vec){
        vec.z = 2;
        if(healthManager.getLeben(vec) == -1 && spawnedUnits.ContainsKey(vec)) spawnedUnits.Remove(vec);
        if(spawnedUnits.ContainsKey(vec) || findUnit(vec)){ 
            selectedUnit = spawnedUnits[vec];
            selectedVector = vec;
            if(getGameObject(vec) != null) {
                getGameObject(vec).GetComponent<SpriteRenderer>().color = Color.grey;
            }
            activatePanel(vec);
        }
    }

    public bool findUnit(Vector3Int vec) {
        foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
            if(us.vec.x == vec.x && us.vec.y == vec.y && us.id == roundManager.id) {
                if(!spawnedUnits.ContainsKey(new Vector3Int(vec.x, vec.y, 2))) {
                    if(healthManager.getLeben(new Vector3Int(vec.x, vec.y, 2)) < 0) {
                        cmddeleteUnit(us.vec);
                        return false;
                    }
                    spawnedUnits.Add(new Vector3Int(vec.x, vec.y, 2), us.GetComponent<Unit>());
                    return true;
                }
            }
        }
        return false;
    }

    public void activatePanel(Vector3Int vec) {
        infobox.SetActive(true);

        //NOTIZÄNDERUNG Leben zu Health und Angriffswert zu Attackpower und verfügbare Bewegung zu Available Movement damit es auf englishc ist
        //Variante für einheiten welche heilen können(z.B. Engel) und sonstige im else
        if(spawnedUnits[vec].getHeilung() != 0){
            GameObject.Find("InGame/Canvas/Infobox/Infotext").GetComponent<TextMeshProUGUI>().text = "<b><u>Infobox</u></b> \n Name: "+spawnedUnits[vec].getName()+"\n Health: "+ healthManager.getLeben(vec) +" \n Attackpower: "+ spawnedUnits[vec].getAngriffswert() + "\n Healing Amount: " + spawnedUnits[vec].getHeilung() + "\n Available Movement: " +reichweite[vec];
        } else {
            GameObject.Find("InGame/Canvas/Infobox/Infotext").GetComponent<TextMeshProUGUI>().text = "<b><u>Infobox</u></b> \n Name: "+spawnedUnits[vec].getName()+"\n Health: "+ healthManager.getLeben(vec) +" \n Attackpower: "+ spawnedUnits[vec].getAngriffswert() +" \n Available Movement: " +reichweite[vec];
        }
    }

    public int distance(Vector3Int vec1, Vector3Int vec2) {
        return Mathf.Abs(vec1.x - vec2.x) + Mathf.Abs(vec1.y - vec2.y);
    }

    //GameObject von Vektor3Int
    public GameObject getGameObject(Vector3Int vec) {
        vec.z = 2;
        UnitSprite[] unitSprites = FindObjectsOfType<UnitSprite>();
        foreach(UnitSprite us in unitSprites) {
            if(us.vec == vec) {
                return us.gameObject;
            }
        }
        return null;
    }

    //Movement von einem Spieler
    public void moveUnit(Unit unit, Vector3Int vec){
        if(!GameObject.Find("GameManager").GetComponent<RoundManager>().isYourTurn) return;
        vec.z = 2;
        if((distance(selectedVector, vec) <= (reichweite[selectedVector]+1)) && mapBehaviour.getBlockDetails(new Vector3Int(vec.x, vec.y, 0)).Item2.getWalkable() && !GetComponent<BuildingManager>().isOwnBuilding(vec)) {
            if(healthManager.isHealth(vec) && !GetComponent<BuildingManager>().isOwnBuilding(new Vector3Int(vec.x, vec.y, 1))){
                angriff(unit, vec);
                return;
                //if(healthManager.isHealth(vec)) return;    //schaut ob gegner besiegt wurde in dieser runde || Auskommentiert, sonst kann gegner Einheit bewegen
            }

            if(distance(selectedVector, vec) > reichweite[selectedVector] || spawnedUnits.ContainsKey(vec)) return;
            //unit.set(tilemap,vec,player.id -1);

            List<Vector3Int> liste = new Pathfinding(selectedVector, vec, unit, spawnedUnitsToVectorList(-1), healthManager.isBuilding(selectedVector)).shortestPath(); //Berechnung des shortest Path
            if(liste == null || liste.Count > reichweite[selectedVector]) return; //Wenn der shortest Path größer ist als die Reichweite return

            spawnedUnits.Remove(selectedVector);    //diese beiden Zeilen damit die Dictionary sich mit der neuen position updated
            spawnedUnits.Add(vec, unit);
            
            int r = reichweite[selectedVector];
            reichweite.Remove(selectedVector);
            reichweite.Add(vec, r-liste.Count);

            foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
                if(us.vec.x == selectedVector.x && us.vec.y == selectedVector.y) us.gameObject.transform.GetChild(0).GetComponent<HealthBar>().changeReichweite(r-liste.Count);
            }
            
            gameManager.moveUnit(selectedVector, vec, unit.getMaxBloeckeProRunde());

            healthManager.moveUnit(selectedVector, vec);
            syncMovedUnits(selectedVector);

            

            cmdMoveUnit(selectedVector, vec, liste, roundManager.id);

            
        }
    }


    [Command(requiresAuthority = false)]
    public void cmddeleteUnit(Vector3Int vector) {
        vector.z = 2;
        UnitSprite[] unitSprites = FindObjectsOfType<UnitSprite>();
        foreach(UnitSprite us in unitSprites) {
            if(us.vec == vector) {
                NetworkServer.Destroy(us.gameObject);
            }
        }
    }

    //Für Sprite movement
    [Command(requiresAuthority = false)]
    public void cmdMoveUnit(Vector3Int from, Vector3Int to, List<Vector3Int> shortestPath, int id) {
        from.z = 2;
        UnitSprite[] unitSprites = FindObjectsOfType<UnitSprite>();
        foreach(UnitSprite us in unitSprites) {
            if(us.vec == from) {
                to.z = 2;
                us.vec = to;
                to.z = 0;
                StartCoroutine(MoveToPosition(us.GetComponent<Transform>(), shortestPath, 0.5f, from, id));
                changeVecInUnitSprite(from, to);
            }
        }
    }

    //Change in UnitSprite bei jedem Client
    [ClientRpc]
    public void changeVecInUnitSprite(Vector3Int from, Vector3Int to) {
        
        inputManager.fogOfWarShowHealthbar();

        from.z = 2;
        to.z = 2;

        foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
            if(us.vec == from) us.vec = to;
        }
    }

    public IEnumerator MoveToPosition(Transform transform, List<Vector3Int> positions, float timeToMove, Vector3Int from, int id)
    {
        transform.GetComponent<AudioUnit>().startAudio(0);
        for(int i=-1; i<positions.Count-1; i++) {
            float elapsedTime = 0f;
            Vector3 startingPosition;
            Vector3 position;
            if(i >= 0) {
                startingPosition = vec3IntToVec3(positions[i]);
                position = vec3IntToVec3(positions[i+1]);
                transform.gameObject.GetComponent<UnitAnimator>().changeDirection(positions[i], positions[i+1], id); //hier change Direction methode aufrufen
            }else {
                startingPosition = vec3IntToVec3(from);
                position = vec3IntToVec3(positions[0]);
                transform.gameObject.GetComponent<UnitAnimator>().changeDirection(from, positions[0], id); //hier change Direction methode aufrufen
            }
            
            

            while (elapsedTime < timeToMove)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / timeToMove);
                transform.position = Vector3.Lerp(startingPosition, position, t);
                yield return null;
            }
        }
        transform.GetComponent<AudioUnit>().startAudio(-1);
        transform.gameObject.GetComponent<UnitAnimator>().changeDirection(positions[positions.Count-1], positions[positions.Count-1], id);
        transform.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    //Convert Vector3Int von Vec3Int für Units
    public Vector3 vec3IntToVec3(Vector3Int vec){
        vec.z = 0;
        Vector3 position = tilemap.CellToWorld(vec);
        position.z = 3.1f;
        return position;
    }


    public void spawnUnit(Unit unit, Vector3Int vec, int colorID){
        vec.z = 2;
        if(!spawnedUnits.ContainsKey(vec)) {

            spawnedUnits.Add(vec, unit);
            healthManager.addUnit(vec, unit.getLeben());
            reichweite.Add(vec, 0);

            serverAddPlayer(volk.getUnitID(unit), vec, colorID, volkManager.getVolkID(volk).Item2);

        }
    }

    [Command(requiresAuthority = false)]
    public void serverAddPlayer(int unitID, Vector3Int vec, int colorID, int volkID)
    {
        Unit unit = volkManager.getVolk(volkID).getUnit(unitID);
        GameObject spriteObject = Instantiate(unit.gameObject, transform.position, transform.rotation);

        spriteObject.GetComponent<UnitSprite>().vec = vec;
        vec.z = 0;

        spriteObject.GetComponent<UnitSprite>().id = colorID+1;

        Vector3 position = tilemap.CellToWorld(vec);
        position.z = 3.1f;
        spriteObject.transform.position = position;

        spriteObject.transform.rotation = Quaternion.identity;
        spriteObject.transform.localScale = Vector3.one;

        NetworkServer.Spawn(spriteObject);

        setRenderer(spriteObject, unitID, vec, colorID, volkID);
    }

    [ClientRpc]
    public void setRenderer(GameObject gameObject, int unitID, Vector3Int vec, int colorID, int volkID) {
        inputManager.fogOfWarShowHealthbar();

        gameObject.GetComponent<UnitSprite>().id = colorID+1;

        Unit unit = volkManager.getVolk(volkID).getUnit(unitID);
        Sprite sprite = unit.getSprite(colorID);

        vec.z = 2;
        gameObject.GetComponent<UnitSprite>().vec = vec;
        gameObject.GetComponent<UnitAnimator>().chooseColor(colorID+1);

        if(roundManager.id == colorID+1) {
            gameObject.transform.GetChild(0).GetComponent<HealthBar>().changeReichweite(0);
        }
        
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>(); 
        spriteRenderer.sprite = sprite;

    }

    public void angriff(Unit unit, Vector3Int vec){
        Vector3Int v = new Vector3Int(vec.x, vec.y, 2);
        if(spawnedUnits.ContainsKey(v) && vec != selectedVector) {
            if(unit.getHeilung() == 0) return;
            int maxhealth = spawnedUnits[v].getLeben();
            int heal = 0;
            if(healthManager.getLeben(v) == maxhealth) return;
            if(healthManager.getLeben(v) + unit.getHeilung() > maxhealth) {
                heal = maxhealth - healthManager.getLeben(v);       //damit nicht mehr als max leben geheilt wird
            }else {
                heal = unit.getHeilung();
            }
            foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
                if(us.vec.x == selectedVector.x && us.vec.y == selectedVector.y) us.gameObject.transform.GetChild(0).GetComponent<HealthBar>().changeReichweite(0);
            }
            
            healthManager.angriff(v, -heal); //da heilung ein negativer angriff in Höhe von heal ist
            reichweite[selectedVector] = 0;

            

            return;
        }
        


        if(spawnedUnits.ContainsKey(v) || !canAttack(unit, selectedVector, vec)) return;

        angriffAusfuehren(selectedVector, vec);

        reichweite[selectedVector] = 0;

        foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
                if(us.vec.x == selectedVector.x && us.vec.y == selectedVector.y) {
                    us.gameObject.transform.GetChild(0).GetComponent<HealthBar>().changeReichweite(0);
                    us.GetComponent<AudioUnit>().startAudio(1);
                }
            }

        if(healthManager.isUnit(new Vector3Int(vec.x, vec.y, 2))) {
            healthManager.angriff(new Vector3Int(vec.x, vec.y, 2), unit.getAngriffswert());
            //Sync Still Exists nun in healthbar
        }else if(healthManager.isBuilding(new Vector3Int(vec.x, vec.y, 1))) {
            healthManager.angriffBuilding(new Vector3Int(vec.x, vec.y, 1), unit.getAngriffswert());
            syncStillExistsBuilding(new Vector3Int(vec.x, vec.y, 1));
        }
        
    }

    [Command(requiresAuthority=false)]
    public void angriffAusfuehren(Vector3Int from, Vector3Int to) {
        Debug.Log(from +" "+ to);
        foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
            if(us.vec.x==from.x && us.vec.y==from.y) us.GetComponent<UnitAnimator>().angreifenN(from, to);
        }
    }

    public bool canAttack(Unit unit, Vector3Int from, Vector3Int to) {
        if(unit.getKampfweite() > 1) {
            if(distance(from, to) > unit.getKampfweite()) return false;
            return true;
        }else {
            for(int x=-1; x<=1; x++) {
                for(int y=-1; y<=1; y++) {
                    Vector3Int vector = new Vector3Int(x, y, 0) + from;
                    if(vector == to) return true;
                }
            }
            return false;
        }
    }

    [Command(requiresAuthority = false)]
    public void syncStillExistsBuilding(Vector3Int vec) {
        if(healthManager.getBuildingLeben(vec) <= 0) {
            GetComponent<BuildingManager>().angriffsCheckBuilding(vec);
        }
    }

    public void auffuellen() {
        Dictionary<Vector3Int, int> temp = new Dictionary<Vector3Int, int>();
        foreach (KeyValuePair<Vector3Int, Unit> kvp in spawnedUnits) {
            temp.Add(kvp.Key, kvp.Value.getMaxBloeckeProRunde());

            foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
                if(us.vec.x == kvp.Key.x && us.vec.y == kvp.Key.y) us.gameObject.transform.GetChild(0).GetComponent<HealthBar>().changeReichweite(kvp.Value.getMaxBloeckeProRunde());
            }
        }
        reichweite = temp;
    }

    [Command(requiresAuthority = false)]
    public void syncStillExists(Vector3Int vec) {
        if(healthManager.getLeben(vec) <= 0) {
            healthManager.removeUnit(vec);
            syncMovedUnitsClient(vec);
            cmddeleteUnit(vec);
        }
    }

    [Command(requiresAuthority = false)]
    public void syncMovedUnits(Vector3Int vec){
        syncMovedUnitsClient(vec);
    }

    [ClientRpc]
    public void syncMovedUnitsClient(Vector3Int vec){
        //tilemap.SetTile(vec, null);

        if(spawnedUnits.ContainsKey(vec)) {
            spawnedUnits.Remove(vec);
            reichweite.Remove(vec);
        }
    }
}
