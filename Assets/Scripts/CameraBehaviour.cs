using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public float panSpeed = 20f;
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(1)) {
            float horizontal = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;
            transform.Translate(-horizontal, -vertical, 0);
        }

        // Zoomen der Kamera mit dem Mausrad
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f) {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - (scroll * zoomSpeed), minZoom, maxZoom);
        }
    }
}