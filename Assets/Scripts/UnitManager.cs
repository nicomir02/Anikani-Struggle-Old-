using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class UnitManager : NetworkBehaviour
{
    [SerializeField] private TilemapHover hover;
    private MapBehaviour map;

    [SerializeField] private Unit[] unit;
    [SerializeField] private TileBase tile;
    [SerializeField] private Tilemap tilemap;

    private readonly SyncDictionary<Vector3Int, int> save = new SyncDictionary<Vector3Int, int>();
    private List<int> list = new List<int>();

    private GameManager gameManager;
    
    //fürs unit setzen noch ausprobieren als Kondition
    //private int MaxUnitCounter = 5;
    //private int CurrentUnitCounter = 0;
    //private bool canPlace = false; 
    public static Unit SelectedUnit;
    
    void Start() {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        hover = GameObject.Find("GameManager").GetComponent<TilemapHover>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        map = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
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

    [Command]
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
    }

}
