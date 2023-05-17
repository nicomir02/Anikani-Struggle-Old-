using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class Unit : NetworkBehaviour
{
    [SerializeField] private string unitname = "";
    [SerializeField] private int leben = 100;
    [SerializeField] private int angriffswert = 30;
    [SerializeField] private int maxBloeckeProRunde = 4;

    [SerializeField] private int trainRounds = 2;

    [SerializeField] private int price = 3;
    /* 
    [SyncVar] private int verteidigung = 0;
    [SyncVar] private int nahkampf = 10;
    [SyncVar] private int fernkampf = 5;
    [SyncVar] private int fernkampfweite = 2; 

    [SyncVar] private int BloeckeProRunde; */

    [SerializeField] List<TileBase> colourUnit = new List<TileBase>();

    public void setTile(Tilemap tilemap, Vector3Int vec, int colorFromID) {
        tilemap.SetTile(vec, colourUnit[colorFromID]);
    }

    public string getName() {
        return unitname;
    }

    public TileBase getTile(int colorFromID) {
        return colourUnit[colorFromID];
    }
    public int getLeben(){
        return leben;
    }

    public int getAngriffswert(){
        return angriffswert;
    }

    public int getMaxBloeckeProRunde() {
        return maxBloeckeProRunde;
    }

    public int getHowManyTrainRounds() {
        return trainRounds;
    }

    public int getPrice() {
        return price;
    }
    
}
