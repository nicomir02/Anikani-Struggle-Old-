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
}
