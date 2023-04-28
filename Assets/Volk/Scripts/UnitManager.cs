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
   
   
   
   
   
   

    public static Unit SelectedUnit;
    
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

    // Update is called once per frame
    /* void Update()
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




    public void spawnUnit(Unit unit, Vector3Int vec, int colorID){
        unit.setTile(tilemap,vec,colorID);
        tilemapManager.CmdUpdateTilemapUnit(vec,volkManager.getVolkID(volk).Item2,volk.getUnitID(unit),colorID);
    }

//old code:
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
