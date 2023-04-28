using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class Volk : NetworkBehaviour
{
      [SerializeField] List<Unit> units = new List<Unit>();
      [SerializeField] List<Building> buildings = new List<Building>();
    
      public void setBuilding(int buildingid, int idColor, Tilemap tilemap, Vector3Int vec) {
         buildings[buildingid].setTile(tilemap, vec, idColor);
      }

      public Building getBuilding(int buildingid) {
         return buildings[buildingid];
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
