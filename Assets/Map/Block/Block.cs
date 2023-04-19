using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Block : NetworkBehaviour
{
    [SerializeField] private TileBase tile;

    public TileBase getTile() {
        return this.tile;
    }
}
