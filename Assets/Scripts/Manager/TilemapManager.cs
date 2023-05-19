using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TilemapManager : NetworkBehaviour //Synchronisieren der Tilemap zwischen Server und Spielern
{
    //Deklarieren von benötigten Klassen
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private VolkManager volkManager;

    //Änderungen auf jeden Client für Homebuilding
    [ClientRpc]
    private void RpcUpdateTilemap(Vector3Int vec, int volkID, int buildID, int colorID) {
        Volk v = volkManager.getVolk(volkID);
        v.setHomeBuilding(buildID, colorID, tilemap, vec);
    }

    //Tilemap Change für Homebuilding auf Server
    [Command(requiresAuthority = false)]
    public void CmdUpdateTilemap(Vector3Int vec, int volkID, int buildID, int colorID)
    {
        RpcUpdateTilemap(vec, volkID, buildID, colorID);
    }
    
    //Änderungen auf jeden Client für Units
    [ClientRpc]
    private void RpcUpdateTilemapUnit(Vector3Int vec, int volkID, int unitID, int colorID) {
        Volk v = volkManager.getVolk(volkID);
        v.setUnit(unitID, colorID, tilemap, vec);
    }
    
    //Tilemap Change für Units auf Server
    [Command(requiresAuthority = false)]
    public void CmdUpdateTilemapUnit(Vector3Int vec, int volkID, int unitID, int colorID)
    {
        RpcUpdateTilemapUnit(vec, volkID, unitID, colorID);
    }

    //Tilemap Change für Ressourcengebäude(generisch für alle gebäude?)
    [Command(requiresAuthority = false)]
    public void CmdUpdateTilemapBuilding(Vector3Int vec, int b, int playerID, int volkID, int lvl) {
        RpcUpdateTilemapBuilding(vec, b, playerID, volkID, lvl);
    }


    //Tilemap Change für Ressourcengebäude(generisch für alle gebäude?) auf Client
    [ClientRpc]
    private void RpcUpdateTilemapBuilding(Vector3Int vec, int b, int playerID, int volkID, int lvl) {
        Building building = volkManager.getBuildingByID(volkManager.getVolk(volkID), b, lvl);
        building.setTile(tilemap, vec, playerID-1);
    }

    //Tilemap Change für Gebäude löschen aufruf
    [Command(requiresAuthority = false)]
    public void removeBuilding(Vector3Int vec, int groesse) {
        rpcRemoveBuilding(vec, groesse);
    }

    //Tilemap Change für Gebäude löschen auf Client
    [ClientRpc]
    public void rpcRemoveBuilding(Vector3Int vec, int groesse) {
        vec.z = 1;
        foreach(Vector3Int v in makeAreaBigger(vec, groesse-2)){
            tilemap.SetTile(v, null);
        }        
    }

    public List<Vector3Int> makeAreaBigger(Vector3Int vec, int groesse) {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        //Schleifen für alle Nachbarsfelder in bestimmtem Radius
        for(int x=-groesse; x<=groesse; x++) {
            for(int y=-groesse; y<=groesse; y++) {
                Vector3Int vector = new Vector3Int(x, y, 0) + vec;
                neighbors.Add(vector);
            }
        }
        return neighbors;
    }

    [Command(requiresAuthority = false)]
    public void removeUnit(Vector3Int vec) {
        RpcRemoveUnit(vec);
    }

    [ClientRpc]
    public void RpcRemoveUnit(Vector3Int vec) {
        vec.z = 2;
        tilemap.SetTile(vec, null);
    }
}
