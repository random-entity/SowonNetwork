using UnityEngine;

public class MouseToCamera : MonoBehaviour
{
    private Vector3 panOrigin;
    private Vector3 oldPos;
    [SerializeField] private float panSpeed;
    [SerializeField] float minFov = 15f;
    [SerializeField] float maxFov = 90f;
    [SerializeField] float zoomSensitivity;
    private void Update()
    {
        mousePan();
        mouseZoom();
    }

    private void mousePan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            oldPos = transform.position;
            panOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition); // Get the ScreenVector the mouse clicked
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - panOrigin; //Get the difference between where the mouse clicked and where it moved
            transform.position = oldPos + -pos * panSpeed; //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
        }
    }

    private void mouseZoom()
    {
        float fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
    }
}