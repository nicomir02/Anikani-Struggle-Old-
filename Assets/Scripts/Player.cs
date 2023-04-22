using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Player : NetworkBehaviour
{
    private GameManager gameManager;
    private int id;
    private static int allids = 0;

    private Button button;

    private bool gefaerbt = false;
    BuildingManager buildingManager;
    

    public int getID() {
        return id;
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        id = allids++;
        if(base.isOwned) addPlayer();
        button = GameObject.Find("ButtonGebiet").GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        buildingManager = GetComponent<BuildingManager>();
    }

    [Command]
    public void addPlayer() {
        gameManager.addID();
    }

    public void OnButtonClick() {
        if(buildingManager.getFirstBuilding()) {
            Tilemap tilemap = buildingManager.getTilemap();
            if(gefaerbt) {
                Dictionary<Vector3Int, int> liste = gameManager.getList(id);
                foreach(KeyValuePair<Vector3Int, int> pair in liste) {
                    tilemap.SetTileFlags(pair.Key, TileFlags.None);
                    tilemap.SetColor(pair.Key, Color.white);
                }
                gefaerbt = false;
            }else {
                Dictionary<Vector3Int, int> liste = gameManager.getList(id);

                foreach(KeyValuePair<Vector3Int, int> pair in liste) {
                    tilemap.SetTileFlags(pair.Key, TileFlags.None);
                    tilemap.SetColor(pair.Key, gameManager.getColor(pair.Value));
                }
                gefaerbt = true;
            }
        }
    }


}
