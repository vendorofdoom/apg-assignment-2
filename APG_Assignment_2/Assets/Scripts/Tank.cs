using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tank : MonoBehaviour
{
    public Vector3 Origin;
    public int NumBoxesX = 5;
    public int NumBoxesY = 10;
    public int NumBoxesZ = 1;
    public float BoxSize = 1f;

    public UndirectedGraph Graph;
    public LayerMask obstacleLayerMask;

    public Collider[] Obstacles;
    public Transform from;
    public Transform to;

    void Start()
    {
        Initialise();
    }

    public void Initialise()
    {
        Graph = new UndirectedGraph();
        Graph.layerMask = obstacleLayerMask;
        float boxMidOffset = BoxSize / 2;

        for (int x = 0; x < NumBoxesX; x++)
        {
            for (int y = 0; y < NumBoxesY; y++)
            {
                for (int z = 0; z < NumBoxesZ; z++)
                {

                    Vector3 mid = new Vector3(Origin.x + (x * BoxSize) + boxMidOffset, 
                                                Origin.y + (y * BoxSize) + boxMidOffset, 
                                                Origin.z + (z * BoxSize) + boxMidOffset);

                    if (!PointWithinObstacle(mid))
                    {
                        Vector3 right = mid + new Vector3(BoxSize, 0, 0);
                        Vector3 up = mid + new Vector3(0, BoxSize, 0);
                        Vector3 fwd = mid + new Vector3(0, 0, BoxSize);


                        if ((x < NumBoxesX - 1) && !PointWithinObstacle(right))
                            Graph.AddEdge(mid, right);

                        if ((y < NumBoxesY - 1) && !PointWithinObstacle(up))
                            Graph.AddEdge(mid, up);

                        if ((z < NumBoxesZ - 1) && !PointWithinObstacle(fwd))
                            Graph.AddEdge(mid, fwd);

                    }
                }
            }
        }

    }

    void OnDrawGizmos()
    {
        
        float boxMidOffset = BoxSize / 2;

        for (int x = 0; x < NumBoxesX; x++)
        {
            for (int y = 0; y < NumBoxesY; y++)
            {
                for (int z = 0; z < NumBoxesZ; z++)
                {


                    Vector3 mid = new Vector3(Origin.x + (x * BoxSize) + boxMidOffset,
                                                Origin.y + (y * BoxSize) + boxMidOffset,
                                                Origin.z + (z * BoxSize) + boxMidOffset);

                    if (PointWithinObstacle(mid))
                    {
                        // don't draw any edges

                        // draw node
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(mid, 0.05f);
                        //Gizmos.DrawWireCube(mid, new Vector3(BoxSize, BoxSize, BoxSize));

                    }
                    else
                    {
                        // draw edges

                        Vector3 right = mid + new Vector3(BoxSize, 0, 0);
                        Vector3 up =    mid + new Vector3(0, BoxSize, 0);
                        Vector3 fwd =   mid + new Vector3(0, 0, BoxSize);

                        Gizmos.color = Color.white;

                        if ((x < NumBoxesX - 1) && !PointWithinObstacle(right))
                            Gizmos.DrawLine(mid, right);

                        if ((y < NumBoxesY - 1) && !PointWithinObstacle(up))
                            Gizmos.DrawLine(mid, up);

                        if ((z < NumBoxesZ - 1) && !PointWithinObstacle(fwd))
                            Gizmos.DrawLine(mid, fwd);

                        // draw node
                        //Gizmos.color = Color.green;
                        //Gizmos.DrawSphere(mid, 0.05f);
                        //Gizmos.DrawWireCube(mid, new Vector3(BoxSize, BoxSize, BoxSize));
                    }
                        
                    

                }
            }
        }
    }



    private bool PointWithinObstacle(Vector3 point)
    {
        foreach (Collider c in Obstacles)
        {
            if (c.ClosestPoint(point) == point)
            {
                return true;
            }
        }

        return false;
    }


}