using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseSender : MonoBehaviour
{
    private const string projectId = "arcoretest-248"; // You can find this in your Firebase project settings
    private static readonly string databaseURL = $"https://{projectId}.firebaseio.com/";
    private static fsSerializer serializer = new fsSerializer();
    public delegate void PostUserCallback();
    public delegate void GetUserCallback(Node node);
    public delegate void GetUsersCallback(Dictionary<string, Node> nodes);
    public InputField input_id;
    public InputField input_type;
    public InputField input_location;
    public InputField input_nextNode;
    public InputField input_costOfNode;
    public Text cloudAnchorId;
    public Text status;
    public Text status2;
    public GameObject panel;

    public static void PostNode(Node node)
    {
        RestClient.Put<Node>($"{databaseURL}nodes/{node.id.ToString()}.json", node).Then(response => {
        });
    }

    public void createNode()
    {
        /*ArrayList liste = new ArrayList();
        string[] nextNode = input_nextNode.text.Split(',');
        for(int i=0;i<nextNode.Length;i++)
        {
            liste.Add(nextNode[i]);
        }*/
        /*foreach (string next in nextNode)
        {
            liste.Add(next);
        }*/
       Node node = new Node(input_id.text, input_type.text.ToString(), input_location.text.ToString(), cloudAnchorId.text.ToString(),input_nextNode.text.ToString(),input_costOfNode.text.ToString());
        /*string[] nextNode = input_nextNode.text.Split(',');
        foreach(string next in nextNode)
        {
            node.nextNodeAdd(next);
        }*/
        PostNode(node);
        clearTable();

    }

    private void clearTable()
    {
        input_id.Select();
        input_id.text = "";
        input_location.Select();
        input_location.text = "";
        input_type.Select();
        input_type.text = "";
        input_nextNode.Select();
        input_nextNode.text = "";
        input_costOfNode.Select();
        input_costOfNode.text = "";
        cloudAnchorId.text = "Node sended!";
        status.text = "Ready for sending new node...";
        status2.text = "Ready for sending new node...";
        panel.gameObject.SetActive(false);
        
    }
}
