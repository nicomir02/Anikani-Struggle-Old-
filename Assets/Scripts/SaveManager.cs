using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class SaveManager : NetworkBehaviour
{
    public readonly SyncDictionary<Vector3Int, Block> vecBloecke = new SyncDictionary<Vector3Int, Block>();

    [SerializeField] MapBehaviour map;

    public override void OnStartClient()
    {
        Debug.Log("hi");
        vecBloecke.Callback += OnVecBlockChange;
        Debug.Log("Wie gehts");

        foreach (KeyValuePair<Vector3Int, Block> kvp in vecBloecke) {
            OnVecBlockChange(SyncDictionary<Vector3Int, Block>.Operation.OP_ADD, kvp.Key, kvp.Value);
        }
        foreach (KeyValuePair<Vector3Int, Block> kvp in vecBloecke) {
            Debug.Log(kvp);
            Debug.Log(kvp.Key);
            Debug.Log(kvp.Value);
        }
        map.buildTerrain();
    }

    void OnVecBlockChange(SyncDictionary<Vector3Int, Block>.Operation op, Vector3Int key, Block item)
    {
        switch (op)
        {
            case SyncIDictionary<Vector3Int, Block>.Operation.OP_ADD:
                // entry added
                break;
            case SyncIDictionary<Vector3Int, Block>.Operation.OP_SET:
                // entry changed
                break;
            case SyncIDictionary<Vector3Int, Block>.Operation.OP_REMOVE:
                // entry removed
                break;
            case SyncIDictionary<Vector3Int, Block>.Operation.OP_CLEAR:
                // Dictionary was cleared
                break;
        }
    }

    public Dictionary<Vector3Int, Block> getBloecke() {
        Dictionary<Vector3Int, Block> bloecke = new Dictionary<Vector3Int, Block>();

        foreach(KeyValuePair<Vector3Int, Block> pair in vecBloecke) {
            bloecke.Add(pair.Key, pair.Value);
        }

        return bloecke;
    }
}
