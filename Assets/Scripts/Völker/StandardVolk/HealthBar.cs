using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public int MaxValue; //Später durch Max Leben von Unit ersetzen 
    [SerializeField] public int Value; //Momentane Wert (HealthManager)

    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private RectTransform topBar;

    [SerializeField] private float speed = 10f;

    private float fullWidth;
    private float targetWidth => Value * fullWidth / MaxValue;

    private Coroutine adjustBarWidthCoroutine;

    public void Change(int amount) {
        Value = Mathf.Clamp(Value+amount, 0, MaxValue); 
    
        if(adjustBarWidthCoroutine != null) {
            StopCoroutine(adjustBarWidthCoroutine);
        }

        adjustBarWidthCoroutine = StartCoroutine(AdjustBarWidth(amount));
    }

    private IEnumerator AdjustBarWidth(int amount) {
        RectTransform suddenChangeBar;
        RectTransform slowChangeBar;

        if(amount >= 0) {
            suddenChangeBar = bottomBar;
            slowChangeBar = topBar;
        }else {
            suddenChangeBar = topBar;
            slowChangeBar = bottomBar;
        }

        suddenChangeBar.SetWidth(targetWidth);
        Debug.Log("test");
        while(Mathf.Abs(suddenChangeBar.rect.width - slowChangeBar.rect.width) > 0.1f) {
            slowChangeBar.SetWidth(Mathf.Lerp(slowChangeBar.rect.width, targetWidth, Time.deltaTime * speed));
            yield return null; //new WaitForSecondsRealtime(0.1f);
        }

        slowChangeBar.SetWidth(targetWidth);
    } 

    void Start() {
        fullWidth = topBar.rect.width; 
    }
}
