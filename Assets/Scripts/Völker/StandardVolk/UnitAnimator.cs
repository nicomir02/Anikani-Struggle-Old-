using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public Sprite[] idle;
    public Sprite[] moveForwardL;
    //public Sprite[] moveForwardR;
    public Sprite[] moveBackL;
    //public Sprite[] moveBackR;

    private SpriteRenderer spriteRenderer; 

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        int index = Mathf.FloorToInt(Time.time * 4) % moveForwardL.Length;
        spriteRenderer.sprite = moveForwardL[index];
    }

    void rumdrehen() {
        GetComponent<Transform>().Rotate(new Vector3(0f, 180f, 0f));
    }
}
