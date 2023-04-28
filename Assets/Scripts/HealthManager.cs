using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthManager : NetworkBehaviour
{
    readonly SyncDictionary<Vector3Int, int> healthUnits = new SyncDictionary<Vector3Int, int>();
    
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