using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public string currentID; // This will be the ID of the current node.
    public List<string> unlockedNodeIDs; // IDs of unlocked nodes. Initialized with the starting node.

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeMap();
    }

    public void InitializeMap()
    {
        // Start by locking all nodes
        foreach (MapNode node in FindObjectsOfType<MapNode>())
        {
            node.LockNode();
        }

        // If there's a currentNode set, unlock it and its children.
        if (currentID != null)
        {
            MapNode current = GetCurrentNode();
            if (current)
            {
                foreach (MapNode child in current.children)
                {
                    child.UnlockNode();
                }
            }
        }
        else
        {
            // If it's the start of the game and no current node is set, 
            // you might want to unlock a specific starting node.
            // For example, if the starting node's ID is "StartNode":
            MapNode startingNode = FindNodeByID("StartNode");
            if (startingNode)
            {
                startingNode.UnlockNode();
                foreach (MapNode child in startingNode.children)
                {
                    child.UnlockNode();
                }
            }
        }
    }


    public void NodeCompleted()
    {
        // Lock the current node and unlock its children.
        MapNode currentNode = FindNodeByID(currentID);
        if (currentNode != null)
        {
            currentNode.LockNode();
            foreach (var child in currentNode.children)
            {
                child.UnlockNode();
                if (!unlockedNodeIDs.Contains(child.uniqueID))
                {
                    unlockedNodeIDs.Add(child.uniqueID);
                }
            }
        }
    }
        public void SetCurrentID(string id)
    {
        currentID = id;
    }
    
    public MapNode GetCurrentNode()
    {
        return FindNodeByID(currentID);
    }

    public MapNode FindNodeByID(string id)
    {
        foreach (var node in FindObjectsOfType<MapNode>())
        {
            if (node.uniqueID == id)
            {
                return node;
            }
        }
        return null;
    }
}
