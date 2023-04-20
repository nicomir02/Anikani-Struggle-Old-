using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class MapBehaviour : NetworkBehaviour
{
    [SerializeField] private SaveManager saveManager;

    [SerializeField] private int width = 25;
    [SerializeField] private int height = 25;

    [SerializeField] private Block[] tile;
    [SerializeField] private Tilemap tilemap;

    private System.Random rand = new System.Random();

    public void createTerrain() {
        Dictionary<Vector3Int, Block> vecBloecke = new Dictionary<Vector3Int, Block>();
        List<Vector3Int> vecs = new List<Vector3Int>();
        List<Block> bloecke = new List<Block>();
        int i=0;
        for(int x=0; x<=width; x++) {
            for(int y=0;y<=height; y++) {
                Vector3Int vec = new Vector3Int(x, y, 0);
                Block b = tile[rand.Next(tile.Length)]; 
                tilemap.SetTile(vec, b.getTile());
                saveManager.vecBloecke.Add(vec, b);
                //addVec(vec, b);
                i++;
            }
        }
    }

    [Command]
    public void addVec(Vector3Int vec, Block b) {
        saveManager.vecBloecke.Add(vec, b);
    }

    public void buildTerrain() {
        Dictionary<Vector3Int, Block> vecBloecke = saveManager.getBloecke();
        foreach(KeyValuePair<Vector3Int, Block> pair in vecBloecke) {
            tilemap.SetTile(pair.Key, pair.Value.getTile());
        }
    }
}
