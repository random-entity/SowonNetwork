using System.Collections;
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

        ClampPosition(false, false);
    }

    public void ClampPosition(bool aboveSinkSurface, bool smooth)
    {
        if (smooth)
        {
            InvokeClampPositionSmooth(aboveSinkSurface);
        }
        else
        {
            Vector3 pos = transform.position;

            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            pos.x = Mathf.Clamp(pos.x, EnvironmentSpecs.boundXLeft - 2f, EnvironmentSpecs.boundXRight + 2f);

            float boundYBottom = aboveSinkSurface ? EnvironmentSpecs.boundYBottom : EnvironmentSpecs.boundYBottomSinked;
            pos.y = Mathf.Clamp(pos.y, boundYBottom - 2f, EnvironmentSpecs.boundYTop + 2f);

            transform.position = new Vector3(pos.x, pos.y, pos.z);
        }
    }

    public void InvokeClampPositionSmooth(bool aboveSinkSurface)
    {
        StopAllCoroutines();
        StartCoroutine(ClampPositionSmooth(aboveSinkSurface));
    }

    private IEnumerator ClampPositionSmooth(bool aboveSinkSurface)
    {
        Vector3 pos = transform.position;

        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        pos.x = Mathf.Clamp(pos.x, EnvironmentSpecs.boundXLeft - 2f, EnvironmentSpecs.boundXRight + 2f);

        float boundYBottom = aboveSinkSurface ? EnvironmentSpecs.boundYBottom : EnvironmentSpecs.boundYBottomSinked;
        pos.y = Mathf.Clamp(pos.y, boundYBottom - 2f, EnvironmentSpecs.boundYTop + 2f);

        Vector3 destination = new Vector3(pos.x, pos.y, pos.z);

        while ((destination - transform.position).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, destination, 0.25f);
            yield return null;
        }
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