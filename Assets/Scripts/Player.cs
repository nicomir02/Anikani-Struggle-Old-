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
    UnitManager unitManager;
    

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
        unitManager = GetComponent<UnitManager>();
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
                    Vector3Int vec = pair.Key;
                    tilemap.SetTileFlags(vec, TileFlags.None);
                    tilemap.SetColor(vec, Color.white);
                    vec.z = 0;
                    tilemap.SetTileFlags(vec, TileFlags.None);
                    tilemap.SetColor(vec, Color.white);
                }
                gefaerbt = false;
            }else {
                Dictionary<Vector3Int, int> liste = gameManager.getList(id); //kann rein theoretisch einmal for dem if-else stehen statt in beiden. anderer gleicher code auch ausgelagert in methode oder nur einmal

                foreach(KeyValuePair<Vector3Int, int> pair in liste) {
                    Vector3Int vec = pair.Key;
                    tilemap.SetTileFlags(vec, TileFlags.None);
                    tilemap.SetColor(vec, gameManager.getColor(pair.Value));
                    vec.z = 0;
                    tilemap.SetTileFlags(vec, TileFlags.None);
                    tilemap.SetColor(vec, gameManager.getColor(pair.Value));
                }
                gefaerbt = true;
            }
        }
    }


}
