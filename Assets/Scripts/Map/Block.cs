using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Block : NetworkBehaviour
{
    [SerializeField] private TileBase[] tile;
    [SerializeField] private string block;
    [SerializeField] private bool isBuildable = true;
    [SerializeField] private bool isWalkable = true;

    public TileBase getTile() {
        return tile[0];
    }

    public void setTile(Vector3Int vec, Tilemap tilemap) {
        for(int i=0; i<tile.Length; i++) {
            vec.z = i;
            tilemap.SetTile(vec, tile[i]);
        }
    }

    public string getName() {
        return this.block;
    }

    public bool getBuildable() {
        return isBuildable;
    }

    public bool getWalkable() {
        return isWalkable;
    }


    


}
