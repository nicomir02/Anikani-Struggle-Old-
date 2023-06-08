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
        vec.z = 1;
        if(health.ContainsKey(vec)) return;
        if(listVec.Count > 0) {
            health.Add(vec, leben);
            foreach(Vector3Int v in listVec) {
                if(!building.ContainsKey(v)) building.Add(new Vector3Int(v.x, v.y, 1), vec);
            }
        }
    }

    public List<Vector3Int> getAllVecsOnZero() {
        List<Vector3Int> liste = new List<Vector3Int>();
        foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in building) {
            liste.Add(new Vector3Int(kvp.Key.x, kvp.Key.y, 0));
        }
        return liste;
    }

    //schaut ob an diesem Vektor leben gespeichert gewerden
    public bool isHealth(Vector3Int vec) {
        bool b = false;
        if(isUnit(vec)) b = true;
        
        if(isBuilding(vec)) b = true;
        return b;
    }

    [Command(requiresAuthority = false)]
    public void removeBuilding(Vector3Int vec) {
        vec.z = 1;
        if(building.ContainsKey(vec)) {
            Vector3Int temp = building[vec];
            health.Remove(temp);
            
            List<Vector3Int> removeList = new List<Vector3Int>();
            foreach(KeyValuePair<Vector3Int, Vector3Int> kvp in building) {
                if(kvp.Value == temp) removeList.Add(kvp.Key);
            }

            foreach(Vector3Int v in removeList) {
                building.Remove(v);
            }
            
        }
    }
    
    //vorherige Methode noch unter verwendung, neue isHealth()
    public bool isUnit(Vector3Int vec) {
        return health.ContainsKey(new Vector3Int(vec.x, vec.y, 2));
    }

    //vorherige Methode noch unter verwendung, neue isHealth()
    public bool isBuilding(Vector3Int vec) {
        vec.z = 1;
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
        if(building.ContainsKey(vec)) health[building[vec]] -= angriffswert;
    }
    
    //Hinzufügen von Einheit
    [Command(requiresAuthority = false)]
    public void addUnit(Vector3Int vec, int leben) {
        if(!health.ContainsKey(vec)) {
            health.Add(vec, leben);
        }else {
            health[vec] = leben;
        }
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
        if(health.ContainsKey(vec)) {
            health[vec] -= angriff;
            ChangeBar(vec, angriff);
        }
        angegriffenVec = vec;
    }

    [ClientRpc]
    public void ChangeBar(Vector3Int vec, int angriff) {
        vec.z = 2;
        foreach(UnitSprite us in FindObjectsOfType<UnitSprite>()) {
            if(us.vec == vec) {
                HealthBar bar = us.gameObject.transform.GetChild(0).GetComponent<HealthBar>();
                bar.Change(-angriff);
            }
        }
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