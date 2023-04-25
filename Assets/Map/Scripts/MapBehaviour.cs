using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class MapBehaviour : NetworkBehaviour
{
    //BlockDetails
    public struct BlockDetails {
        public int Biomindex;
        public int Blockindex;
        public bool Ressourcenbool;
        public int ressindex;

        public BlockDetails(int biom, int block) {
            Biomindex = biom;
            Blockindex = block;
            Ressourcenbool = false;
            ressindex = 0;
        }

        public BlockDetails(int biom, int block, int ress) {
            Biomindex = biom;
            Blockindex = block;
            Ressourcenbool = true;
            ressindex = ress;
        }
    }

    [SerializeField] private Biom[] biome;
    [SerializeField] private Ressource[] ressourcen;

    //SyncDictionary<Vector3Int, (Biomindex, Blockindex, Ressourcenbool, ressindex)> eindeutiger als vorher
    public readonly SyncDictionary<Vector3Int, BlockDetails> blockDetails = new SyncDictionary<Vector3Int, BlockDetails>();

    //Random
    private System.Random rand = new System.Random();

    //Einstellungen Größe der Karte
    [SerializeField] private int width = 25; 
    [SerializeField] private int height = 25;
    [SerializeField] private int biomgroesse = 250;

    //Karteneinstellungen
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Block randTile;

    private HashSet<Vector3Int> vectors = new HashSet<Vector3Int>();

    public void buildTerrain() {
        foreach(KeyValuePair<Vector3Int, BlockDetails> kvp in blockDetails) {
            if(kvp.Value.Ressourcenbool) {
                ressourcen[kvp.Value.ressindex].getBlock().setTile(kvp.Key, tilemap);
            }else {
                biome[kvp.Value.Biomindex].getBlockByIndex(kvp.Value.Blockindex).setTile(kvp.Key, tilemap);
            }
        }
        createRand();
    }

    public void createTerrain() {
        resetAll();

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
                int indexbiom = 0;

                for(i=0; i<biome.Length; i++) {
                    if(biome[i] == biom) {
                        indexbiom=0;
                        break;
                    }
                }

                blockDetails.Add(vect, new BlockDetails(indexbiom, indexblock));
            }
        }
        
        Dictionary<Vector3Int, BlockDetails> vecBlock = new Dictionary<Vector3Int, BlockDetails>();

        foreach(KeyValuePair<Vector3Int, BlockDetails> kvp in blockDetails) {
            Vector3Int vec = kvp.Key;
            Biom b = getBiomByVec(vec);
            int rindex = rand.Next(ressourcen.Length);
            Ressource r = ressourcen[rindex];
            float pro = r.getProb(biome[kvp.Value.Biomindex]);
            int x = 10000;
            if(rand.Next(x) <= pro*x) {
                r.getBlock().setTile(vec, tilemap);
                BlockDetails bdetails = blockDetails[vec];
                bdetails.Ressourcenbool = true;
                bdetails.ressindex = rindex;
                vecBlock.Add(vec, bdetails);
            }
        }

        foreach(KeyValuePair<Vector3Int, BlockDetails> kvp in vecBlock) {
            blockDetails.Remove(kvp.Key);
            blockDetails[kvp.Key] = kvp.Value;
        }

        createRand();
    }

    public Biom getBiomByVec(Vector3Int vec) {
        return biome[blockDetails[vec].Blockindex];
    }

    void createBiom() {
        List<Vector3Int> neighbors = new List<Vector3Int>(); //Verfügbare Nachbarn Speicher
        Vector3Int vec = rndmValidVec(); //Anfangsvektor

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

        vectors.Remove(vec);

        blockDetails.Add(vec, new BlockDetails(indexbiom, indexblock));

        for(int i=0; i<=biomgroesse*b.getBiomMultiplier(); i++) {
            if(neighbors.Count > 0) {
                vec = neighbors[rand.Next(neighbors.Count)];
                if(!blockDetails.ContainsKey(vec)) {
                    indexblock = rand.Next(b.countBlocks());
                    rndm = b.getBlockByIndex(indexblock);

                    blockDetails.Add(vec, new BlockDetails(indexbiom, indexblock));

                    tilemap.SetTile(vec, rndm.getTile());
                    vectors.Remove(vec);
                    neighbors.AddRange(getValidNeighbors(vec));
                    neighbors = updateNeighbors(neighbors);
                }
            }else {
                break;
            }
        }

    }

    void createRand() {
        int a=height+1;

        for(int i=-1; i<=a; i++) {
            Vector3Int vec = new Vector3Int(i, a, 0);
            randTile.setTile(vec, tilemap);
            vec = new Vector3Int(i, -1, 0);
            randTile.setTile(vec, tilemap);
        }

        a = width+1;
        for(int i=-1; i<=a; i++) {
            Vector3Int vec = new Vector3Int(a, i, 0);
            randTile.setTile(vec, tilemap);
            vec = new Vector3Int(-1, i, 0);
            randTile.setTile(vec, tilemap);
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
    
    Biom getMostNeighbors(Vector3Int vec) {
        Dictionary<Biom, int> biomCounter = new Dictionary<Biom, int>();
        Biom curBiom = null;

        for(int x=-1; x<=1; x++) {
            for(int y=-1; y<=1; y++) {
                Vector3Int vector = new Vector3Int(x, y, 0) + vec;
                if(blockDetails.ContainsKey(vector)) {
                    curBiom = getBiomByVec(vector);
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
        }
        return null;
    }

    Vector3Int rndmValidVec() {
        List<Vector3Int> vecs = new List<Vector3Int>(vectors);
        if(vecs.Count > 0) {
            return vecs[rand.Next(vectors.Count)];
        }
        return new Vector3Int(width*width*height*height, width*width*height*height, 0);
    }

    private void resetAll() {
        Vector3Int vec = new Vector3Int(0, 0, 0);
        for(int x=-1; x<=width; x++) {
            for(int y=-1; y<=height; y++) {
                //for(int z=-1; z<=1; y++) {
                vec = new Vector3Int(x, y, 0);
                tilemap.SetTile(vec, null);
                vectors.Add(vec);
                //}
            }
        }
    }

    public (Biom, Block, Ressource) getBlockDetails(Vector3Int vec) {
        Biom biom = biome[blockDetails[vec].Biomindex];
        Block block = biom.getBlockByIndex(blockDetails[vec].Blockindex);
        Ressource ressource = null;
        if(blockDetails[vec].Ressourcenbool) {
            ressource = ressourcen[blockDetails[vec].ressindex];
        }

        return (biom, block, ressource);
    }

    public int mapHeight() {
        return height;
    }

    public int mapWidth() {
        return width;
    }
}
