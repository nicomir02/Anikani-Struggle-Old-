using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{


    [SerializeField] TileBase buildingTiles;

    public void setTile(Tilemap tilemap, Vector3Int vec) {
        tilemap.SetTile(vec, buildingTiles);
    }

    public TileBase getTile() {
        return buildingTiles;
    }
}
