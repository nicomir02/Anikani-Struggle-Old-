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
    [SerializeField] public List<Color> spielFarben = new List<Color>();

    public readonly SyncDictionary<int, string> playernames = new SyncDictionary<int, string>();


    //Liste hier, da bei Player Probleme mit Sync gab
    public readonly SyncList<int> disqualifiedPlayers = new SyncList<int>();

    [SerializeField] private int minAbstandMainBuilding = 20;
    

    public int getMinAbstandMainBuilding() {
        return minAbstandMainBuilding;
    }

    public void setMinAbstandMainBuilding(int distance) {
        minAbstandMainBuilding = distance;
    }


    [Command(requiresAuthority = false)]
    public void syncNames() {
        addPlayer();
    }

    [ClientRpc]
    public void addPlayer() {
        if(!playernames.ContainsKey(GetComponent<RoundManager>().id)) addInDictionary(GetComponent<RoundManager>().id, GetComponent<RoundManager>().playername);
    }

    [Command(requiresAuthority=false)]
    public void addInDictionary(int id, string pname) {
        if(!playernames.ContainsKey(id)) playernames.Add(id, pname);
    }

    //Methode fürs disqualifien
    [Command(requiresAuthority = false)]
    public void spielerDisqualifizieren(int id) {
        disqualifiedPlayers.Add(id);
        List<Vector3Int> list = new List<Vector3Int>();

        foreach(KeyValuePair<Vector3Int, int> kvp in teamVecs) {
            if(kvp.Value == id) {
                list.Add(kvp.Key);
            }
        }

        foreach(Vector3Int v in list) {
            teamVecs.Remove(v);
        }
        foreach(BuildingManager bm in FindObjectsOfType<BuildingManager>()) {
            bm.CMDallReloadArea();
        }
    }

    [Command(requiresAuthority=false)]
    public void removeTeamVec(Vector3Int v) {
        teamVecs.Remove(v);
        foreach(BuildingManager bm in FindObjectsOfType<BuildingManager>()) {
            bm.CMDallReloadArea();
        }
    }

    //Methode um zu überprüfen ob Spieler disqualifiziert ist
    public bool isDisqualified(int id) {
        return disqualifiedPlayers.Contains(id);
    }


    //Vector zu einem Team hinzufügen
    [Command(requiresAuthority = false)]
    public void addVec(Vector3Int vec, int id) {
        if(!GetComponent<TilemapHover>().insideField(vec)) return;
        if(!teamVecs.ContainsKey(vec)) teamVecs.Add(vec, id);
        foreach(BuildingManager bm in FindObjectsOfType<BuildingManager>()) {
            bm.CMDallReloadArea();
        }
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
        vec.z = 0;
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
