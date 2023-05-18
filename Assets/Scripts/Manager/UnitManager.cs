using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class UnitManager : NetworkBehaviour
{
    private Volk volk;
    private Player player;
    private TilemapHover hover;
    private Tilemap tilemap;
    private TilemapManager tilemapManager;
    private VolkManager volkManager;
    private MapBehaviour mapBehaviour;

    private GameManager gameManager;

    private HealthManager healthManager;

    //private int buildInRound = 0;
    //bewegungsreichweite fehlt/angriffsbegrenzung fehlt/begrenzte blöcke fehlen
    Dictionary<Vector3Int, Unit> spawnedUnits = new Dictionary<Vector3Int, Unit>();

    //Bewegungsreichweite
    Dictionary<Vector3Int, int> reichweite = new Dictionary<Vector3Int, int>();

    private GameObject infobox;

    private Unit selectedUnit;
    private Vector3Int selectedVector;

    [SerializeField] GameObject spritePrefab;
    
    
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
    }

    public void disqualify() {
        foreach(KeyValuePair<Vector3Int, Unit> kvp in spawnedUnits) {
            healthManager.removeUnit(kvp.Key);
            tilemapManager.removeUnit(kvp.Key);
        }
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
    
    private void Update(){
        if(GameObject.Find("GameManager").GetComponent<PauseMenu>().getPause()) return;
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
        
    }

    public void deselectUnit(){
        Vector3Int vec = new Vector3Int(mapBehaviour.mapWidth()+2,mapBehaviour.mapHeight()+2,-1);
        selectedUnit = null;

        tilemap.SetColor(selectedVector, Color.white);
        selectedVector = vec;

        player.infobox.SetActive(false);
    }
    public void selectUnit(Vector3Int vec){
        vec.z = 2;
        if(healthManager.getLeben(vec) == -1 && spawnedUnits.ContainsKey(vec)) spawnedUnits.Remove(vec);
        if(spawnedUnits.ContainsKey(vec)){ 
            selectedUnit = spawnedUnits[vec];
            selectedVector = vec;
            tilemap.SetTileFlags(selectedVector, TileFlags.None);
            tilemap.SetColor(vec, Color.grey);
            activatePanel(vec);
        }
    }

    public void activatePanel(Vector3Int vec) {
        player.infobox.SetActive(true);
        GameObject.Find("InGame/Canvas/Infobox/Infotext").GetComponent<TextMeshProUGUI>().text = "<b><u>Infobox</u></b> \n Name: "+spawnedUnits[vec].getName()+"\n Leben: "+ healthManager.getLeben(vec) +" \n Angriffswert: "+ spawnedUnits[vec].getAngriffswert() +" \n verfügbare Bewegung: " +reichweite[vec];
    }

    public int distance(Vector3Int vec1, Vector3Int vec2) {
        return Mathf.Abs(vec1.x - vec2.x) + Mathf.Abs(vec1.y - vec2.y);
    }


    //Movement von einem Spieler
    public void moveUnit(Unit unit, Vector3Int vec){
        vec.z = 2;
        if(distance(selectedVector, vec) <= reichweite[selectedVector] && mapBehaviour.getBlockDetails(new Vector3Int(vec.x, vec.y, 0)).Item2.getWalkable()) {
            if(healthManager.isHealth(vec) && !GetComponent<BuildingManager>().isOwnBuilding(new Vector3Int(vec.x, vec.y, 1))){
                angriff(unit, vec);
                return;
                //if(healthManager.isHealth(vec)) return;    //schaut ob gegner besiegt wurde in dieser runde || Auskommentiert, sonst kann gegner Einheit bewegen
            }
            if(spawnedUnits.ContainsKey(vec)) return;
            tilemap.SetTile(selectedVector, null);
            //unit.setTile(tilemap,vec,player.id -1);
            //tilemapManager.CmdUpdateTilemapUnit(vec,volkManager.getVolkID(volk).Item2,volk.getUnitID(unit),player.id -1);
            spawnedUnits.Remove(selectedVector);    //diese beiden Zeilen damit die Dictionary sich mit der neuen position updated
            spawnedUnits.Add(vec, unit);
            
            int r = reichweite[selectedVector];
            reichweite.Remove(selectedVector);
            reichweite.Add(vec, r-distance(selectedVector, vec));
            
            healthManager.moveUnit(selectedVector, vec);
            syncMovedUnits(selectedVector);

            cmdMoveUnit(selectedVector, vec);
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
    public void cmdMoveUnit(Vector3Int from, Vector3Int to) {
        from.z = 2;
        UnitSprite[] unitSprites = FindObjectsOfType<UnitSprite>();
        foreach(UnitSprite us in unitSprites) {
            if(us.vec == from) {

                to.z = 2;
                us.vec = to;

                to.z = 0;
                //MoveToPosition(us.GetComponent<Transform>(), vec3IntToVec3(to), 2f);
                us.GetComponent<Transform>().position = Vector3.Lerp(us.GetComponent<Transform>().position, vec3IntToVec3(to), 2f);
            }
        }
    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        Vector3 currentPos = transform.position;
        float t = 0f;
        Debug.Log("test");
        while(t < 1) {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }

    public Vector3 vec3IntToVec3(Vector3Int vec){
        vec.z = 0;
        Vector3 position = tilemap.CellToWorld(vec);
        position.z += 5f;
        return position;
    }


    public void spawnUnit(Unit unit, Vector3Int vec, int colorID){
        //unit.setTile(tilemap,vec,colorID);
        //tilemapManager.CmdUpdateTilemapUnit(vec,volkManager.getVolkID(volk).Item2,volk.getUnitID(unit),colorID);
        spawnedUnits.Add(vec, unit);
        healthManager.addUnit(vec, unit.getLeben());
        reichweite.Add(vec, 0);

        //tilemap.SetColor(new Vector3Int(vec.x, vec.y, 2), Color.clear);

        serverAddPlayer(volk.getUnitID(unit), vec, colorID, volkManager.getVolkID(volk).Item2);
    }

    [Command(requiresAuthority = false)]
    public void serverAddPlayer(int unitID, Vector3Int vec, int colorID, int volkID)
    {
        GameObject spriteObject = Instantiate(spritePrefab, transform.position, transform.rotation);

        UnitSprite unitSprite = spriteObject.GetComponent<UnitSprite>();
        unitSprite.vec = vec;
        vec.z = 0;
        Vector3 position = tilemap.CellToWorld(vec);
        position.z += 5f;
        spriteObject.transform.position = position;

        spriteObject.transform.rotation = Quaternion.identity;
        spriteObject.transform.localScale = Vector3.one;

        NetworkServer.Spawn(spriteObject);

        setRenderer(spriteObject, unitID, vec, colorID, volkID);
    }

    [ClientRpc]
    public void setRenderer(GameObject gameObject, int unitID, Vector3Int vec, int colorID, int volkID) {

        Unit unit = volkManager.getVolk(volkID).getUnit(unitID);
        TileBase tile = unit.getTile(colorID);
        Sprite sprite = null;
        if (tile != null && tile is Tile tileComponents)
        {
            sprite = tileComponents.sprite;
        }
        
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>(); 
        spriteRenderer.sprite = sprite;
    }

    public void angriff(Unit unit, Vector3Int vec){
        if(spawnedUnits.ContainsKey(new Vector3Int(vec.x, vec.y, 2)) || distance(selectedVector, vec) > unit.getKampfweite()) return;

        reichweite[selectedVector] = 0;

        if(healthManager.isUnit(new Vector3Int(vec.x, vec.y, 2))) {
            healthManager.angriff(new Vector3Int(vec.x, vec.y, 2), unit.getAngriffswert());
            syncStillExists(new Vector3Int(vec.x, vec.y, 2));
        }else if(healthManager.isBuilding(new Vector3Int(vec.x, vec.y, 1))) {
            healthManager.angriffBuilding(new Vector3Int(vec.x, vec.y, 1), unit.getAngriffswert());
            syncStillExistsBuilding(new Vector3Int(vec.x, vec.y, 1));
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
