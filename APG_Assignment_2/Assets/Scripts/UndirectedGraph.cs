using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndirectedGraph
{
    public HashSet<Vector3> Nodes;
    public HashSet<HashSet<Vector3>> Edges;

    public Dictionary<Vector3, HashSet<Vector3>> AdjList;

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

    public void PrintInfo()
    {
        foreach(Vector3 node in Nodes)
        {
            Debug.Log(node);
        }
    }
}
