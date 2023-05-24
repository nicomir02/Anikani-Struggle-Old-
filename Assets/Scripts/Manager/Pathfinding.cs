using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{
    Vector3Int position;
    int G = 10; //weil nur benachbarte Felder, kann man hübscher lösen aber ich lass es der Vollständigkeit halber drin, sonst komm ich durcheinander :)
    int H = 0; //Manhatten-Distand zum Ziel-Tile (also einfach die geschätzte Distanz)
    int F = G + H;

    //Konstruktor
    public Pathfinding(Vector3Int positionK, Vector3Int ziel)
    {
        position = positionK;
        H = Vector3Int.ManhattenDistance(position, ziel);
    }


    List<Vector3Int> shortestPath(Vector3Int start, Vector3Int end)
    {
        List<Pathfinding> offeneL = new List<Pathfinding>();
        List<Vector3Int> geschlosseneL = new List<Vector3Int>();

        Pathfinding startK = new Pathfinding(start, end);
        Pathfinding zielK = new Pathfinding(end, end);

        //startknoten der Liste hinzufügen
        offeneL.Add(startK);


        while(offeneL.Count > 0)
        {
            Pathfinding aktiverK = lowestCost(offeneL); //neuen aktiven Knoten aus der Liste der Nachbarn suchen

            offeneL.Remove(aktiverK);
            geschlosseneL.Add(aktiverK.position); //aktiven Knoten in die geschlossene Liste hinzufügen
            
            if(aktiverK.position == end) break; //wenn der aktive Knoten der ZielKnoten ist -> FERTIG

            List<Pathfinding> alleNachbarn = nachbarnFinden(aktiverK, end); //liste mit allen Nachbarn drin zurückgeben, die in Frage kommen

                /* ist der schon in der offenen Liste? 
                nein -> hinzufügen
                ja -> gucken ob der aktuelle abstand länger ist als wenn ich über den jetzigen Knoten gehe
                        nein -> alles so lassen
                        ja -> abstand F aktualisieren
                */

            foreach (Pathfinding knoten in alleNachbarn) 
            {
                if(offeneL.Contains(knoten))
                {
                    if(aktiverK.F+10 > knoten.F)
                    {
                        knoten.F = aktiverK.F+10;
                    }
                }
                else
                {
                    offeneL.Add(knoten);
                }
            }

        }
        //wenn alle felder ausprobiert und kein Ergebnis gefunden -> bescheid sagen dass nicht auf das Feld kommt
        return geschlosseneL;
    }

    //Tile mit kleinster F Komponente finden
    Pathfinding lowestCost(List<Pathfinding> offeneListe) 
    {
        Pathfinding naechstesTile = null;
        int kuerzesterAbstand = offeneListe[0].F;
        foreach (Pathfinding tile in offeneListe) 
        {
            if(tile.F < kuerzesterAbstand)
            {
                kuerzesterAbstand = tile.F;
                naechstesTile = tile;
            }
        }
        return naechstesTile;
    }


    //alle Nachbarn von einem Tile finden
    public List<Pathfinding> nachbarnFinden(Pathfinding knoten, Vector3Int end) 
    {
        List<Pathfinding> nachbarn = new List<Pathfinding>(); //Liste aller Nachbarn die am ende zurückgegeben wird
        MapBehaviour mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();

        if(mapBehaviour.getBlockDetails((knoten.position + new Vector3Int(1,0,0))).Item2.getWalkable()) //wenn man auf dem Feld stehen kann
        {
            Pathfinding p = new Pathfinding(knoten.position + new Vector3Int(1,0,0), end);
            nachbarn.Add(p); //Nachbarfeld hinzufügen
        }
        if(mapBehaviour.getBlockDetails((knoten.position - new Vector3Int(1,0,0))).Item2.getWalkable()) 
        {
            Pathfinding p = new Pathfinding(knoten.position - new Vector3Int(1,0,0), end);
            nachbarn.Add(p);
        }
        if(mapBehaviour.getBlockDetails((knoten.position + new Vector3Int(0,1,0))).Item2.getWalkable()) 
        {
            Pathfinding p = new Pathfinding(knoten.position + new Vector3Int(0,1,0), end);
            nachbarn.Add(p);
        }
        if(mapBehaviour.getBlockDetails((knoten.position - new Vector3Int(0,1,0))).Item2.getWalkable()) 
        {
            Pathfinding p = new Pathfinding((knoten.position - new Vector3Int(0,1,0)), end);
            nachbarn.Add(p);
        }
        return nachbarn;
    }
}
