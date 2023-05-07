using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolkManager : MonoBehaviour
{


//Instanzvariable
    [SerializeField] private List<Volk> volkList = new List<Volk>();    //Liste aller Völker(im GameManager bei Unity erweiterbar), später Auswahl in Lobby im LobbyManager, 

//Getter für ID des Volkes um auf das Volk zugreifen zu können
    public (bool, int) getVolkID(Volk v) {
        for(int i=0; i<volkList.Count; i++) {
            if(volkList[i] == v) return (true, i);
        }
        return (false, 0);
    }
//Getter für das spezifische Volk an einer bestimmten Stelle in der Liste(um deren Einheiten/Gebäude zu nutzen)
    public Volk getVolk(int id) {
        return volkList[id];
    }

    //Herausfinden was für ein Building mit einer ID
    public int getBuildingID(Volk v, Building b) {
        if(v.isHomeBuilding(b)) return 1;
        if(v.isTreeBuilding(b)) return 2;
        return -1;
    }

    //Building herausfinden mit id
    public Building getBuildingByID(Volk v, int buildingID, int lvl) {
        if(buildingID == 1) {
            return v.getHomeBuilding(lvl);
        }else if(buildingID == 2) {
            return v.getTreeBuilding(lvl);
        }
        return null;
    }
}
