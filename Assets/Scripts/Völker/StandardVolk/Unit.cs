using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class Unit : NetworkBehaviour
{
    [SerializeField] private string unitname = "";  //Name der Einheit
    [SerializeField] private int leben = 100;   //Lebenspunkte
    [SerializeField] private int angriffswert = 30; //Angriffsstärke
    [SerializeField] private int maxBloeckeProRunde = 4;    //Bewegungsreichweite

    [SerializeField] private int trainRounds = 2; //anzahl der Runden zum Rekrutieren/Trainieren der Einheit

    [SerializeField] private int price = 3; //Kosten der einheit zum rekrutieren(momentan nur Holz)


    [SerializeField] private int kampfweite = 1; //angriffsreichweite

    [SerializeField] private int heilung = 0; 

    [SerializeField] private int standardSprite = 0;

    //Standard Sachen werden hier nicht rein gemacht, aber beim Engel bspw Wasser
    //Hier kann er nicht drauf stehen bleiben aber überqueren
    [SerializeField] private List<Block> canWalkOver = new List<Block>(); 

    
    public bool canWalk(Block b) {
        return canWalkOver.Contains(b);
    }

    //Wie viel kann die Unit andere heilen
    public int getHeilung() {
        return heilung;
    }

    //Getter Methoden:
    public int getKampfweite() {
        return kampfweite;
    }

    public string getName() {
        return unitname;
    }

    public Sprite getSprite(int playerid) {
        return GetComponent<UnitAnimator>().getSprite(playerid, standardSprite);
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
