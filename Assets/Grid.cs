using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    //public bool displayPath;
    public bool displayGrid;
    public Vector2 gridWorldSize;
    Node[,] grid;
    public float nodeRadius;
    public LayerMask unWalkableMask;
    public Transform player;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    // Heap Size
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    //Create the grid
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; //world edge
        
        // Go through each position of the grid 
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unWalkableMask)); //check if its walkable
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    //Get a list of neighbors of the current node
    public List<Node> GetNeightbors(Node node)
    {
        //List of neighbor nodes
        List<Node> neighbors = new List<Node>();

        //Search neighbors
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //Skipe the node where the player is 
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                //Check if its inside the grid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        //return the neighbors of the player position Node
        return neighbors;
    }

    //Find Player position node in the world grid
    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); 
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    //public List<Node> path;

    //Debug Grid
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        /*if (displayPath)
        {
            if (path != null)
            {
                foreach (Node node in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
            else
            {*/

         if (grid != null && displayGrid)
         {
                    //Node playerNode = NodeFromWorldPosition(player.position);

             foreach (Node node in grid)
             {
                Gizmos.color = (node.walkable) ? Color.white : Color.red; //If the node is walkable will draw it white, if its red mean its not walkable
                
                /*if (playerNode == node)
                  {
                     Gizmos.color = Color.blue;
                  }

                  if (grid != null)
                  {
                    if (path.Contains(node))
                      {
                          Gizmos.color = Color.black;
                      }
                  }*/
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f)); //draw a cube per each node in the grid
             }
                
         }
          
    }
}
