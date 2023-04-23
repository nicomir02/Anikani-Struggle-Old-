using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Ressource : NetworkBehaviour
{
    public string ressourceName;
    public Block ressource;
    public float generalProb;

    public List<Biom> biome;
    public List<float> probabilities;


    public float getProb(Biom b) {
        if(biome.Contains(b)) {
            return probabilities[biome.IndexOf(b)];
        }
        return generalProb;
    }

    public TileBase getRessourceTile() {
        return ressource.getTile();
    }

    public Block getBlock() {
        return ressource;
    }
}
