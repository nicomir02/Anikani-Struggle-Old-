using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    public readonly SyncDictionary<Vector3Int, int> teamVecs = new SyncDictionary<Vector3Int, int>();

    [Command(requiresAuthority = false)]
    public void addVec(Vector3Int vec, int id) {
        teamVecs.Add(vec, id);
    }

    public bool hasVec(Vector3Int vec) {
        return teamVecs.ContainsKey(vec);
    }

    public int getID(Vector3Int vec) {
        return teamVecs[vec];
    }

    public Dictionary<Vector3Int, int> getDictionary() {
        Dictionary<Vector3Int, int> vecs = new Dictionary<Vector3Int, int>();
        foreach(KeyValuePair<Vector3Int, int> kvp in teamVecs) {
            vecs.Add(kvp.Key, kvp.Value);
        }
        return vecs;
    }

    public Color getColorByID(int id) {
        if(id==1) {
            return Color.blue;
        }else if(id==2) {
            return Color.red;
        }
        return Color.white;
    }
}
