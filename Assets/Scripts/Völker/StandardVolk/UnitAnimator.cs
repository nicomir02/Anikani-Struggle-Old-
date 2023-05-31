using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{ 
    public Sprite[] idle;
    public Sprite[] idleChoice;
    public Sprite[] moveForwardBLUE;
    public Sprite[] moveBackBLUE;
    public Sprite[] moveForwardRED;
    public Sprite[] moveBackRED;
    public Sprite[] moveForwardGREEN;
    public Sprite[] moveBackGREEN;
    public Sprite[] moveForwardPURP;
    public Sprite[] moveBackPURP;

    private SpriteRenderer spriteRenderer; 
    private Sprite[] forward;
    private Sprite[] back;
    public Sprite[] animation;
    bool gedreht = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        int spielerFarbe = GetComponent<UnitSprite>().id;

        if(spielerFarbe == 1) {        //BLAU
            forward = moveForwardBLUE;
            back = moveBackBLUE;
        } else if(spielerFarbe == 2) { //ROT
            forward = moveForwardRED;
            back = moveBackRED;
        } else if(spielerFarbe == 3) { //GRÜN
            forward = moveForwardGREEN;
            back = moveBackGREEN;
        } else {                       //LILA
            forward = moveForwardPURP;
            back = moveBackPURP;
        }
        idle[0] = idleChoice[spielerFarbe-1];
        animation = idle;
        
    }

    // Update is called once per frame
    void Update()
    {
        int index = Mathf.FloorToInt(Time.time * 4) % animation.Length; //4 für Geschwindigkeit
        Debug.Log(idle.Length);
        spriteRenderer.sprite = animation[index];
    }

    //richtige Sprites auswählen
    public void changeDirection(Vector3Int start, Vector3Int ziel) {
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
