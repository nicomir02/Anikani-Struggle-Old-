using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitAnimator : NetworkBehaviour
{ 
    [SerializeField] private Sprite[] idleBLUE;
    [SerializeField] private Sprite[] idleRED;
    [SerializeField] private Sprite[] idleGREEN;
    [SerializeField] private Sprite[] idlePURP;
    [SerializeField] private Sprite[] moveForwardBLUE;
    [SerializeField] private Sprite[] moveBackBLUE;
    [SerializeField] private Sprite[] moveForwardRED;
    [SerializeField] private Sprite[] moveBackRED;
    [SerializeField] private Sprite[] moveForwardGREEN;
    [SerializeField] private Sprite[] moveBackGREEN;
    [SerializeField] private Sprite[] moveForwardPURP;
    [SerializeField] private Sprite[] moveBackPURP;

    private SpriteRenderer spriteRenderer; 
    private Sprite[] idle;
    private Sprite[] forward;
    private Sprite[] back;
    public new Sprite[] animation;
    bool gedreht = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        int spielerFarbe = GetComponent<UnitSprite>().id;
        
        if(spielerFarbe == 1) {        //BLAU
            forward = moveForwardBLUE;
            back = moveBackBLUE;
            idle = idleBLUE;
        } else if(spielerFarbe == 2) { //ROT
            forward = moveForwardRED;
            back = moveBackRED;
            idle = idleRED;
        } else if(spielerFarbe == 3) { //GRÜN
            forward = moveForwardGREEN;
            back = moveBackGREEN;
            idle = idleGREEN;
        } else {                       //LILA
            forward = moveForwardPURP;
            back = moveBackPURP;
            idle = idlePURP;
        }
        animation = idle;
        
    }

    // Update is called once per frame
    void Update()
    {
        int index = Mathf.FloorToInt(Time.time * 4) % animation.Length; //4 für Geschwindigkeit
        spriteRenderer.sprite = animation[index];
    }


    //Für UnitGUIPanel um richtiges Sprite auszuwählen
    public Sprite getSprite(int spielerFarbe, int auswahl) {
        if(spielerFarbe == 1) {        //BLAU
            return idleBLUE[auswahl];
        } else if(spielerFarbe == 2) { //ROT
            return idleRED[auswahl];
        } else if(spielerFarbe == 3) { //GRÜN
            return idleGREEN[auswahl];
        } else if(spielerFarbe == 4){  //LILA
            return idlePURP[auswahl];
        }
        return null;
    }

    //Vom Server changeDirection anstoßen
    [Command(requiresAuthority=false)]
    public void changeDirection(Vector3Int start, Vector3Int ziel) {
        RPCchangeDirection(start, ziel);
    }

    //richtige Sprites auswählen auf Clients
    [ClientRpc]
    public void RPCchangeDirection(Vector3Int start, Vector3Int ziel) {
        if(start == ziel) {
            animation = idle;
        } else {
            if(start.x > ziel.x) {
                if(gedreht) rumdrehen();
                animation = forward;
            }
            if(start.y > ziel.y) {
                if(!gedreht) rumdrehen();
                animation = forward;
            }
            if(start.x < ziel.x) {
                if(!gedreht) rumdrehen();
                animation = back;
            }
            if(start.y < ziel.y) {
                if(gedreht) rumdrehen();
                animation = back;
            }
        }
    }

    //Sprite rumdrehen
    void rumdrehen() {
        GetComponent<Transform>().Rotate(new Vector3(0f, 180f, 0f));
        gedreht = !gedreht;
    }
}
