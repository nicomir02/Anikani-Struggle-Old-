using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class Volk : NetworkBehaviour
{
      [SerializeField] List<Unit> units = new List<Unit>();

      [SerializeField] List<Building> homeBuildings = new List<Building>();

      [SerializeField] List<Building> treeBuildings = new List<Building>();

      [SerializeField] List<Building> barrackBuildings = new List<Building>();

      [SerializeField] List<Building> stoneBuildings = new List<Building>();

      [TextArea(5,10)]
      [SerializeField] string description = "";

      
      public string getDescription() {
         return description;
      }

      public Building getBarrackBuilding(int buildingid) {
         return barrackBuildings[buildingid];
      }

      public void setHomeBuilding(int buildingid, int idColor, Tilemap tilemap, Vector3Int vec) {
         homeBuildings[buildingid].setTile(tilemap, vec, idColor);
      }


      public int getBuildings(int a) {
         if(a == 0) {
            return homeBuildings.Count;
         }else if(a == 1) {
            return treeBuildings.Count;
         }else if(a == 2) {
            return barrackBuildings.Count;
         }else if(a == 3) {
            return stoneBuildings.Count;
         }
         return -1;
      }

      public Building getHomeBuilding(int buildingid) {
         return homeBuildings[buildingid];
      }

      public Building getTreeBuilding(int buildingid) {
         return treeBuildings[buildingid];
      }

      public Building getStoneBuilding(int buildingid) {
         return stoneBuildings[buildingid];
      }

      public bool isHomeBuilding(Building b) {
         return homeBuildings.Contains(b);
      }

      public bool isTreeBuilding(Building b) {
         return treeBuildings.Contains(b);
      }

      public bool isBarrackBuilding(Building b) {
         return barrackBuildings.Contains(b);
      }

      public bool isStoneBuilding(Building b) {
         return stoneBuildings.Contains(b);
      }

      public Building getRessourceBuilding(Ressource r, int id) {
         if(r.ressName == "Wood") {
            return getTreeBuilding(id);
         }else if(r.ressName == "Stone") {
            return stoneBuildings[id];
         }
         return null;
      }

      //für Units

      public void setUnit(int unitID, int idColor, Tilemap tilemap, Vector3Int vec) {
         units[unitID].setTile(tilemap, vec, idColor);
      }

      public Unit getUnit(int unitID) {
         return units[unitID];
      } 

      public int getUnitID(Unit unit){
         for(int i = 0; i < units.Count; i++){
            if(unit == units[i]){
               return i;
            }
         }
         return -1;
      }
}

