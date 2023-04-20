using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Ressource : NetworkBehaviour
{
    public string ressourceName;
    public TileBase ressource;
    public float generalProb;

    public List<Biom> biome;
    public List<int> probabilities;


    public float getProb(Biom b) {
        if(biome.Contains(b)) {
            return probabilities[biome.IndexOf(b)];
        }
        return generalProb;
    }

    public TileBase getRessourceTile() {
        return ressource;
    }
}
