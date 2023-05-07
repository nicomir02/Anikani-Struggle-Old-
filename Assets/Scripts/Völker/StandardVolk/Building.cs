using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Building : NetworkBehaviour //Networkbehaviour zum speichern?
{
    //Farbe 1: Blau
    //Farbe 2: Rot

    [SerializeField] List<TileBase> buildingTile = new List<TileBase>();
    [SerializeField] private int health = 1000;
    [SerializeField] private string buildingname = "";

    public void setTile(Tilemap tilemap, Vector3Int vec, int colorFromID) {
        tilemap.SetTile(vec, buildingTile[colorFromID]);
    }

    public TileBase getTile(int colorFromID) {
        return buildingTile[colorFromID];
    }

    public int getHealth() {
        return health;
    }

    public string getName() {
        return buildingname;
    }
}
