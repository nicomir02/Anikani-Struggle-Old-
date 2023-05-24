using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding 
{
    Vector3Int start;
    Vector3Int end;
    Knoten kuerzesterWeg = null;
    List<Vector3Int> path = new List<Vector3Int>();

    //Konstruktor
    public Pathfinding(Vector3Int startTile, Vector3Int zielTile)
    {
        this.start = startTile;
        this.end = zielTile;
        this.end.z = 2;
    }

    public List<Vector3Int> shortestPath()
    {
        List<Knoten> offeneListe = new List<Knoten>(); //Knoten die noch überprüft werden müssen
        List<Knoten> geschlosseneListe = new List<Knoten>(); // Knoten die schon geprüft wurden

        Knoten startKnoten = new Knoten(start, end, 0, null);
        Knoten ZielKnoten = new Knoten(end, end, 0, null); //  0???????

        //Startknoten der Liste hinzufügen
        offeneListe.Add(startKnoten);

        Debug.Log("vorWhileSchleife");
        int testzaehler = 0;

        while(offeneListe.Count > 0)
        {
            testzaehler++;
            //if(testzaehler > 20) break;
            
            Knoten aktiverKnoten = lowestCost(offeneListe); //neuen aktiven Knoten aus der Liste der Nachbarn suchen

            Debug.Log("aktiverKnoten: " + aktiverKnoten.position + ", AbstandS: " + aktiverKnoten.G + ", AbstandE: " + aktiverKnoten.H + ", best?: " + aktiverKnoten.F);
            Debug.Log("endKonten: " + end);
            Debug.Log(aktiverKnoten.position == end);
            Debug.Log(offeneListe.Count);
            /*foreach(Knoten p in offeneListe){
                Debug.Log("offeneListe" + p);
            }*/

            offeneListe.Remove(aktiverKnoten);
            geschlosseneListe.Add(aktiverKnoten); 
            
            //wenn der aktive Knoten der ZielKnoten ist -> FERTIG
            if(aktiverKnoten.position == end) {
                kuerzesterWeg = aktiverKnoten;

                Knoten temp1 = kuerzesterWeg;

                while(temp1.vorgaenger != null) {
                    path.Add(temp1.position);
                    temp1 = temp1.vorgaenger;
                }
                path.Reverse();
                return path;
            }

            List<Knoten> alleNachbarn = nachbarnFinden(aktiverKnoten, end); //liste mit allen Nachbarn drin zurückgeben, die in Frage kommen

            /* ist der schon in der offenen Liste? 
            nein -> hinzufügen
            ja -> gucken ob der aktuelle abstand länger ist als wenn ich über den jetzigen Knoten gehe
                    nein -> alles so lassen
                    ja -> abstand F aktualisieren
            */

            foreach (Knoten nachbar in alleNachbarn) 
            {
                Debug.Log(nachbar.position + "G= "+nachbar.G + "H= " + nachbar.H + "F= " + nachbar.F);
                if(geschlosseneListe.Contains(nachbar)) continue;

                if(aktiverKnoten.G+1 < nachbar.G){
                    nachbar.G = aktiverKnoten.G+1;
                    nachbar.F = Mathf.Abs(nachbar.position.x - end.x) + Mathf.Abs(nachbar.position.y - end.y) + Mathf.Abs(nachbar.position.z - end.z);
                    nachbar.vorgaenger = aktiverKnoten;
                }
                if(!offeneListe.Contains(nachbar)){
                    offeneListe.Add(nachbar);
                }
            }

        }
        Debug.Log(testzaehler);   
        //wenn alle felder ausprobiert und kein Ergebnis gefunden -> bescheid sagen dass nicht auf das Feld kommt
        return null;
    }

    Knoten lowestCost(List<Knoten> moegliche) 
    {
        //der Knoten mit dem niedrigsten F führt am wahrscheinlichsten schnell zum Ziel
        int abstand = moegliche[0].F;

        //Knoten mit dem geringsten Abstand
        Knoten probBestTile = moegliche[0];

        foreach (Knoten k in moegliche) 
        {
            if(k.F < abstand)
            {
                abstand = k.F;
                probBestTile = k;
            }
        }

        return probBestTile;
    }


    //alle Nachbarn von einem Tile finden
    public List<Knoten> nachbarnFinden(Knoten knoten, Vector3Int end) 
    {
        List<Knoten> nachbarn = new List<Knoten>(); //Liste aller Nachbarn die am ende zurückgegeben wird
        MapBehaviour mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();

        //prüfen ob man auf dem Feld stehen kann
        if(mapBehaviour.getBlockDetails((knoten.position + new Vector3Int(1,0,0))).Item2.getWalkable())
        {
            Knoten p = new Knoten(knoten.position + new Vector3Int(1,0,0), end, knoten.G+1, knoten);
            nachbarn.Add(p); //Nachbarfeld hinzufügen
        }
        if(mapBehaviour.getBlockDetails((knoten.position - new Vector3Int(1,0,0))).Item2.getWalkable()) 
        {
            Knoten p = new Knoten(knoten.position - new Vector3Int(1,0,0), end, knoten.G+1, knoten);
            nachbarn.Add(p);
        }
        if(mapBehaviour.getBlockDetails((knoten.position + new Vector3Int(0,1,0))).Item2.getWalkable()) 
        {
            Knoten p = new Knoten(knoten.position + new Vector3Int(0,1,0), end, knoten.G+1, knoten);
            nachbarn.Add(p);
        }
        if(mapBehaviour.getBlockDetails((knoten.position - new Vector3Int(0,1,0))).Item2.getWalkable()) 
        {
            Knoten p = new Knoten((knoten.position - new Vector3Int(0,1,0)), end, knoten.G+1, knoten);
            nachbarn.Add(p);
        }
        return nachbarn;
    }
}
