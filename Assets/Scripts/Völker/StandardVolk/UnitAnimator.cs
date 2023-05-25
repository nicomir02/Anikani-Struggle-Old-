using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public Sprite[] moveForwardL;
    private SpriteRenderer spriteRenderer; 

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveForwardL.Length > 0)
        {
            int index = Mathf.FloorToInt(Time.time * 10) % moveForwardL.Length;
            spriteRenderer.sprite = moveForwardL[index];
        }
    }
}
