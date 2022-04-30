using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UndirectedGraph
{
    public HashSet<Vector3> Nodes;
    public HashSet<HashSet<Vector3>> Edges;

    public Dictionary<Vector3, HashSet<Vector3>> AdjList;

    public LayerMask layerMask;

    public UndirectedGraph()
    {
        Nodes = new HashSet<Vector3>();
        Edges = new HashSet<HashSet<Vector3>>();
        AdjList = new Dictionary<Vector3, HashSet<Vector3>>();
    }

    public void AddNode(Vector3 node)
    {
        Nodes.Add(node);
        AdjList.Add(node, new HashSet<Vector3>());
    }

    public void AddEdge(Vector3 node1, Vector3 node2)
    {
        if (!AdjList.ContainsKey(node1))
            AddNode(node1);

        if (!AdjList.ContainsKey(node2))
            AddNode(node2);

        AdjList[node1].Add(node2);
        AdjList[node2].Add(node1);

        HashSet<Vector3> edge = new HashSet<Vector3>();
        edge.Add(node1);
        edge.Add(node2);

        Edges.Add(edge);

    }

    public List<Vector3> GetPath(Vector3 from, Vector3 to, bool simplify)
    {

        //Debug.Log(from);
        //Debug.Log(to);

        //Debug.DrawLine(from, to, Color.green, 100f);

        List<Vector3> path = ComputePath(NearestNode(from), NearestNode(to), 100);

        // Replace the nearest grid nodes with the actual nodes
        path[0] = from;
        path[path.Count - 1] = to;

        //// debug draw path
        //for (int i = 0; i < path.Count - 1; i++)
        //{
        //    Debug.DrawLine(path[i], path[i + 1], Color.red, 100f);
        //}

        if (simplify)
        {
            path = SimplifyPath(path);
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.blue, 100f);
            }

        }

        return path;

    }

    public Vector3 NearestNode(Vector3 point)
    {
        float minDist = Mathf.Infinity;
        Vector3 nearestNode = Vector3.zero;

        foreach (Vector3 node in Nodes)
        {
            float dist = Vector3.Distance(node, point);
            if (dist < minDist)
            {
                minDist = dist;
                nearestNode = node;
            }
        }

        return nearestNode;
    }


    private float DefaultGet(Dictionary<Vector3, float> dict, Vector3 key, float defaultValue)
    {
        if (dict.ContainsKey(key))
        {
            return dict[key];
        }
        else
        {
            return defaultValue;
        }

    }

    private Vector3 GetMinNodeInFringe(Dictionary<Vector3, float> f, List<Vector3> fringe)
    {
        float min = Mathf.Infinity;
        Vector3 minNode = Vector3.zero;

        foreach (Vector3 node in fringe)
        {
            if (f.ContainsKey(node))
            {
                if (f[node] < min)
                {
                    min = f[node];
                    minNode = node;
                }
            }
        }

        return minNode;
    }

    private List<Vector3> NotClosedNeighbours(Vector3 node, List<Vector3> closed)
    {
        List<Vector3> neighbours = new List<Vector3>();
        if (AdjList.ContainsKey(node))
        {
            foreach (Vector3 neighbour in AdjList[node])
            {
                if (!closed.Contains(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }

        }
        return neighbours;
    }


    private List<Vector3> RetrievePath(Dictionary<Vector3, Vector3> predecessor, Vector3 from, Vector3 to)
    {
        Vector3 target = to;
        List<Vector3> path = new List<Vector3>();
        path.Add(to);
        bool predecessorExists = predecessor.ContainsKey(target);

        while (predecessorExists)
        {
            path.Add(predecessor[target]);
            target = predecessor[target];
            predecessorExists = predecessor.ContainsKey(target);
        }

        path.Add(from);
        path.Reverse();

        return path;
    }

    // Based off the A* algorithm
    private List<Vector3> ComputePath(Vector3 from, Vector3 to, int maxIters)
    {

        List<Vector3> path = new List<Vector3>();

        List<Vector3> closed = new List<Vector3>();
        List<Vector3> fringe = new List<Vector3>();
        fringe.Add(from);

        Dictionary<Vector3, float> g = new Dictionary<Vector3, float>();
        Dictionary<Vector3, float> f = new Dictionary<Vector3, float>();
        Dictionary<Vector3, Vector3> predecessor = new Dictionary<Vector3, Vector3>();

        g[from] = 0;
        f[from] = DefaultGet(g, from, Mathf.Infinity) + Vector3.Distance(from, to);

        int numIter = 0;

        while (fringe.Count > 0 && numIter < maxIters)
        {
            Vector3 node = GetMinNodeInFringe(f, fringe);

            if (node == to)
                break;

            closed.Add(node);
            fringe.Remove(node);

            foreach (Vector3 neighbour in NotClosedNeighbours(node, closed))
            {
                float gPrime = DefaultGet(g, node, Mathf.Infinity); // TODO: Do we need to add an edge weight here? currently all edges are equal

                if (!fringe.Contains(neighbour) || (DefaultGet(g, neighbour, Mathf.Infinity) > gPrime))
                {
                    g[neighbour] = gPrime;
                    f[neighbour] = gPrime + Vector3.Distance(neighbour, to);
                    predecessor[neighbour] = node;
                    if (!fringe.Contains(neighbour))
                    {
                        fringe.Add(neighbour);
                    }

                }
            }

            numIter++;

        }

        path = RetrievePath(predecessor, from, to);
        return path;

    }

    private List<Vector3> SimplifyPath(List<Vector3> path)
    {
        List<Vector3> simplifiedPath = new List<Vector3>();

        simplifiedPath.Add(path[0]);

        for (int i = 1; i < path.Count; i++)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(simplifiedPath[simplifiedPath.Count - 1], path[i] - (simplifiedPath[simplifiedPath.Count - 1]), out hitInfo, (simplifiedPath[simplifiedPath.Count - 1] - path[i]).magnitude, layerMask))
            {
                simplifiedPath.Add(path[i-1]);
            }
        }

        simplifiedPath.Add(path[path.Count - 1]);

        return simplifiedPath;
    }

}
