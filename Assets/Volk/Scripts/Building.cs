using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{
    //Farbe 1: Blau
    //Farbe 2: Rot

    [SerializeField] List<TileBase> buildingTile = new List<TileBase>();

    public void setTile(Tilemap tilemap, Vector3Int vec, int colorFromID) {
        tilemap.SetTile(vec, buildingTile[colorFromID]);
    }

    public TileBase getTile(int colorFromID) {
        return buildingTile[colorFromID];
    }
}
