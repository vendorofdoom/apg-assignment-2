using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public CameraControls cameraControls;
    public GameObject[] foods;

    [Header("Range for dropping food into tank")]
    public Vector2 foodDropX;
    public float foodDropY;
    public Vector2 foodDropZ;

    [Header("Mouse click raycast")]
    public Collider ground;
    public float raycastDist;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraControls.ToggleCameraView();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            DropFood();
        }

    }

    private void DropFood()
    {

        RaycastHit hitInfo;
        Vector3 worldPos;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (ground.Raycast(ray, out hitInfo, raycastDist))
        {
            worldPos = hitInfo.point;
            Debug.Log("Hit! " + worldPos);
            Instantiate(foods[Random.Range(0, foods.Length)], new Vector3(worldPos.x, foodDropY, worldPos.z), Quaternion.identity, transform);
        }


    }
}
