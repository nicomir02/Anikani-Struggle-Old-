using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class MapBehaviour : NetworkBehaviour
{
    [SerializeField] private Biom[] biome;
    [SerializeField] private Ressource[] ressource;

    private readonly SyncList<Vector3Int> vectorsave = new SyncList<Vector3Int>();
    
    private readonly SyncList<int> biomindex = new SyncList<int>();
    private readonly SyncList<int> blockindex = new SyncList<int>();
    //^^oberhalb Cantorsche Paarungsfunktion einrichten?^^

    private readonly SyncList<Vector3Int> ressourcevec = new SyncList<Vector3Int>();
    private readonly SyncList<int> ressindex = new SyncList<int>();

    HashSet<Vector3Int> vectors = new HashSet<Vector3Int>();
    private Dictionary<Vector3Int, Biom> alleBiome = new Dictionary<Vector3Int, Biom>();

    private System.Random rand = new System.Random(); //Für Zufallsvariablen

    [SerializeField] private int width = 25;
    [SerializeField] private int height = 25;
    [SerializeField] private int biomgroesse = 250;

    [SerializeField] private Tilemap tilemap;

    [SerializeField] private TileBase randTile;

    public (Biom, Block, Ressource) getBlockDetails(Vector3Int vec) {
        Biom biom = null;
        Block block = null;
        Ressource ress = null;

        biom = getBiomByVec(vec);
        if(ressourcevec.Contains(vec)) {
            for(int i=0; i<ressourcevec.Count; i++) {
                if(ressourcevec[i] == vec) {
                    ress = ressource[ressindex[i]];
                    block = ress.getBlock();
                    break;
                }
            }
        }else {
            for(int i=0; i<vectorsave.Count; i++) {
                if(vectorsave[i] == vec) {
                    block = biom.getBlockByIndex(blockindex[i]);
                }
            }
        }
        
        return (biom, block, ress);
    }

    public int mapWidth() {
        return width;
    }

    public int mapHeight() {
        return height;
    }

    void Start() {
        if(isServer) {
            createTerrain();
        }else {
            buildTerrain();
            //Debug.Log(getBlockDetails(new Vector3Int(0,0,0)));
        }
    }

    void createTerrain() {
        for(int x=-1; x<=1; x++) {
            for(int y=-1; y<=1; y++) {
                tilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }

        for(int x=0; x<=width; x++) {
            for(int y=0; y<=height; y++) {
                vectors.Add(new Vector3Int(x, y, 0));
            }
        }

        int i=0;
        float biomes = (width*height)/biomgroesse*1.1f; 
        while(vectors.Count > 0 || i <= Math.Ceiling(biomes)) {
            createBiom();
            i++;
        }

        foreach(Vector3Int vect in vectors) {
            if(getMostNeighbors(vect) == null) {
                createBiom();
            }else {
                Biom biom = getMostNeighbors(vect);
                int indexblock = rand.Next(biom.countBlocks());
                tilemap.SetTile(vect, biom.getBlockByIndex(indexblock).getTile());
                alleBiome[vect] = biom;
                int indexbiom = 0;
                for(i=0; i<biome.Length; i++) {
                    if(biome[i] == biom) {
                        indexbiom=0;
                        break;
                    }
                }
                saveBlock(vect, indexbiom, indexblock);
            }
        }

        for(int x=0; x<=width; x++) {
            for(int y=0; y<=height; y++) {
                vectors.Add(new Vector3Int(x, y, 0));
            }
        }

        foreach(Vector3Int vec in vectors) {
            int x = rand.Next(ressource.Length);
            Ressource ress = ressource[x];

            Biom curBiom = alleBiome[vec];

            int max = 10000;
            
            if(rand.Next(0, max) <= (ress.getProb(curBiom)*max)) {
                tilemap.SetTile(vec, ress.getRessourceTile());
                ressourcevec.Add(vec);
                ressindex.Add(x);
            }
        }

        createRand();
    }

    void createRand() {
        int a=height+1;

        for(int i=-1; i<=a; i++) {
            Vector3Int vec = new Vector3Int(i, a, 0);
            tilemap.SetTile(vec, randTile);
            vec = new Vector3Int(i, -1, 0);
            tilemap.SetTile(vec, randTile);
        }

        for(int i=-1; i<=a; i++) {
            Vector3Int vec = new Vector3Int(a, i, 0);
            tilemap.SetTile(vec, randTile);
            vec = new Vector3Int(-1, i, 0);
            tilemap.SetTile(vec, randTile);
        }
    }

    Biom getMostNeighbors(Vector3Int vec) {
        Dictionary<Biom, int> biomCounter = new Dictionary<Biom, int>();
        Biom curBiom = null;

        for(int x=-1; x<=1; x++) {
            for(int y=-1; y<=1; y++) {
                vec = new Vector3Int(x, y, 0) + vec;
                if(alleBiome.ContainsKey(vec)) {
                    curBiom = alleBiome[vec];
                    if(biomCounter.ContainsKey(curBiom)) {
                        biomCounter[curBiom]++;
                    }else {
                        biomCounter.Add(curBiom, 1);
                    }
                }
            }
        }

        if(biomCounter.Count > 0) {
            int i=0;
            foreach (KeyValuePair<Biom, int> pair in biomCounter) { 
                if(pair.Value > i) {
                    curBiom = pair.Key;
                    i = pair.Value;
                }
            }
            return curBiom;
        }else {
            return null;
        }
    }

    void createBiom() {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        Vector3Int vec = rndmValidVec();
        
        int indexbiom = rand.Next(biome.Length);
        Biom b = biome[indexbiom];
        if(getMostNeighbors(vec) != null) {
            b = getMostNeighbors(vec);
            int i=0;
            foreach(Biom bi in biome) {
                if(bi == b) {
                    break;
                }
                i++;
            }
            indexbiom = i;
        }

        int indexblock = rand.Next(b.countBlocks());
        Block rndm = b.getBlockByIndex(indexblock);
        tilemap.SetTile(vec, rndm.getTile());
        neighbors.AddRange(getValidNeighbors(vec));

        saveBlock(vec, indexbiom, indexblock);
        alleBiome.Add(vec, b);

        for(int i=0; i<=biomgroesse*b.getBiomMultiplier(); i++) {
            if(neighbors.Count > 0) {
                vec = neighbors[rand.Next(neighbors.Count)];
                if(!alleBiome.ContainsKey(vec)) {
                    indexblock = rand.Next(b.countBlocks());
                    rndm = b.getBlockByIndex(indexblock);

                    saveBlock(vec, indexbiom, indexblock);

                    tilemap.SetTile(vec, rndm.getTile());

                    neighbors.AddRange(getValidNeighbors(vec));
                    neighbors = updateNeighbors(neighbors);
                    alleBiome.Add(vec, b);
                }
            }else {
                break;
            }
        }
    }

    List<Vector3Int> updateNeighbors(List<Vector3Int> curNeighbor) {
        List<Vector3Int> result = new List<Vector3Int>();
        foreach (Vector3Int vec in curNeighbor) {
            if (tilemap.GetTile(vec) == null) {
                result.Add(vec);
            }
        }
        return result;
    }

    List<Vector3Int> getValidNeighbors(Vector3Int vec) {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for(int x=-1; x<=1; x++) {
            for(int y=-1; y<=1; y++) {
                Vector3Int vector = new Vector3Int(x, y, 0) + vec;

                if(vectors.Contains(vector)) {
                    neighbors.Add(vector);
                }
            }
        }
        return neighbors;

    }

    void buildTerrain() {
        for(int x=0; x<vectorsave.Count; x++) {
            tilemap.SetTile(vectorsave[x], getTile(x));
        }

        int i = 0;
        foreach(Vector3Int vec in ressourcevec) {
            tilemap.SetTile(vec, ressource[ressindex[i]].getRessourceTile());
            i++;
        }

        createRand();
    }

    Vector3Int rndmValidVec() {
        List<Vector3Int> list = new List<Vector3Int>(vectors);
        Vector3Int vec = list[rand.Next(vectors.Count)];
        return vec;
    }

    void saveBlock(Vector3Int vec, int indexbiom, int indexblock) {
        vectors.Remove(vec);
        vectorsave.Add(vec);
        blockindex.Add(indexblock);
        biomindex.Add(indexbiom);
    }

    Biom getBiomByVec(Vector3Int vec) {
        if(vectorsave.Contains(vec)) {
            int i=0;
            foreach(Vector3Int v in vectorsave) {
                if(v == vec) {
                    break;
                }
                i++;
            }
            return biome[biomindex[i]];
        }else {
            return null;
        }
    }

    Block getBlock(int index) {
        return biome[biomindex[index]].getBlockByIndex(index);
    }

    TileBase getTile(int index) {
        return getBlock(index).getTile();
    }
}
