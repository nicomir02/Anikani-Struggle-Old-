using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public Sprite[] idle;
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

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        int spielerFarbe = GetComponent<UnitSprite>().id;
        Debug.Log(spielerFarbe);

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
    }

    // Update is called once per frame
    void Update()
    {

        int index = Mathf.FloorToInt(Time.time * 4) % 4; //erste 4 für Geschwindigkeit, zweite 4 weil in jedem Array 4 Sprites drin sind
        Debug.Log(index);
        spriteRenderer.sprite = forward[index];
    }


    //Sprite rumdrehen
    void rumdrehen() {
        GetComponent<Transform>().Rotate(new Vector3(0f, 180f, 0f));
    }
}
