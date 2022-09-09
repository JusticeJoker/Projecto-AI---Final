using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathFinding : MonoBehaviour
{
    //public Transform seeker, target;

    PathFindManager requestManager;

    Grid grid;

    void Awake()
    {
        requestManager = GetComponent<PathFindManager>();
        grid = GetComponent<Grid>();
    }

    /*void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            FindPath(seeker.position, target.position);
        }
    }*/

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        //Stop watch to count the processing time
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPosition(startPosition);
        Node targetNode = grid.NodeFromWorldPosition(targetPosition);

        // If either of them aren't walkable then there's no
        // path to be found
        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize); //set of nodes to be checked
            HashSet<Node> closedSet = new HashSet<Node>(); //set of names already checked
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst(); //Setting first Node
                closedSet.Add(currentNode);
                //Run through all the Nodes
                /*for (int i = 1; i < openSet.Count; i++)
                {
                    //Check the cost of the fCost and hCost of the nodes
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }

                }

                openSet.Remove(currentNode);*/

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;

                    //RetracePath(startNode, targetNode);
                    break;
                }


                //check if the neighbor is walkable or in the close list
                foreach (Node neighbor in grid.GetNeightbors(currentNode))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    //check if new path to neighbor is shorter then old path or
                    //if the neighbor is not in the Open List
                    int newMovementCost = currentNode.gCost + GetDistanceNode(currentNode, neighbor);
                    if (newMovementCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCost; //calculate new gCost
                        neighbor.hCost = GetDistanceNode(neighbor, targetNode); //calculate new hCost
                        neighbor.parent = currentNode;

                        //Add new neighbor
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }        
        yield return null; //Wait one frame before returning
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        requestManager.FinishedBuildPath(waypoints, pathSuccess);
    }

    //Find new path from point A to point B
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        //remake new path
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplePath(path);

        //reverse waypoints
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplePath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 direction = Vector2.zero; //Store the direction of the last 2 Nodes

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            
            //If the path hasnt changed
            if (directionNew != direction)
            {
                waypoints.Add(path[i].worldPosition);
            }
            direction = directionNew;
        }

        // return the waypoints to array after the look is done
        return waypoints.ToArray();
    }

    //Get distance between 2 nodes
    int GetDistanceNode(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeB.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
