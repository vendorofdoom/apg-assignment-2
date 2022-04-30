using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleBrain : MonoBehaviour
{
    public Tank tank;
    public Movement movement;
    public CuttleColour cuttleColour;

    public Transform target;

    public bool simplifyPath;

    private void Start()
    {
        movement.path = tank.Graph.GetPath(transform.position, target.position, simplifyPath);
    }

}
