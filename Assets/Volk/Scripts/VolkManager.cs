using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolkManager : MonoBehaviour
{

    [SerializeField] private List<Volk> volkList = new List<Volk>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public (bool, int) getVolkID(Volk v) {
        for(int i=0; i<volkList.Count; i++) {
            if(volkList[i] == v) return (true, i);
        }
        return (false, 0);
    }

    public Volk getVolk(int id) {
        return volkList[id];
    }
}
