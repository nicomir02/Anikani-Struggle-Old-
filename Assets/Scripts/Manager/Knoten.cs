using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knoten
{
    public Vector3Int position;
    public int G = 0; //abstand zum Start
    public int H = 0; //Manhatten-Distand zum Ziel-Tile (also einfach die geschätzte Distanz)
    public int F;
    public Knoten vorgaenger = null;

    public Knoten(Vector3Int position, Vector3Int ziel, int abstandZumStart, Knoten vorher) {
        this.position = position;

        //ManhattenDistanz zum Ziel
        this.H = Mathf.Abs(position.x - ziel.x) + Mathf.Abs(position.y - ziel.y) + Mathf.Abs(position.z - ziel.z);
        
        this.G = abstandZumStart;

        this.F = G + H;

        this.vorgaenger = vorher;
    }
}
