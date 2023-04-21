using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public readonly SyncList<int> playerid = new SyncList<int>();
    private int currentTurn = 0;

    public int getCurTurnID() {
        return currentTurn;
    }

    int zaehler = 0;
    
    void Update() {
        if(zaehler % 1000 == 0 && zaehler != 0) {
            currentTurn++;
            if(currentTurn > playerid.Count) {
                currentTurn = 0;
            }
        }
        zaehler++;
    }
}
