using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthManager : NetworkBehaviour
{
    private readonly SyncDictionary<Vector3Int, int> healthUnits = new SyncDictionary<Vector3Int, int>();

    private readonly SyncDictionary<Vector3Int, Vector3Int> building = new SyncDictionary<Vector3Int, Vector3Int>();


    [SyncVar] public Vector3Int angegriffenVec;
    

    private MapBehaviour mapBehaviour;

    void Start() {
        mapBehaviour = GameObject.Find("GameManager").GetComponent<MapBehaviour>();
    }

    [Command(requiresAuthority = false)]
    public void addBuilding(List<Vector3Int> listVec, int health, Vector3Int vec) {
        if(listVec.Count > 0) {
            healthUnits.Add(vec, health);
            foreach(Vector3Int v in listVec) {
                building.Add(new Vector3Int(v.x, v.y, 1), vec);
            }
        }
    }

    public bool isHealth(Vector3Int vec) {
        bool b = false;
        if(healthUnits.ContainsKey(vec)) b = true;
        vec.z = 1;
        if(healthUnits.ContainsKey(vec)) b = true;
        if(building.ContainsKey(vec)) b = true;
        return b;
    }

    public bool isUnit(Vector3Int vec) {
        return healthUnits.ContainsKey(vec);
    }

    public bool isBuilding(Vector3Int vec) {
        return building.ContainsKey(vec);
    }

    public int getBuildingLeben(Vector3Int vec) {
        if(building.ContainsKey(vec) && healthUnits.ContainsKey(building[vec])) return healthUnits[building[vec]];
        return 0;
    }

    [Command(requiresAuthority = false)]
    public void angriffBuilding(Vector3Int vec, int angriffswert) {
        vec.z = 1;
        healthUnits[building[vec]] -= angriffswert;
    }
    
    [Command(requiresAuthority = false)]
    public void addUnit(Vector3Int vec, int health) {
        healthUnits.Add(vec, health);
    }

    [Command(requiresAuthority = false)]
    public void moveUnit(Vector3Int oldVec, Vector3Int newVec) {
        if(healthUnits.ContainsKey(oldVec)) {
            int temp = healthUnits[oldVec];
            healthUnits.Remove(oldVec);
            healthUnits.Add(newVec, temp);
        }
    }

    [Command(requiresAuthority = false)]
    public void angriff(Vector3Int vec, int angriff) {
        healthUnits[vec] -= angriff;
        angegriffenVec = vec;
    }

    [Command(requiresAuthority = false)]
    public void noAngriff() {
        angegriffenVec = new Vector3Int(mapBehaviour.mapWidth()+2,mapBehaviour.mapHeight()+2,-1);
    }

    [Command(requiresAuthority = false)]
    public void removeUnit(Vector3Int vec) {
        healthUnits.Remove(vec);
    }

    public int getLeben(Vector3Int vec) {
        if(healthUnits.ContainsKey(vec)) return healthUnits[vec];
        return -1;
    }
    
}