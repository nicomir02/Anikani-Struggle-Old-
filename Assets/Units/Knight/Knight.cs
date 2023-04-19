using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class Knight : NetworkBehaviour
{
    [SerializeField] private TileBase knight;

    //Zufallsvariable
    private System.Random rand = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        Tilemap map = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        map.SetTile(new Vector3Int(rand.Next(25), rand.Next(25), 1), knight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
