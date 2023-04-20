using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Block : NetworkBehaviour
{
    [SerializeField] private TileBase tile;
    [SerializeField] private string block;

    public TileBase getTile() {
        return tile;
    }

    public string getName() {
        return this.block;
    }

    public Block(string name) {

    }

    public Block(string name, TileBase tb) {
        this.block = name;
        this.tile = tb;
    }
}
