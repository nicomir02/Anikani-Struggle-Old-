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
    
    private void Update(){
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

    public void moveUnit(Unit unit, Vector3Int vec){
        if(distance(selectedVector, vec) <= reichweite[selectedVector] && mapBehaviour.getBlockDetails(new Vector3Int(vec.x, vec.y, 0)).Item2.getWalkable()) {
            if(healthManager.isHealth(vec)){
                angriff(unit, vec);
                if(healthManager.isHealth(vec)) return;    //schaut ob gegner besiegt wurde in dieser runde
            }
            tilemap.SetTile(selectedVector, null);
            unit.setTile(tilemap,vec,player.id -1);
            tilemapManager.CmdUpdateTilemapUnit(vec,volkManager.getVolkID(volk).Item2,volk.getUnitID(unit),player.id -1);
            spawnedUnits.Remove(selectedVector);    //diese beiden Zeilen damit die Dictionary sich mit der neuen position updated
            spawnedUnits.Add(vec, unit);
            
            int r = reichweite[selectedVector];
            reichweite.Remove(selectedVector);
            reichweite.Add(vec, r-distance(selectedVector, vec));
            
            healthManager.moveUnit(selectedVector, vec);
            syncMovedUnits(selectedVector);
        }
    }


    public void spawnUnit(Unit unit, Vector3Int vec, int colorID){
        unit.setTile(tilemap,vec,colorID);
        tilemapManager.CmdUpdateTilemapUnit(vec,volkManager.getVolkID(volk).Item2,volk.getUnitID(unit),colorID);
        spawnedUnits.Add(vec, unit);
        healthManager.addUnit(vec, unit.getLeben());
        reichweite.Add(vec, 0);
    }

    public void angriff(Unit unit, Vector3Int vec){
        if(healthManager.isUnit(vec)) {
            if(spawnedUnits.ContainsKey(vec)) return;
            healthManager.angriff(vec, unit.getAngriffswert());
            syncStillExists(vec);
            reichweite[selectedVector] = 0;
        }else {
            healthManager.angriffBuilding(vec, unit.getAngriffswert());
            reichweite[selectedVector] = 0;
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
