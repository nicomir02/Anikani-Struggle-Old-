using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class GameManager : NetworkBehaviour
{
    //Variable zum Prüfen ob Cheats angeschaltet wurden für Spezielle noch zu implementierende Spielsituationen
    private bool cheatsOn = false;

    [SerializeField] private TileBase fogOfWar;

    //Liste der Vektoren/Tiles die den einzelnen Spielern gehören(Spielerfelder)
    public readonly SyncDictionary<Vector3Int, int> teamVecs = new SyncDictionary<Vector3Int, int>();

//Auflisten bei Unity bei den GameObjects ders Szene für verschiedene Teamfarben
    [SerializeField] public List<Color> spielFarben = new List<Color>();

    [SerializeField] private MapBehaviour mapBehaviour;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private Color unsichtbar;
    [SerializeField] private HealthManager healthManager;

    public List<Vector3Int> aufgedecktDurchBuildings = new List<Vector3Int>();
    public Dictionary<Vector3Int, List<Vector3Int>> aufgedecktDurchUnits = new Dictionary<Vector3Int, List<Vector3Int>>();

    public readonly SyncDictionary<int, string> playernames = new SyncDictionary<int, string>();

    private bool generated = false;


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
        v.z = 0;
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
        vec.z = 0;
        if(!GetComponent<TilemapHover>().insideField(vec)) return;
        if(!teamVecs.ContainsKey(vec)) teamVecs.Add(vec, id);
        foreach(BuildingManager bm in FindObjectsOfType<BuildingManager>()) {
            bm.CMDallReloadArea();
        }
        if(roundManager.round > 0) reloadFogOfWar();
    }

    [ClientRpc]
    public void reloadFogOfWar() {
        onChangeFogOfWar();
    }

    //Gehört der Vektor schon zu einem Team Prüfung
    public bool hasVec(Vector3Int vec) {
        vec.z = 0;
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

    public void onFogOfWar() {
        if(!generated) {
            int maxX = mapBehaviour.mapWidth()+6;
            int maxY = mapBehaviour.mapHeight()+6;
            for(int x=-6;x<maxX;x++) {
                for(int y=-6;y<maxY;y++) {
                    Vector3Int newV = new Vector3Int(x-2,y-2,4);
                    Vector3Int vector = new Vector3Int(x,y,4);
                    tilemap.SetTile(newV, fogOfWar);
                    tilemap.SetTileFlags(new Vector3Int(x, y, 1), TileFlags.None);
                    tilemap.SetColor(new Vector3Int(x, y, 1), unsichtbar);
                }
            }
            onChangeFogOfWar();
            generated = true;
        }
    }

    public void onChangeFogOfWar() {
        Vector3Int vec = new Vector3Int(-2, -2, 0);
        foreach(KeyValuePair<Vector3Int, int> kvp in teamVecs) {
            if(kvp.Value == roundManager.id) {
                Vector3Int v = kvp.Key;
                for(int x=-2;x<4;x++) {
                    for(int y=-2;y<4;y++) {
                        v = new Vector3Int(x,y,4);
                        Vector3Int tile = kvp.Key+v+vec;
                        tile.z = 4;
                        tilemap.SetTile(tile, null);
                        aufgedecktDurchBuildings.Add(tile);
                        v = kvp.Key+v;
                        v.z = 1;
                        tilemap.SetColor(v, Color.white);
                       
                    }
                }
            }
        }
    }

    public void fogOfWarEnemyAddBuilding(Vector3Int vec) {
        Vector3Int vector = vec;
        vec.y-=2;
        vec.x-=2;
        vec.z = 4;

        bool aufgedeckt = false;
        foreach(KeyValuePair<Vector3Int, List<Vector3Int>> kvp in aufgedecktDurchUnits) {
            if(kvp.Value.Contains(vec)) aufgedeckt = true;
        }

        if(!aufgedecktDurchUnits.ContainsKey(vec) || !aufgedeckt) {
            tilemap.SetTileFlags(vector, TileFlags.None);
            tilemap.SetColor(vector, unsichtbar);
        }
    }

    public bool isAufgedeckt(Vector3Int vec) {
        vec.z = 4;
        bool aufgedeckt = false;
        foreach(KeyValuePair<Vector3Int, List<Vector3Int>> kvp in aufgedecktDurchUnits) {
            if(kvp.Value.Contains(vec)) aufgedeckt = true;
        }


        return aufgedeckt || aufgedecktDurchBuildings.Contains(vec);
    }

    public void moveUnit(Vector3Int old, Vector3Int newVec, int maxReichweite) {
        Vector3Int vec = old;

        if(aufgedecktDurchUnits.ContainsKey(old)) aufgedecktDurchUnits.Remove(old);

        for(int x=-maxReichweite-2;x<maxReichweite+4;x++) {
            for(int y=-maxReichweite-2;y<maxReichweite+4;y++) {
                vec = new Vector3Int(x, y, 4);
                Vector3Int vecundold = vec+old;
                vecundold.z = 4;
                bool aufgedeckt = false;
                foreach(KeyValuePair<Vector3Int, List<Vector3Int>> kvp in aufgedecktDurchUnits) {
                    if(kvp.Value.Contains(vecundold)) aufgedeckt = true;
                }
                if(!aufgedecktDurchBuildings.Contains(vecundold) && !aufgedeckt) {
                    tilemap.SetTile(vecundold, fogOfWar);
                    vecundold.z = 1;
                    tilemap.SetColor(vecundold, unsichtbar);
                }
            }
        }
        List<Vector3Int> newVectors = new List<Vector3Int>();
        for(int x=-maxReichweite;x<maxReichweite+2;x++) {
            for(int y=-maxReichweite;y<maxReichweite+2;y++) {
                for(int x2=-2;x2<1;x2++) {
                    for(int y2=-2;y2<1;y2++) {
                        vec = new Vector3Int(x, y, 4) + new Vector3Int(x2,y2,0);
                        vec = vec+newVec;
                        vec.z = 1;
                        tilemap.SetColor(vec, Color.white);
                    }
                }
                vec = new Vector3Int(x, y, 4);
                vec = vec+newVec;

                vec.y-=2;
                vec.x-=2;
                vec.z = 4;
                tilemap.SetTile(vec, null);
                newVectors.Add(vec);
            }
        }
        aufgedecktDurchUnits.Add(newVec, newVectors);
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
