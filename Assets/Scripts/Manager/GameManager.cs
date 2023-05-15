using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    //Variable zum Prüfen ob Cheats angeschaltet wurden für Spezielle noch zu implementierende Spielsituationen
    private bool cheatsOn = false;

    //Liste der Vektoren/Tiles die den einzelnen Spielern gehören(Spielerfelder)
    public readonly SyncDictionary<Vector3Int, int> teamVecs = new SyncDictionary<Vector3Int, int>();

//Auflisten bei Unity bei den GameObjects ders Szene für verschiedene Teamfarben
    [SerializeField] List<Color> spielFarben = new List<Color>();

    //Vector zu einem Team hinzufügen
    [Command(requiresAuthority = false)]
    public void addVec(Vector3Int vec, int id) {
        if(!teamVecs.ContainsKey(vec)) teamVecs.Add(vec, id);
    }

    //Gehört der Vektor schon zu einem Team Prüfung
    public bool hasVec(Vector3Int vec) {
        return teamVecs.ContainsKey(vec);
    }

    //herausfinden welche spieler-id zum Vektor gehört
    public int getID(Vector3Int vec) {
        return teamVecs[vec];
    }

    //Macht aus syncdictionary der Team Vektoren ein normales Dictionary
    public Dictionary<Vector3Int, int> getDictionary() {
        Dictionary<Vector3Int, int> vecs = new Dictionary<Vector3Int, int>();
        foreach(KeyValuePair<Vector3Int, int> kvp in teamVecs) {
            vecs.Add(kvp.Key, kvp.Value);
        }
        return vecs;
    }
    
    //Methode um herauszufinden ob der Vektor einem gegnerischen Team gehört
    public bool isEnemyArea(Vector3Int vec, int ownTeamID) {
        if(teamVecs.ContainsKey(vec)) if(teamVecs[vec] != ownTeamID) return true;
        return false;
    }

    //Gibt dir die Farbe vom Team:
    //1 = Blau
    //2 = Rot
    //weitere Farben bei Unity im GameManager
    public Color getColorByID(int id) {
        return spielFarben[id-1];
    }
    
    //Getter Methode für cheats
    public bool getCheatsOn() {
        return cheatsOn;
    }
}
