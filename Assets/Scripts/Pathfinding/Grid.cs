using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public bool onlyDisplayPathGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    public int gridSizeX, gridSizeY;

    private readonly float[] weights = new float[] { 8f, 4f, 2f, 1f };

    public float[,] map;
    public int[,] biomeMap;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        //gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        //gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        //map = TerrainFunctions.BaseTerrain(gridSizeX, gridSizeY, new Vector2(0f, 0f), weights);
        //biomeMap = TerrainFunctions.GenerateBiome(gridSizeX, gridSizeY, new Vector2(0f, 0f), map, 6, 0.25f, 0.45f);

        //CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                bool walkable = biomeMap[x, y] > 2;
                bool pathExists = false;
                int movementPenalty = 0;
                if (biomeMap[x, y] == 4)
                {
                    movementPenalty = 1000;
                }
                grid[x, y] = new Node(walkable, x, y, movementPenalty, 0);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector2Int worldPosition)
    {
        //float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        //float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        //percentX = Mathf.Clamp01(percentX);
        //percentY = Mathf.Clamp01(percentY);

        //int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        //int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[worldPosition.x, worldPosition.y];
    }

    public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        Vector3 worldPosition;

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    worldPosition = new Vector3(n.gridX, n.gridY, 0f);
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else
        {

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    worldPosition = new Vector3(n.gridX, n.gridY, 0f);
                    //Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (n.walkable & n.movementPenalty == 0) 
                    {
                        Gizmos.color = Color.white;
                    } else if (n.walkable & n.movementPenalty != 0)
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    if (path != null)
                        if (path.Contains(n))
                            Gizmos.color = Color.black;
                    Gizmos.DrawCube(worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}
