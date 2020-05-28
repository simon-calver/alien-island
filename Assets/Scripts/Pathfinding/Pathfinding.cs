using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {
	
    public Vector2Int startPos, endPos;

    Grid grid;
    public int gridSizeX, gridSizeY;

    private readonly float[] weights = new float[] { 8f, 4f, 2f, 1f };
    public float[,] map;
    public int[,] biomeMap;
    public int[] tileTypes = new int[1];


    void Awake() {
        TerrainFunctions terrainFunctions = new TerrainFunctions();
        grid = GetComponent<Grid>();
        //map = TerrainFunctions.BaseTerrain(gridSizeX, gridSizeY, new Vector2(0f, 0f), weights);
        //biomeMap = TerrainFunctions.GenerateBiome(gridSizeX, gridSizeY, new Vector2(0f, 0f), map, 6, 0.25f, 0.45f);

        //nodeDiameter = nodeRadius * 2;
        //gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        //gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        grid.gridSizeX = gridSizeX;
        grid.gridSizeY = gridSizeY;
        map = terrainFunctions.BaseTerrain(gridSizeX, gridSizeY, new Vector2(0f, 0f), weights);
        biomeMap = terrainFunctions.GenerateBiome(gridSizeX, gridSizeY, new Vector2(0f, 0f), map, 6, 0.25f, 0.45f, tileTypes);

        grid.biomeMap = biomeMap;


    }

    void Start()
    {
        //grid.CreateGrid();
    }
    void Update() {


        grid.CreateGrid();
        FindPath(startPos, endPos);
	}

	void FindPath(Vector2Int startPos, Vector2Int targetPos) {

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

        //Debug.Log(openSet.Count);

		while (openSet.Count > 0) {
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			if (currentNode == targetNode) {
				RetracePath(startNode,targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
					else {
                        openSet.UpdateItem(neighbour);
                    }
				}
			}
		}

        //Debug.Log(openSet.Count);
        //return 1;
    }

	void RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		grid.path = path;
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}


}
