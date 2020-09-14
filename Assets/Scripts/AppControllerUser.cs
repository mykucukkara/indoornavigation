using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.XR.ARCoreExtensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using FullSerializer;
using Proyecto26;
using System;
using System.Linq;

public class AppControllerUser : MonoBehaviour
{
    public GameObject HostedPointPrefab;
    public GameObject ResolvedPointPrefab;
    public ARAnchorManager AnchorManager;
    public ARRaycastManager RaycastManager;
    public ARCameraManager ARCamera;
    public static int totalNodeCount = 9;
    public Dropdown dropdown_from;
    public Dropdown dropdown_to;
    public static List<Node> nodes = new List<Node>();
    private List<String> locations = new List<string>();
    private string text_from;
    private string text_to;
    public Text OutputText;
    public GameObject panel;
    public static int j=0;
    private static List<Node> paths = new List<Node>();

    // Start is called before the first frame update*/

    private static void Start()
    {
        j = 0;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public void download()
    {
        string temp2;
        for (int i = 1; i <= totalNodeCount; i++)
        {
            temp2 = i.ToString();
            DatabaseReceiver.GetUser(temp2, user =>
            {
                nodes.Add(user);
            });
        }
        Debug.Log("Map download success!");

    }
    public void controlStatus()
    {
        Debug.Log("Nodes size: "+nodes.Count);
    }

    public Node GetNode(string id)
    {
        Node node = new Node();
        foreach (Node n in nodes)
        {
            if (n.id == id)
                node = n;
        }
        return node;
    }
    public void PopulateList()
    {
        locations.Add("Please Choose Destination");
        foreach (Node val in nodes)
        {
            val.dataSplit();
            locations.Add(val.location.ToString());
        }
        dropdown_from.AddOptions(locations);
        dropdown_to.AddOptions(locations);
    }
    public void Dropdown_IndexChancedFrom(int index)
    {
        if (index == 0)
        {
            text_from = null;
        }
        else
        {
            text_from = locations[index].ToString();

        }
    }
    public void Dropdown_IndexChancedFrom2(int index)
    {
        if (index == 0)
        {
            text_to = null;
        }
        else
        {
            text_to = locations[index].ToString();
        }
    }
    public void Run()
    {
        Node start = new Node();
        Node finish = new Node();
        foreach (Node val in nodes)
        {
            val.findNeighbors(nodes);
            val.dataSplit();
            if (val.location == text_from)
                start = val;
            if (val.location == text_to)
                finish = val;
        }
        panel.SetActive(false);
        Path path = new Path();
        path = GetShortestPath(start.id, finish.id);
        OutputText.text = "Path Find!";
        foreach (Node val in path.nodes)
        {
            paths.Add(val);
        }
        printScreen(paths[j]);
    }

    private enum AppMode
    {
        //Wait for user to tap screen to begin hosting a point.
        TouchToHostCloudReferencePoint,

        //Poll hosted point state until it is ready to use.
        WaitingForHostedReferencePoint,

        //Wait for user to tap screen to begin resolving the point.
        TouchToResolveCloudReferencePoint,

        //Poll resolving point state until it is ready to use.
        WaitingForResolvedReferencePoint,

        FinishApp,
    }

    private AppMode m_AppMode = AppMode.TouchToHostCloudReferencePoint;
    private ARCloudAnchor m_CloudReferencePoint;
    private string m_CloudReferenceId;

    // Update is called once per frame
    void Update()
    {
        if (m_AppMode == AppMode.WaitingForResolvedReferencePoint)
        {
            OutputText.text = m_AppMode.ToString();

            CloudAnchorState cloudReferenceState =
                m_CloudReferencePoint.cloudAnchorState;
            OutputText.text += " - " + cloudReferenceState.ToString();

            if (cloudReferenceState == CloudAnchorState.Success)
            {
                GameObject cloudAnchor = Instantiate(
                                             ResolvedPointPrefab,
                                             Vector3.zero,
                                             Quaternion.identity);
                cloudAnchor.transform.SetParent(
                    m_CloudReferencePoint.transform, false);
                m_CloudReferencePoint = null;
                j++;
                if(j == paths.Count-1)
                {
                    m_AppMode = AppMode.FinishApp;
                }
                printScreen(paths[j]);
            }
        }
        if(m_AppMode == AppMode.FinishApp)
        {
            OutputText.text = "Hedefe Ulaştınız!";
        }

    }
    private void printScreen(Node key)
    {
        m_CloudReferenceId = string.Empty;

        m_CloudReferencePoint =
            AnchorManager.ResolveCloudAnchorId(key.cloudAnchorId);
        if (m_CloudReferencePoint == null)
        {
            OutputText.text = "Resolve Failed!";
            printScreen(key);
            return;
        }

        // Wait for the reference point to be ready.
        m_AppMode = AppMode.WaitingForResolvedReferencePoint;
    }

    public virtual Path GetShortestPath(string start, string end)
    {

        // We don't accept null arguments
        if (start == null || end == null)
        {
            throw new ArgumentNullException();
        }

        // The final path
        Path path = new Path();

        // If the start and end are same node, we can return the start node
        if (start == end)
        {
            path.nodes.Add(GetNode(start));
            return path;
        }

        // The list of unvisited nodes
        List<Node> unvisited = new List<Node>();

        // Previous nodes in optimal path from source
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

        // The calculated distances, set all to Infinity at start, except the start Node
        Dictionary<Node, float> distances = new Dictionary<Node, float>();

        for (int i = 0; i < nodes.Count; i++)
        {
            Node node = nodes[i];
            unvisited.Add(node);

            // Setting the node distance to Infinity
            distances.Add(node, float.MaxValue);
        }

        // Set the starting Node distance to zero
        distances[GetNode(start)] = 0f;
        while (unvisited.Count != 0)
        {

            // Ordering the unvisited list by distance, smallest distance at start and largest at end
            unvisited = unvisited.OrderBy(node => distances[node]).ToList();

            // Getting the Node with smallest distance
            Node current = unvisited[0];

            // Remove the current node from unvisisted list
            unvisited.Remove(current);

            // When the current node is equal to the end node, then we can break and return the path
            if (current.id == end)
            {

                // Construct the shortest path
                while (previous.ContainsKey(current))
                {

                    // Insert the node onto the final result
                    path.nodes.Insert(0, current);

                    // Traverse from start to end
                    current = previous[current];
                }

                // Insert the source onto the final result
                path.nodes.Insert(0, current);
                break;
            }

            // Looping through the Node connections (neighbors) and where the connection (neighbor) is available at unvisited list
            for (int i = 0; i < current.neighborsString.Count; i++)
            {
                Node neighbor = GetNode(current.neighborsString[i]);

                // Getting the distance between the current node and the connection (neighbor)
                float length = Int32.Parse(current.costOfNode) + Int32.Parse(neighbor.costOfNode);
                

                // The distance from start node to this connection (neighbor) of current node
                float alt = distances[current] + length;

                // A shorter path to the connection (neighbor) has been found
                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
        }
        return path;
    }
}
    
