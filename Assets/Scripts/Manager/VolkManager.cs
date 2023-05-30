using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolkManager : MonoBehaviour
{


//Instanzvariable
    [SerializeField] public List<Volk> volkList = new List<Volk>();    //Liste aller Völker(im GameManager bei Unity erweiterbar), später Auswahl in Lobby im LobbyManager, 

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
        if(v.isBarrackBuilding(b)) return 3;
        if(v.isStoneBuilding(b)) return 4;
        return -1;
    }

    public Volk getVolkByString(string volkname) {
        foreach(Volk v in volkList) {
            if(v.name == volkname) return v;
        }
        return null;
    }

    //Building herausfinden mit id
    public Building getBuildingByID(Volk v, int buildingID, int lvl) {
        if(buildingID == 1) {
            return v.getHomeBuilding(lvl);
        }else if(buildingID == 2) {
            return v.getTreeBuilding(lvl);
        }else if(buildingID == 3) {
            return v.getBarrackBuilding(lvl);
        }else if(buildingID == 4) {
            return v.getStoneBuilding(lvl);
        }
        return null;
    }
}

