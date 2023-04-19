using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class MapBehaviour : NetworkBehaviour
{
    //Mapdaten
    [SerializeField] private int width = 25;
    [SerializeField] private int height = 25;
    [SerializeField] private Tilemap tilemap;

    //Blöcke
    [SerializeField] private Block[] bloecke;

    //Zufallsvariable
    private System.Random rand = new System.Random();

    private readonly SyncDictionary<Vector3Int, Block> vecBloecke = new SyncDictionary<Vector3Int, Block>();

    void Start() {
        createTerrain();
    }

    public void createTerrain() {
        Vector3Int vec = new Vector3Int(0, 0, 0);
        if(vecBloecke.ContainsKey(vec)) {
            for(int x=0; x<=width; x++) {
                for(int y=0; y<=height; y++) {
                    vec = new Vector3Int(x, y, 0);
                    tilemap.SetTile(vec, vecBloecke[vec].getTile());
                }
            }
        }else {
            for(int x=0; x<=width; x++) {
                for(int y=0; y<=height; y++) {
                    vec = new Vector3Int(x, y, 0);
                    Block b = bloecke[rand.Next(bloecke.Length)];
                    tilemap.SetTile(vec, b.getTile());
                    vecBloecke.Add(vec, b);
                    
                }
            }
        }
    }

    public Tilemap getTilemap() {
        return this.tilemap;
    }
}
