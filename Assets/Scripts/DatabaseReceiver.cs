using FullSerializer;
using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DatabaseReceiver
{
    private const string projectId = "arcoretest-248"; // You can find this in your Firebase project settings
    private static readonly string databaseURL = $"https://{projectId}.firebaseio.com/";
    private static fsSerializer serializer = new fsSerializer();

    public delegate void PostUserCallback();
    public delegate void GetUserCallback(Node node);
    public delegate void GetUsersCallback(Dictionary<string, Node> nodes);


    /// <summary>
    /// Adds a user to the Firebase Database
    /// </summary>
    /// <param name="user"> User object that will be uploaded </param>
    /// <param name="userId"> Id of the user that will be uploaded </param>
    /// <param name="callback"> What to do after the user is uploaded successfully </param>
    public static void PostUser(Node node, string nodeId, PostUserCallback callback)
    {
        RestClient.Put<Node>($"{databaseURL}nodes/{nodeId}.json", node).Then(response => { callback(); });
    }

    /// <summary>
    /// Retrieves a user from the Firebase Database, given their id
    /// </summary>
    /// <param name="userId"> Id of the user that we are looking for </param>
    /// <param name="callback"> What to do after the user is downloaded successfully </param>
    public static void GetUser(string nodeId, GetUserCallback callback)
    {
        RestClient.Get<Node>($"{databaseURL}nodes/{nodeId}.json").Then(node => {
            callback(node);
        });
    }

    /// <summary>
    /// Gets all users from the Firebase Database
    /// </summary>
    /// <param name="callback"> What to do after all users are downloaded successfully </param>
    public static void GetUsers(GetUsersCallback callback)
    {
        RestClient.Get($"{databaseURL}nodes.json").Then(response =>
        {
            var responseJson = response.Text;

            // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
            // to serialize more complex types (a Dictionary, in this case)
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, Node>), ref deserialized);

            var nodes = deserialized as Dictionary<string, Node>;
            callback(nodes);
        });
    }
}
