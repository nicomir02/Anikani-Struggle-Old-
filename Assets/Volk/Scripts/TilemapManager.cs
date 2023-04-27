using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TilemapManager : NetworkBehaviour
{
    Tilemap tilemap;

    [ClientRpc]
    private void RpcUpdateTilemap(Vector3Int vec, Tile tile)
    {
        tilemap.SetTile(vec, tile);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateTilemap(Vector3Int vec, Tile tile)
    {
        RpcUpdateTilemap(vec, tile);
    }
}
