using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Transform topDownView;
    public Transform sideView;
    public Camera cam;

    public bool isTopDown;

    // Start is called before the first frame update
    void Start()
    {
        isTopDown = false;
        SetSideView();
    }

    public void ToggleCameraView()
    {
        isTopDown = !isTopDown;
        if (isTopDown)
        {
            SetSideView();
        }
        else
        {
            SetTopDownView();
        }
    }

    private void SetCameraTransform(Transform target)
    {
        cam.transform.position = target.position;
        cam.transform.rotation = target.rotation;
        cam.transform.localScale = target.localScale;
    }

    private void SetTopDownView()
    {
        SetCameraTransform(topDownView);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 25;
    }

    private void SetSideView()
    {
        SetCameraTransform(sideView);
        Camera.main.orthographic = false;
        Camera.main.farClipPlane = 100;
        Camera.main.nearClipPlane = 5;
        Camera.main.fieldOfView = 35;
    }
}
