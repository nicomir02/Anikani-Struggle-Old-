using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    /*
    [SyncVar] public int allPlayers = 0;
    private int currentTurn = 0;

    [SerializeField] private Color[] farben;

    public readonly SyncDictionary<Vector3Int, int> teamVecs = new SyncDictionary<Vector3Int, int>();

    public int getCurTurnID() {
        return currentTurn; //mit %allPlayers vielleicht machen damit es nicht tausende turns zählt?
    }

    public void addID() {
        allPlayers++;
    }

    public bool containsVec(Vector3Int vec) {
        return teamVecs.ContainsKey(vec);
    }

    public void addVec(Vector3Int vec, int id) {
        teamVecs.Add(vec, id);
    }

    public Dictionary<Vector3Int, int> getList(int id) {
        Dictionary<Vector3Int, int> liste = new Dictionary<Vector3Int, int>();
        foreach(KeyValuePair<Vector3Int, int> pair in teamVecs) {
            liste.Add(pair.Key, pair.Value);
        }
        return liste;
    }

    public Color getColor(int id) {
        if(farben.Length <= id) {
            return Color.blue;
        }
        return farben[id];
    }
    /*int zaehler = 0;
    
    void Update() {
        if(zaehler % 1000 == 0 && zaehler != 0) {
            currentTurn++;
            if(currentTurn > playerid.Count) {
                currentTurn = 0;
            }
        }
        zaehler++;
    }
    */
}
