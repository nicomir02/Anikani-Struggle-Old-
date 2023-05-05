using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthManager : NetworkBehaviour
{
    //Deklarieren von benötigten Klassen
    private MapBehaviour mapBehaviour;

    //Speicherung von leben
    private readonly SyncDictionary<Vector3Int, int> health = new SyncDictionary<Vector3Int, int>();

    //multiblockstrukturen, buildings zeigen auf ein Vektor für Variable health
    private readonly SyncDictionary<Vector3Int, Vector3Int> building = new SyncDictionary<Vector3Int, Vector3Int>();

    //zeigt aktuell angegriffen vektor an
    [SyncVar] public Vector3Int angegriffenVec;
    
    

    void Start() {
        mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
    }

    //neues Building wird gesetzt, also add für health
    [Command(requiresAuthority = false)]
    public void addBuilding(List<Vector3Int> listVec, int leben, Vector3Int vec) {
        if(listVec.Count > 0) {
            health.Add(vec, leben);
            foreach(Vector3Int v in listVec) {
                building.Add(new Vector3Int(v.x, v.y, 1), vec);
            }
        }
    }

    //schaut ob an diesem Vektor leben gespeichert gewerden
    public bool isHealth(Vector3Int vec) {
        bool b = false;
        if(health.ContainsKey(vec)) b = true;
        vec.z = 1;
        if(health.ContainsKey(vec)) b = true;
        if(building.ContainsKey(vec)) b = true;
        return b;
    }
    
    //vorherige Methode noch unter verwendung, neue isHealth()
    public bool isUnit(Vector3Int vec) {
        return health.ContainsKey(vec);
    }

    //vorherige Methode noch unter verwendung, neue isHealth()
    public bool isBuilding(Vector3Int vec) {
        return building.ContainsKey(vec);
    }

    //schaut wieviele Leben die einheit auf diesem Vektor hat
    public int getBuildingLeben(Vector3Int vec) {
        if(building.ContainsKey(vec) && health.ContainsKey(building[vec])) return health[building[vec]];
        return 0;
    }
    
    //angriff auf building
    [Command(requiresAuthority = false)]
    public void angriffBuilding(Vector3Int vec, int angriffswert) {
        vec.z = 1;
        health[building[vec]] -= angriffswert;
    }
    
    //Hinzufügen von Einheit
    [Command(requiresAuthority = false)]
    public void addUnit(Vector3Int vec, int leben) {
        health.Add(vec, leben);
    }

    //Bewegung von Einheit
    [Command(requiresAuthority = false)]
    public void moveUnit(Vector3Int oldVec, Vector3Int newVec) {
        if(health.ContainsKey(oldVec)) {
            int temp = health[oldVec];
            health.Remove(oldVec);
            health.Add(newVec, temp);
        }
    }

    //Normale Angriffsmethode
    [Command(requiresAuthority = false)]
    public void angriff(Vector3Int vec, int angriff) {
        health[vec] -= angriff;
        angegriffenVec = vec;
    }

    //Setzt angriffsvektor um
    [Command(requiresAuthority = false)]
    public void noAngriff() {
        angegriffenVec = new Vector3Int(mapBehaviour.mapWidth()+2,mapBehaviour.mapHeight()+2,-1);
    }

    //entfernen von einheit
    [Command(requiresAuthority = false)]
    public void removeUnit(Vector3Int vec) {
        health.Remove(vec);
    }

    //Lebensanzeige für Units
    public int getLeben(Vector3Int vec) {
        if(health.ContainsKey(vec)) return health[vec];
        return -1;
    }
    
}