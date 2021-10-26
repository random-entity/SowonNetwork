using UnityEngine;

public class MouseToCamera : MonoBehaviour
{
    public static bool trueCloseCamFalseFarCam = true;
    [SerializeField] private bool isCloseCam;

    private Vector3 panOrigin;
    private Vector3 oldPos;
    [SerializeField] private float panSpeed;
    // [SerializeField] float minFov = 15f;
    // [SerializeField] float maxFov = 90f;
    [SerializeField] float minZ;
    [SerializeField] float maxZ;
    [SerializeField] float zoomSensitivity;
    private void Update()
    {
        mousePan();

        if (isCloseCam == trueCloseCamFalseFarCam)
        {
            mouseZoom();
        }

        Vector3 pos = transform.position;

        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        pos.x = Mathf.Clamp(pos.x, EnvironmentSpecs.boundXLeft + 15f, EnvironmentSpecs.boundXRight - 15f);
        pos.y = Mathf.Clamp(pos.y, EnvironmentSpecs.boundYBottomSinked + 5f, EnvironmentSpecs.boundYTop - 15f);
   
        transform.position = new Vector3(pos.x, pos.y, pos.z);
    }

    public void setTrueCloseCamFalseFarCam(bool set)
    {
        trueCloseCamFalseFarCam = set;

        this.GetComponent<Camera>().enabled = isCloseCam == trueCloseCamFalseFarCam;
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
        // float fov = Camera.main.fieldOfView;
        // fov -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        // fov = Mathf.Clamp(fov, minFov, maxFov);
        // Camera.main.fieldOfView = fov;

        Vector3 pos = transform.position;
        pos.z += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;

        transform.position = new Vector3(pos.x, pos.y, pos.z);
    }
}