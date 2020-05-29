using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerrainFunctions// : MonoBehaviour
{
    public bool[,] structureMap;

    //private int width;
    //private int height;

    // This makes the terrain shape, which determines where rocks and water go and where other things can't go
    public float[,] BaseTerrain(int width, int height, Vector2 offset, float[] weights)
    {
        // Array to store terrain data in
        float[,] terrainMap = new float[width, height];

        // Variables to store height in         
        float GaussVal;
        float PerlinVal;
        float PerlinVal_0;
        float PerlinVal_1;
        float PerlinVal_2;
        float PerlinVal_3;

        float radius;
        // The terrain is a float array with values from 0 to 1, the shape is found by multipying a Gaussian distribution by Perlin noise
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Rescale the coordinates so that -1 < x,y < 1, the maximum value is in the middle of the map (x=width, y=height), has values in the range [0,1]
                GaussVal = Mathf.Exp(-80 * (Mathf.Pow(2 * (float)x / width - 1, 4) + Mathf.Pow(2 * (float)y / height - 1, 4)));

                // Find the normalised distance to the centre
                radius = Mathf.Pow(2 * (float)x / width - 1, 2) + Mathf.Pow(2 * (float)y / height - 1, 2);

                // Find the Perlin noise at this point, has values in the range [0,1]
                PerlinVal_0 = Mathf.PerlinNoise(2.0f*x / width + offset.x, 2.0f*y / height + offset.y);
                PerlinVal_1 = Mathf.PerlinNoise(4.0f * x / width + offset.x, 4.0f * y / height + offset.y);
                PerlinVal_2 = Mathf.PerlinNoise(8.0f * x / width + offset.x, 8.0f * y / height + offset.y);
                PerlinVal_3 = Mathf.PerlinNoise(16.0f * x / width + offset.x, 16.0f * y / height + offset.y);

                // The differnt noises are summed with different weights
                float weightSum = weights[0] + weights[1] + weights[2] + weights[3];
                PerlinVal = (weights[0]*PerlinVal_0 + weights[1] * PerlinVal_1 + weights[2] * PerlinVal_2 + weights[3] * PerlinVal_3) / weightSum;

                // This tries to make sure that the terrain map height is low near the edges
                terrainMap[x, y] = (3.0f*PerlinVal - 1.4f*radius) / 4.0f;    
            }
        }
        return terrainMap;
    }

    // Make map for biome types
    public int[,] GenerateBiome(int width, int height, Vector2 offset, float[,] terrainMap, int poo_of_biomes, float water_bound, float rock_bound, int[] terrainTypes)
    {
        // Array to store terrain data in
        int[,] biomeMap = new int[width, height];

        // Count the number of times there is a 3 in terrainTypes and when this first occurs. This assumes that the "biome" tiles (not rocks road or water)
        // are listed sequentially
        int no_of_biomes = 0;
        int first_biome_ind = 0;
        for (int i = 0; i < terrainTypes.Length; i++)
        {
            if (terrainTypes[i] == 3)
            {
                no_of_biomes++;
                if (first_biome_ind == 0)
                {
                    first_biome_ind = i;
                }
                
            }
        }

        //Debug.Log((terrainTypes.Length- no_of_biomes));

        //Debug.Log(first_biome_ind);
        // The values 0 and 1 in the biome map are reserved for water and rock tiles
        int PerlinVal;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // First use the water and rock biome types if terrainMap is not bounded by the water and rock bounds
                if (terrainMap[x, y] < water_bound)
                {
                    PerlinVal = 0;
                }
                else if (terrainMap[x, y] > rock_bound)
                {
                    PerlinVal = 1;
                }
                else
                {
                    // The Perlin noise function may give values slightly outside the range [0, 1] so the  values are clamped
                    float perlin_noise = Mathf.Clamp(Mathf.PerlinNoise(12.0f * x / width + offset.x, 12.0f * y / height + offset.y), 0f, 0.99f);

                    // Convert this to ints in the range [first_biome_ind, first_biome_ind+no_of_biomes]
                    PerlinVal = Mathf.FloorToInt((float)(no_of_biomes) * perlin_noise) + first_biome_ind;

                    if (PerlinVal > 5) { Debug.Log(PerlinVal); }
                    if (PerlinVal < 2) { Debug.Log(PerlinVal); }
                    // DO THIS DIFFERENTLY
                    //also since some tiles don't correspond to 
                    // biomes fewer values are required
                    //PerlinVal = (int)Mathf.Floor(((float)(terrainTypes.Length - no_of_biomes) - 0.1f) * Mathf.Clamp(Mathf.PerlinNoise(12.0f * x / width + offset.x, 12.0f * y / height + offset.y), 0f, 0.99f)) + first_biome_ind;

                    // Near to the water the first biome is used as the beach
                    if (terrainMap[x, y] - water_bound <= 0.03f && terrainMap[x, y] >= water_bound)
                    {
                        PerlinVal = first_biome_ind;
                    }
                }
                
                // Set the value in the array 
                biomeMap[x, y] = PerlinVal;
               
            }
        }
        return biomeMap;
    }

    public Vector2[] PlaceStructures(int[,] biomeMap, GameObject[] structuresInScene)
    {
        // Array to store map tiles with structures no
        structureMap = new bool[biomeMap.GetUpperBound(0), biomeMap.GetUpperBound(1)];

        // Array for origins of each structure
        Vector2[] structureOrigins = new Vector2[structuresInScene.Length];
        Vector2 testPoint = new Vector2(0,0);

        // Choose structureCount many random positions to build structures on, check they are positioned appropraietly and are far apart 
        for (int structureInd = 0; structureInd < structuresInScene.Length; structureInd++)
        {
            // Get random coordinates within the map not on the edge
            bool validPoint = false;
            int count = 0;

            while (!validPoint & count < 40)
            {
                count++;

                testPoint = new Vector2(Random.Range(10, biomeMap.GetUpperBound(0) - 10), Random.Range(10, biomeMap.GetUpperBound(1) - 10));
                // If this is water or rock try again
                if (biomeMap[Mathf.RoundToInt(testPoint.x), Mathf.RoundToInt(testPoint.y)] >= 2)
                {
                    structureOrigins[structureInd] = testPoint; // new Vector2(Mathf.RoundToInt(Random.Range(0, width)), Mathf.RoundToInt((int)Random.Range(0, height)));
                    validPoint = true;
                }
            }

            structureOrigins[structureInd] = testPoint;
            structuresInScene[structureInd].transform.position = new Vector3(testPoint.x, testPoint.y, -10f);

            // Fill in the footprint of the struture in the structureMap with the predefined shapes, currently just written here 
            bool[,] shape_1 = new bool[,] { { true, true, true, true, true, true, true, true, true, true, true},
                                            { true, true, true, true, true, true, true, true, true, true, true }, 
                                            { true, true, true, true, true, true, true, true, true, true, true },
                                            { true, true, true, true, true, true, true, true, true, true, true },
                                            { true, true, true, true, true, true, true, true, true, true, true },
                                            { true, true, true, true, true, true, true, true, true, true, true },
                                            { true, true, true, true, true, true, true, true, true, true, true },
                                            { true, true, true, true, true, true, true, true, true, true, true },
                                            { true, true, true, true, true, true, true, true, true, true, true },
                                            { true, true, true, true, true, true, true, true, true, true, true },
                                            { true, true, true, true, true, true, true, true, true, true, true } };

            for (int x = 0; x < shape_1.GetUpperBound(0); x++)
            {
                for (int y = 0; y < shape_1.GetUpperBound(1); y++)
                {
                    structureMap[(int)testPoint.x - x - 2, (int)testPoint.y - y + 4] = shape_1[x, y];
                }
            }

        }
        // Make this a sparse matrix??
        return structureOrigins;
    }


    public int[,] GenerateRoad(int[,] biomeMap, Vector2[] structureOrigins, int[] terrainTypes)
    {

        // Find number of road tiles, assume the first one is just for water and the next for everything else
        int road_tile_no = 0;
        int first_road_ind = 0;
        for (int i = 0; i < terrainTypes.Length; i++)
        {
            if (terrainTypes[i] == 2)
            {
                road_tile_no++;
                if (first_road_ind == 0)
                {
                    first_road_ind = i;
                }
            }
        }

        //Debug.Log(first_road_ind);


        //int[,] roadMap = new int[width, height];
        TerrainGrid grid = new TerrainGrid(); //GetComponent<TerrainGrid>(); //
        PathFinder2 newPath = new PathFinder2();

        // For each structure choose at least one thing to connect it to 
        // landing pad
        grid.biomeMap = biomeMap;


        grid.unpassableTiles = new int[] { 1 };
        grid.penaltyTiles = new int[] { 0, 2, 3 };
        grid.movementPenalty = new int[] { 100, 10, 20 };
        grid.bonusTiles = new int[] { 6, 7 };
        grid.movementBonus = new int[] { 1000, 1000 };

        grid.structures = structureMap;
        grid.CreateGrid();

        newPath.grid = grid;

        // Array to store road data in
        int[,] roadMap = new int[biomeMap.GetUpperBound(0), biomeMap.GetUpperBound(1)];

        List<int> pathTargets = null;
        pathTargets = Enumerable.Range(0, structureOrigins.Length).ToList();

    //private int _current = 0;
    //void Start()
    //{
        //_questions = Enumerable.Range(0, questionPool.Length).ToList();

        //List<int> numbersToChooseFrom = new List<int>(new int[] { 1, 2, 3, 4, 5 });
        // For each structure at one road is connected to it 
        for (int i = 0; i < structureOrigins.Length; i++)
        {

            List<int> possibleTargets = new List<int>(pathTargets);

            //possibleTargets = pathTargets;
            //Debug.Log(possibleTargets.Count);
            possibleTargets.RemoveAt(i);

            // Choose a random different thing to connect to 
            int randInd = Random.Range(0, possibleTargets.Count);

            // Convert to Vector2Int
            Vector2Int startPos = new Vector2Int((int)structureOrigins[i].x, (int)structureOrigins[i].y);
            Vector2Int endPos = new Vector2Int((int)structureOrigins[randInd].x, (int)structureOrigins[randInd].y);

            newPath.FindPath(startPos, endPos);

            if (newPath.grid.path != null)
            {
                for (int node_in_path = 0; node_in_path < newPath.grid.path.Count; node_in_path++)
                {
                    int biomeMap_val = biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY];
                    bool is_road = false;

                    // Check that this isn't already a road
                    for (int road_ind = first_road_ind; road_ind < first_road_ind + road_tile_no; road_ind++)
                    {
                        if (biomeMap_val == road_ind)
                        {
                            is_road = true;
                        }
                    }

                    // If it is not a road the value in biome map is altered
                    if (!is_road)
                    {
                        if (biomeMap_val != 0 | road_tile_no == 1)
                        {
                            biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] = first_road_ind;
                        }
                        else
                        {
                            biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] = first_road_ind + 1;
                        }
                    }
                    else
                    {
                        break;
                    }
                    
                    //int 
                    // If the tile underneath is a water tile a different round surface is used 
                    //if (biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] == 2 | biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] == 3)
                    //{
                    //    break;
                    //}
                    //else
                    //{
                    //    if (biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] == 0)
                    //    {
                    //        biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] = 3;
                    //    }
                    //    else
                    //    {
                    //        biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] = 2;
                    //    }
                    //}

                    //if (biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] != 2)
                    //{
                    //    biomeMap[newPath.grid.path[node_in_path].gridX, newPath.grid.path[node_in_path].gridY] = 2;
                    //}
                    //else
                    //{
                    //    break;
                    //}
                    //Debug.Log(newPath.grid.path[node_in_path].gridX);
                }
            }

        }
        //foreach (Vector2 pos in structureOrigins)
        //{
        //    Debug.Log(pos);
        //}



        //Vector2Int roadEnds;








        //// Convert to Vector2Int
        //startPos = new Vector2Int((int)structureOrigins[2].x, (int)structureOrigins[2].y);
        //endPos = new Vector2Int((int)structureOrigins[3].x, (int)structureOrigins[3].y);

        //newPath.FindPath(startPos, endPos);



        //Pathfinder.FindPath(startPos, endPos);

        // Create grid for path finding algorithm
        //Grid grid = GetComponent<Grid>();

        //grid.gridSizeX = biomeMap.GetUpperBound(0);
        //grid.gridSizeY = biomeMap.GetUpperBound(1);

        // //= gridSizeY;
        ////map = TerrainFunctions.BaseTerrain(gridSizeX, gridSizeY, new Vector2(0f, 0f), weights);
        ////biomeMap = TerrainFunctions.GenerateBiome(gridSizeX, gridSizeY, new Vector2(0f, 0f), map, 6, 0.25f, 0.45f);

        //grid.biomeMap = biomeMap;


        // A* algorithm https://www.youtube.com/watch?v=3Dw5d7PlcTM&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=4

        //Debug.Log("W");
        return biomeMap;
    }




    //public int MaxSize
    //{
    //    get
    //    {
    //        return gridSizeX * gridSizeY;
    //    }
    //}

    //public void CreateGrid()
    //{
    //    grid = new Node[gridSizeX, gridSizeY];
    //    Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

    //    for (int x = 0; x < gridSizeX; x++)
    //    {
    //        for (int y = 0; y < gridSizeY; y++)
    //        {
    //            Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
    //            //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
    //            bool walkable = biomeMap[x, y] > 2;
    //            bool pathExists = false;
    //            int movementPenalty = 0;
    //            if (biomeMap[x, y] == 4)
    //            {
    //                movementPenalty = 1000;
    //            }
    //            grid[x, y] = new Node(walkable, pathExists, worldPoint, x, y, movementPenalty);
    //        }
    //    }
    //}

    //public List<Node> GetNeighbours(Node node)
    //{
    //    List<Node> neighbours = new List<Node>();

    //    for (int x = -1; x <= 1; x++)
    //    {
    //        for (int y = -1; y <= 1; y++)
    //        {
    //            if (x == 0 && y == 0)
    //                continue;

    //            int checkX = node.gridX + x;
    //            int checkY = node.gridY + y;

    //            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
    //            {
    //                neighbours.Add(grid[checkX, checkY]);
    //            }
    //        }
    //    }

    //    return neighbours;
    //}






    //public static int[,] biomeOverlay(int width, int height, Vector2 offset, int biomeTypes)
    //{
    //    // Array to store terrain data in
    //    int[,] biomeMap = new int[width, height];
    //    int PerlinVal;

    //    // Find Perlin noise value at each grid point
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {

    //            PerlinVal = Mathf.RoundToInt((float)biomeTypes * Mathf.PerlinNoise(6.0f * x / width + offset.x, 6.0f * y / height + offset.y));
    //            biomeMap[x, y] = PerlinVal;

    //        }

    //    }

    //    return biomeMap;
    //}











    // Makes a predefined environment for testing stuff
    public int[,] PredefinedBiome(int width, int height, int no_of_biomes)
    {

        // Array to store biome data in
        int[,] biomeMap = new int[width, height];

        int gridWidth = width / 4;
        int gridHeight = height / 4;

        // Divide the centre of the array into squares and assign a biome type to each, the outer values should be zero (for water)       
        for (int x = gridWidth-1; x < 2*gridWidth+1; x++)
        {
            for (int y = gridHeight-1; y < 2*gridHeight+1; y++)
            {
                biomeMap[x, y] = 1;
            }
        }
        for (int x = gridWidth-1; x < 2*gridWidth+1; x++)
        {
            for (int y = 2*gridHeight-1; y < 3*gridHeight+1; y++)
            {
                biomeMap[x, y] = 2;
            }
        }
        for (int x = 2*gridWidth-1; x < 3*gridWidth+1; x++)
        {
            for (int y = 2*gridHeight-1; y < 3*gridHeight+1; y++)
            {
                biomeMap[x, y] = 3;
            }
        }


        //        // Rescale the coordinates so that -1 < x,y < 1, the maximum value is in the middle of the map (x=width, y=height), has values in the range [0,1]
        //        GaussVal = Mathf.Exp(-80 * (Mathf.Pow(2 * (float)x / width - 1, 4) + Mathf.Pow(2 * (float)y / height - 1, 4)));

        //        // Find the normalised distance to the centre
        //        radius = Mathf.Pow(2 * (float)x / width - 1, 2) + Mathf.Pow(2 * (float)y / height - 1, 2);

        //        // Find the Perlin noise at this point, has values in the range [0,1]
        //        PerlinVal_0 = Mathf.PerlinNoise(2.0f * x / width + offset.x, 2.0f * y / height + offset.y);
        //        PerlinVal_1 = Mathf.PerlinNoise(4.0f * x / width + offset.x, 4.0f * y / height + offset.y);
        //        PerlinVal_2 = Mathf.PerlinNoise(8.0f * x / width + offset.x, 8.0f * y / height + offset.y);
        //        PerlinVal_3 = Mathf.PerlinNoise(16.0f * x / width + offset.x, 16.0f * y / height + offset.y);

        //        // The differnt noises are summed with different weights
        //        float weightSum = weights[0] + weights[1] + weights[2] + weights[3];
        //        PerlinVal = (weights[0] * PerlinVal_0 + weights[1] * PerlinVal_1 + weights[2] * PerlinVal_2 + weights[3] * PerlinVal_3) / weightSum;

        //        // This tries to make sure that the terrain map height is low near the edges
        //        terrainMap[x, y] = (3.0f * PerlinVal - 1.2f * radius) / 4.0f;

        //    }
        //}

        return biomeMap;
    }

}



public class TerrainGrid// : MonoBehaviour
{

    Node[,] grid;
    public int[,] biomeMap;
    public bool[,] structures;
    // This list stores the path
    public List<Node> path;

    public int[] unpassableTiles; // No path passes through these tiles
    public int[] penaltyTiles; // There is a penalty for having a path through these
    public int[] movementPenalty; // This is the corresponding penalty
    public int[] bonusTiles; // These tiles are easy to pass
    public int[] movementBonus;

    public void CreateGrid()
    {
        grid = new Node[biomeMap.GetUpperBound(0), biomeMap.GetUpperBound(1)];

        for (int x = 0; x < biomeMap.GetUpperBound(0); x++)
        {
            for (int y = 0; y < biomeMap.GetUpperBound(1); y++)
            {
                //if (structures[x, y])
                //{
                //    Debug.Log("HA");
                //}
                //Debug.Log(structures[x, y]);
                bool walkable = true;
                int pathPenalty = 0;
                int pathBonus = 0;
                // Make this node untraversable if the biome map is untraversible or if any of the surrounding tiles are 
                for (int i = Mathf.Max(0, x - 1); i <= Mathf.Min(biomeMap.GetUpperBound(0) - 1, x + 1); i++)
                {
                    for (int j = Mathf.Max(0, y - 1); j <= Mathf.Min(biomeMap.GetUpperBound(1) - 1, y + 1); j++)
                    {
                        for (int ind = 0; ind < unpassableTiles.Length; ind++)
                        {
                            if ((biomeMap[i, j] == unpassableTiles[ind]) | structures[x, y])
                            {
                                walkable = false;
                                break;
                            }
                        }
                    }
                }

                
                for (int ind = 0; ind < penaltyTiles.Length; ind++)
                {
                    if (biomeMap[x, y] == penaltyTiles[ind])
                    {
                        pathPenalty = movementPenalty[ind];
                    }
                }

                for (int ind = 0; ind < bonusTiles.Length; ind++)
                {
                    if (biomeMap[x, y] == bonusTiles[ind])
                    {
                        pathBonus = movementBonus[ind];
                    }
                }

                //bool walkable = biomeMap[x, y] > 2; //true;
                //Array.Exists(1, 1);
                //Debug.Log(Contains(biomeMap[x, y], unwalkable));// biomeMap[x, y] > 2;

                //if (biomeMap[x, y] == 4)
                //{
                //    movementPenalty = 1000;
                //}
                grid[x, y] = new Node(walkable, x, y, pathPenalty, pathBonus);
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

                if (checkX >= 0 && checkX < biomeMap.GetUpperBound(0) && checkY >= 0 && checkY < biomeMap.GetUpperBound(1))
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public int MaxSize
    {
        get
        {
            return biomeMap.GetUpperBound(0) * biomeMap.GetUpperBound(1);
        }
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

//    void OnDrawGizmosSelected()
//    {
////#if UNITY_EDITOR
//        Gizmos.DrawWireCube(new Vector3(0, 0, -10), new Vector3(biomeMap.GetUpperBound(0), biomeMap.GetUpperBound(1), 1));
//        Vector3 worldPosition;

//        Debug.Log("WA");

//        if (grid != null)
//        {
            
//            foreach (Node n in grid)
//            {
//                worldPosition = new Vector3(n.gridX, n.gridY, -10);

//                //Gizmos.color = (n.walkable) ? Color.white : Color.red;
//                if (n.walkable & n.movementPenalty == 0)
//                {
//                    Gizmos.color = Color.white;
//                }
//                else if (n.walkable & n.movementPenalty != 0)
//                {
//                    Gizmos.color = Color.green;
//                }
//                else
//                {
//                    Gizmos.color = Color.red;
//                }
//                Gizmos.DrawCube(worldPosition, Vector3.one);
//            }
//        }
////#endif
//    }

}

