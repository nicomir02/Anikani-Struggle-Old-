using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        //Kamera Bewegung mit Rechtsklick
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