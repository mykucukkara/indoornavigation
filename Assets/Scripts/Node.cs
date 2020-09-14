using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The user class, which gets uploaded to the Firebase Database
/// </summary>

[Serializable] // This makes the class able to be serialized into a JSON
public class Node
{
    public string id;
    public string type;
    public string location;
    public string cloudAnchorId;
    public string next;
    public string costOfNode;
    public List<Node> neighborsNode = new List<Node>();
    public List<string> neighborsString = new List<string>();
    protected List<Node> m_Connections = new List<Node>();

    public Node() { }

    public Node(string id, string type, string location, string cloudAnchorId,string next,string costOfNode)
    {
        this.id = id;
        this.type = type;
        this.location = location;
        this.cloudAnchorId = cloudAnchorId;
        this.next = next;
        this.costOfNode = costOfNode;
    }


    public void dataSplit()
    {
        string[] nextlist = next.Split(',');
        foreach(string n in nextlist)
        {
            neighborsString.Add(n);
        }
    }
    public void findNeighbors(List<Node> nodes)
    {
        for(int i=0;i<neighborsString.Count;i++)
        {
            foreach(Node n in nodes)
            {
                if (neighborsString[i] == n.id)
                    neighborsNode.Add(n);
            }
        }
    }

}
