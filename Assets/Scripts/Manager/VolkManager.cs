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
}
