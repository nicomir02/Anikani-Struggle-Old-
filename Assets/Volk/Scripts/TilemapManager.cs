using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TilemapManager : NetworkBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private VolkManager volkManager;

    [ClientRpc]
    private void RpcUpdateTilemap(Vector3Int vec, int volkID, int buildID, int colorID) {
        Volk v = volkManager.getVolk(volkID);
        v.setBuilding(buildID, colorID, tilemap, vec);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateTilemap(Vector3Int vec, int volkID, int buildID, int colorID)
    {
        RpcUpdateTilemap(vec, volkID, buildID, colorID);
    }
}
