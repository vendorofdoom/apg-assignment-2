using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamoCamera : MonoBehaviour
{
    public Vector3 targetDir;

    void Update()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, targetDir); // Force the camera to always point in target direction
    }
}
