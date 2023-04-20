using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class Biom : NetworkBehaviour
{
    [SerializeField] private List<Block> blocks;
    [SerializeField] private string biom;
    [SerializeField] private float biommultiplier = 1f;

    private System.Random rand = new System.Random(); //Für Zufallsvariablen

    public Block getRandomTile() {
        return blocks[rand.Next(blocks.Count)];
    }

    public Block getBlockByIndex(int index) {
        if(index < blocks.Count) {
            return blocks[index];
        }
        return blocks[0];
    }

    public int countBlocks() {
        return blocks.Count;
    }

    public string getName() {
        return biom;
    }

    public float getBiomMultiplier() {
        return biommultiplier;
    }
}
