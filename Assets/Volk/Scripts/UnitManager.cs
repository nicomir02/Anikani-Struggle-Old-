using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
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

    //private int buildInRound = 0;
    //bewegungsreichweite fehlt/angriffsbegrenzung fehlt/begrenzte blöcke fehlen
   
   
    Dictionary<Vector3Int, Unit> spawnedUnits = new Dictionary<Vector3Int, Unit>();
    readonly SyncDictionary<Vector3Int, int> healthUnits = new SyncDictionary<Vector3Int, int>();
   
   

    public Unit selectedUnit;
    public Vector3Int selectedVector;
    
    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hover = GameObject.Find("GameManager").GetComponent<TilemapHover>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        player = GetComponent<Player>();
        volk = player.eigenesVolk;
        tilemapManager = GameObject.Find("GameManager").GetComponent<TilemapManager>();
        volkManager = GameObject.Find("GameManager").GetComponent<VolkManager>();
        mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
    }

    
    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            Vector3Int vec = hover.getVectorFromMouse();
            vec.z = 2;
            if(hover.insideField(vec)){
                if (selectedUnit == null){
                selectUnit(vec);}
                else{
                    moveUnit(selectedUnit, vec);
                    deselectUnit();
                }

            }
        }
    }

    public void deselectUnit(){
        Vector3Int vec = new Vector3Int(mapBehaviour.mapWidth()+2,mapBehaviour.mapHeight()+2,-1);
        selectedUnit = null;
        selectedVector = vec;
    }
    public void selectUnit(Vector3Int vec){
        if(spawnedUnits.ContainsKey(vec)){ 
            selectedUnit = spawnedUnits[vec];
            selectedVector = vec;
        }
    }

    public void moveUnit(Unit unit, Vector3Int vec){
        if(tilemap.GetTile(vec) != null){
            angriff(unit, vec);
            if(tilemap.GetTile(vec) != null) return;    //schaut ob gegner besiegt wurde in dieser runde
        }
        tilemap.SetTile(selectedVector, null);
        unit.setTile(tilemap,vec,player.id -1);
        tilemapManager.CmdUpdateTilemapUnit(vec,volkManager.getVolkID(volk).Item2,volk.getUnitID(unit),player.id -1);
        spawnedUnits.Remove(selectedVector);    //diese beiden Zeilen damit die Dictionary sich mit der neuen position updated
        spawnedUnits.Add(vec, unit);
        int currentLeben = healthUnits[selectedVector];
        healthUnits.Remove(selectedVector);
        healthUnits.Add(vec, currentLeben);
        syncMovedUnits(selectedVector);
    }


    public void spawnUnit(Unit unit, Vector3Int vec, int colorID){
        unit.setTile(tilemap,vec,colorID);
        tilemapManager.CmdUpdateTilemapUnit(vec,volkManager.getVolkID(volk).Item2,volk.getUnitID(unit),colorID);
        spawnedUnits.Add(vec, unit);
        healthUnits.Add(vec, unit.getLeben());
    }

    public void angriff(Unit unit, Vector3Int vec){
        syncAngriff(unit.getAngriffswert(), vec);
    }

    [Command]
    public void syncAngriff(int angriffswert, Vector3Int vec){  //funktioniert noch nicht. wahrschienlich einen Unitmanager aufbauen außerhalb der spieler für die SyncDictionary
        
        if(healthUnits.ContainsKey(vec)){
            healthUnits[vec] -= angriffswert;
            if(healthUnits[vec] <= 0){
                healthUnits.Remove(vec);
                syncMovedUnits(vec);
            }
        }
        
        syncAngriffClient(angriffswert, vec);
    }

    [ClientRpc]
    public void syncAngriffClient(int angriffswert, Vector3Int vec){
        foreach(KeyValuePair<Vector3Int, Unit> kvp in spawnedUnits){ Debug.Log(kvp.Key); };
        if(spawnedUnits.ContainsKey(vec) && !healthUnits.ContainsKey(vec)){
            Debug.Log("Test"); //Testzwecke
            spawnedUnits.Remove(vec);
        }
    }

    [Command]
    public void syncMovedUnits(Vector3Int vec){
        syncMovedUnitsClient(vec);
    }

    [ClientRpc]
    public void syncMovedUnitsClient(Vector3Int vec){
        tilemap.SetTile(vec, null);
    }

//old code:


/*void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            Vector3Int vec = hover.getVectorFromMouse();
            if(hover.insideField(vec) /* && weiter Konditionen für Spieler setzen ){
                canPlace = true;
                if(canPlace && CurrentUnitCounter < MaxUnitCounter) {
                    localAddTile(vec, GetComponent<Player>().getID());
                    CurrentUnitCounter++;
                }else {
                    //Debug.Log("Not possible");
                }
            }
        }   
    } */
    /* [Command]
    public void localAddTile(Vector3Int vec, int id) {
        vec = new Vector3Int(vec.x, vec.y, vec.z+1);
        if (unit[id].BesetzterBlock != null){
            unit[id].BesetzterBlock.BesetzteUnit = null;
            unit[id].transform.position = vec;
            map.getBlockDetails(vec).Item2.BesetzteUnit = unit[id];
            unit[id].BesetzterBlock = map.getBlockDetails(vec).Item2;
            save.Add(vec, id);
            list.Add(id); 
        }           
    }

    public static void SetSelectedUnit(Unit unit) {
        SelectedUnit = unit;
    }

    void OnMouseDown() {
        Vector3Int vec = hover.getVectorFromMouse();
        if (map.getBlockDetails(vec).Item2.BesetzteUnit != null) {
            if (UnitManager.SelectedUnit != null) {
                    var enemy = map.getBlockDetails(vec).Item2.BesetzteUnit;
                    Destroy(enemy.gameObject);
                    UnitManager.SetSelectedUnit(null);
                }
            UnitManager.SetSelectedUnit(map.getBlockDetails(vec).Item2.BesetzteUnit);
                
        }
        else {
            if (UnitManager.SelectedUnit != null) {
                SetUnit(UnitManager.SelectedUnit, vec);
                UnitManager.SetSelectedUnit(null);
            }
        }

    }

    [Command]
    public void SetUnit(Unit unit, Vector3Int vec) {
        if (unit.BesetzterBlock != null) unit.BesetzterBlock.BesetzteUnit = null;
        Block block = map.getBlockDetails(vec).Item2;
        unit.transform.position = vec;
        block.BesetzteUnit = unit;
        unit.BesetzterBlock = block;
    } */

}
