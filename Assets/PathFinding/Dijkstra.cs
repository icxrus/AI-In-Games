using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Dijkstra : MonoBehaviour
{
    class Connection
    {
        public int from, to;
        public float cost;

        public Connection(int from, int to)
        {
            this.from = from;
            this.to = to;
        }
    }

    struct NodeRecord
    {
        public int node, connection;
        public float costSoFar;
    }

    public GameObject[] nodes;
    
    List<Connection> connections = new List<Connection>();

    private const float connectionDrawOffset = 0.25f;
    
    [SerializeField]int startNode = 7, goalNode = 1;
    private int iteration = 0;
    List<NodeRecord> openList = new List<NodeRecord>();
    List<NodeRecord> closedList = new List<NodeRecord>();
    
    NodeRecord currentNode;

    private string pathFindingStatus = "Initializing...";
    private List<int> finalPath;
    bool foundPath = false;

    private void Start()
    {
        connections.Add(new Connection(0, 1));
        connections.Add(new Connection(0, 5));
        connections.Add(new Connection(1, 2));
        connections.Add(new Connection(1, 3));
        connections.Add(new Connection(1, 7));
        connections.Add(new Connection(2, 0));
        connections.Add(new Connection(3, 4));
        connections.Add(new Connection(3, 8));
        connections.Add(new Connection(4, 6));
        connections.Add(new Connection(5, 0));
        connections.Add(new Connection(5, 9));
        connections.Add(new Connection(7, 2));
        connections.Add(new Connection(8, 6));
        connections.Add(new Connection(9, 4));


        InitializeSearch();

        foreach (var conn in connections)
        {
            conn.cost=Vector3.Distance(nodes[conn.from].transform.position, nodes[conn.to].transform.position);
            Debug.Log(conn.cost);
        }
    }

    private void InitializeSearch()
    {
        iteration = 0;
        
        openList.Clear();
        
        NodeRecord startRecord;
        startRecord.node = startNode;
        startRecord.connection = -1;
        startRecord.costSoFar = 0;
        openList.Add(startRecord);

        closedList.Clear();
        
        currentNode.node = -1;
        
        pathFindingStatus = "Initializing...";
        
        foundPath = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!foundPath && openList.Count > 0)
            {
                iteration++;
                pathFindingStatus = "In Progress...";
                currentNode = FindSmallestOpenNode();
                if (currentNode.node == goalNode)
                {

                }
                else
                {
                    List<int> connectionIDs = GetConnections(currentNode.node);
                    foreach (var id in connectionIDs)
                    {
                        int endNode = connections[id].to;

                        NodeRecord endNodeRecord;
                        float endNodeCost = currentNode.costSoFar + connections[id].cost;
                        int indexInClosedList = Contains(closedList, endNode);
                        int indexInOpenList = Contains(openList, endNode);

                        if (indexInClosedList != -1)
                        {
                            //This node has already been processed, continue
                            continue;
                        }
                        else if (indexInOpenList != -1)
                        {
                            endNodeRecord = openList[indexInOpenList];
                            if (endNodeRecord.costSoFar <= endNodeCost)
                            {
                                //The new path has worse cost than the previous
                                continue;
                            }
                        }
                        else
                        {
                            //This is an unvisited node
                            endNodeRecord.node = endNode;
                        }

                        endNodeRecord.costSoFar = endNodeCost;
                        endNodeRecord.connection = id;

                        if (indexInOpenList == -1)
                        {
                            openList.Add(endNodeRecord);
                        }
                        else
                        {
                            //update node in open list
                            openList[indexInOpenList] = endNodeRecord;
                        }
                    }
                    openList.Remove(currentNode);
                    closedList.Add(currentNode);
                }

                if (currentNode.node == goalNode)
                {
                    finalPath = new List<int>();
                    string path = string.Format("{0}", goalNode);
                    while (currentNode.node != startNode)
                    {
                        finalPath.Add(currentNode.connection);
                        path = string.Format("{0} ->", connections[currentNode.connection].to);
                        int sourceNode = connections[currentNode.connection].from;
                        int indexInClosedList = Contains(closedList, sourceNode);
                        currentNode = closedList[indexInClosedList];
                    }

                    finalPath.Reverse();
                    var printablePath = closedList.Select(x=>x.node);
                    pathFindingStatus = "Terminated, path: " + string.Join(" -> ",printablePath);
                    foundPath = true;
                }
                else if (openList.Count == 0)
                {
                    pathFindingStatus = "Terminated, no path";
                }
            }
            else
            {
                    InitializeSearch();
            }
                
        }
    }
    

    private List<int> GetConnections(int from)
    {
        List<int> conns = new List<int>();
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].from == from)
            {
                conns.Add(i);
            }
        }
        return conns;
    }

    private NodeRecord FindSmallestOpenNode()
    {
        int minNode = -1;
        float minCost = float.MaxValue;
        for (int i = 0; i < openList.Count; i++)
        {
            if (openList[i].costSoFar < minCost)
            {
                minCost = openList[i].costSoFar;
                minNode = i;
            }
        }
        return openList[minNode];
    }

    private int Contains(List<NodeRecord> list,int node)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].node == node)
            {
                return i;
            }
        }
        return -1;
    }

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        float labelY = 0, yDist = 0.4f;
        
        Handles.Label(new Vector3(0,labelY,0), pathFindingStatus, style);

        labelY -= yDist;
        Handles.Label(new Vector3(0,labelY,0), string.Format("Iteration {0}", iteration), style);
        
        labelY -= yDist;
        Handles.Label(new Vector3(0,labelY,0), string.Format("Open list:"), style);
        for (int i = 0; i < openList.Count; i++)
        {
            string openNodeStr = string.Format("node {0}, (from: {1}, cost so far: {2:0.0})", 
                openList[i].node,
                openList[i].connection == -1 ? -1 : connections[openList[i].connection].from, 
                openList[i].costSoFar);
            labelY -=yDist;
            Handles.Label(new Vector3(0,labelY,0), openNodeStr, style);
        }
        
        labelY -= yDist;
        Handles.Label(new Vector3(0,labelY,0), string.Format("Closed list:"), style);
        for (int i = 0; i < closedList.Count; i++)
        {
            string closedNodeStr = string.Format("node: {0} (from {1}, cost so far: {2:0.0})",
                closedList[i].node,
                closedList[i].connection == -1 ? -1 : connections[closedList[i].connection].from,
                closedList[i].costSoFar);
            labelY -= yDist;
            Handles.Label(new Vector3(0,labelY,0), closedNodeStr, style);
        }
    }
}
