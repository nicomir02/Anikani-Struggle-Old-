using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class NewMapBehaviour : NetworkBehaviour
{
    [SerializeField] private Biom[] biome;
    [SerializeField] private Ressource[] ressourcen;

    //Syncvariablen funktionieren scheinbar nur mit nicht eigens implementierten Objekten

    //SyncDictionary<Vector3Int, (Biomindex, Blockindex, Ressourcenbool, ressindex)> eindeutiger als vorher
    //private readonly SyncDictionary<Vector3Int, (int, int, bool)> blockDetails = new SyncDictionary<Vector3Int, (int, int, bool)>();

    //Random
    private System.Random rand = new System.Random();

    //Einstellungen Größe der Karte
    [SerializeField] private int width = 25; 
    [SerializeField] private int height = 25;
    [SerializeField] private int biomgroesse = 250;

    //Karteneinstellungen
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase randTile;

    public (Biom, Block, Ressource) getBlockDetails(Vector3Int vec) {
        /*Biom biom = biome[blockDetails[vec].Item1];
        Block block = biom.getBlockByIndex(blockDetails[vec].Item2);
        Ressource ressource = null;
        if(blockDetails[vec].Item3) {
            ressource = ressourcen[blockDetails[vec].Item4];
        }*/

        return (null, null, null);
    }

    public int mapHeight() {
        return height;
    }

    public int mapWidth() {
        return width;
    }
}
