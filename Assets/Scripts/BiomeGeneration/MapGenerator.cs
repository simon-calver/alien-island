using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//using System;

public class MapGenerator : MonoBehaviour
{
    // Gizmo parameters


    // Tilemaps for the biomes, they all should be on the same grid. And the corressponding
    // rule tiles
    public Tilemap[] biomeTilemap = new Tilemap[1];
    public TileBase[] biomeTiles = new TileBase[1];

    // Array for what type these tiles are, alters how they are drawn etc. Currently there are 4 types, water, rock,
    // road & biome: [0,1,2,3]
    public int[] tileTypes = new int[1];









    public Tilemap[] detailTilemap = new Tilemap[2];
    public TileBase[] detailTiles = new TileBase[1];

    public Tilemap[] roadTilemap = new Tilemap[1];
    public TileBase[] roadTiles = new TileBase[1];

    // The tiles with colliders are added to a seperate tilemap with different physics
    //public Tilemap[] colliderTilemap = new Tilemap[1];
    //public TileBase[] colliderTiles = new TileBase[2];

    // Size of the map
    public int width = 50;
    public int height = 40;

    // Parameters to modify the Perlin noise. These could be randomised
    public Vector2 heightOffset;// = new Vector2(0.0f, 0.0f);
    public Vector2 biomeOffset;// = new Vector2(0.0f, 0.0f);
    public float[] weights = new float[4];

    public bool usePredefined;

    // Boundaries in the height map for rock and water
    private float water_bound = 0.25f;
    private float rock_bound = 0.45f;

    // The float array defines the height of the island, the int arrays are used to define 
    // the biome types 
    public float[,] map;
    public int[,] biomeMap;
    public Vector2[] structureMap;
    public int[,] roadMap;

    //Grid grid;

    TerrainFunctions terrainFunctions = new TerrainFunctions();
    //public GameObject defaultStructure;
    //public int structureCount;

    public GameObject[] structuresInScene; //  = new GameObject[6];

    public void RandomiseMap()
    {
        heightOffset = new Vector2(Random.Range(-1e4f, 1e4f), Random.Range(-1e4f, 1e4f));
        biomeOffset = new Vector2(Random.Range(-1e4f, 1e4f), Random.Range(-1e4f, 1e4f));
    }

    // Generate the random terrain
    public void GenerateMap()
    {
        // Randomise shape of map

        // Remove existing tiles from the map
        ClearMap();

        // Delete existing structures in scene
        //for (int i = 0; i < structuresInScene.Length; i++)
        //{
        //    Destroy(structuresInScene[i]);
        //}


            // The float array defines the height of the island, the int arrays are used to define 
            // the biome types 
            //map = new float[width, height];
            //biomeMap = new int[width, height];
            //int[,] treeMap = new int[width, height];

        // The number of biomes depends on how many tile types are supplied
        int no_of_biomes = biomeTiles.Length;

        // Generate the terrain and biome maps or use a predefined map
        if (usePredefined)
        {
            biomeMap = terrainFunctions.PredefinedBiome(width, height, no_of_biomes);
        }
        else
        {
            map = terrainFunctions.BaseTerrain(width, height, heightOffset, weights);
            biomeMap = terrainFunctions.GenerateBiome(width, height, biomeOffset, map, no_of_biomes, water_bound, rock_bound, tileTypes);
            //treeMap = TerrainFunctions.biomeOverlay(width, height, new Vector2(offset.x - 100.0f, offset.y + 20.0f), 6);
            
        }
        // Add the structures
        structureMap = terrainFunctions.PlaceStructures(biomeMap, structuresInScene);

        //roadMap
        biomeMap = terrainFunctions.GenerateRoad(biomeMap, structureMap, tileTypes);



        // Draw the biome tiles on to the tilemap
        TilemapFunctions.DrawBiomeTilemap(biomeMap, biomeTilemap, biomeTiles, detailTilemap, detailTiles, terrainFunctions.structureMap, tileTypes);


        //TilemapFunctions.DrawRoadTilemap(biomeMap, roadTilemap, roadTiles);//, detailTilemap, detailTiles, terrainFunctions.structureMap);


        //PathFinder2 waa = new PathFinder2();

        //waa.gridSizeX = 9;

        ////waa.PlaceStructures(width, height, biomeMap, structuresInScene); //
        //waa.FindPath(new Vector2Int(1, 2), new Vector2Int(3, 4));



    }

    // This removes the tiles from all the tilemaps
    public void ClearMap()
    {
        //for (int i = 0; i < colliderTilemap.Length; i++)
        //{
        //    colliderTilemap[i].ClearAllTiles();
        //}

        for (int i = 0; i < biomeTilemap.Length; i++)
        {
            biomeTilemap[i].ClearAllTiles();
        }


        for (int i = 0; i < detailTilemap.Length; i++)
        {
            detailTilemap[i].ClearAllTiles();
        }


        for (int i = 0; i < detailTilemap.Length; i++)
        {
            roadTilemap[i].ClearAllTiles();
        }
    }

    void OnDrawGizmosSelected()
    {
        //#if UNITY_EDITOR
        //Gizmos.DrawWireCube(new Vector3(0, 0, -10), new Vector3(biomeMap.GetUpperBound(0), biomeMap.GetUpperBound(1), 1));
        Vector3 worldPosition;

        Color[] highlightColours = { new Color(0.05f, 0.05f, 0.05f, 0.4f),
                                     new Color(0.18f, 0.37f, 0.23f, 0.4f) };
            
            
        //    Color(0.27f, 0.18f, 0.18f, 0.4f);
        //Color showColour = new Color(0.27f, 0.18f, 0.18f, 0.4f);
        if (biomeMap != null)//terrainFunctions.structureMap != null)
        {
            for (int x = 0; x < biomeMap.GetUpperBound(0); x++)
            {
                for (int y = 0; y < biomeMap.GetUpperBound(1); y++)
                {
                    worldPosition = new Vector3(x + 0.5f, y + 0.5f, -10);

                    if (biomeMap[x, y] == 2)//terrainFunctions.structureMap[x, y])
                    {
                        Gizmos.color = highlightColours[0];
                    }
                    else
                    {
                        Gizmos.color = highlightColours[1];
                    }

                    Gizmos.DrawCube(worldPosition, Vector3.one);
                }
            }
        }
    }

    //            if (grid != null)
    //    {

    //        foreach (Node n in grid)
    //        {
    //            worldPosition = new Vector3(n.gridX, n.gridY, -10);

    //            //Gizmos.color = (n.walkable) ? Color.white : Color.red;
    //            if (n.walkable & n.movementPenalty == 0)
    //            {
    //                Gizmos.color = Color.white;
    //            }
    //            else if (n.walkable & n.movementPenalty != 0)
    //            {
    //                Gizmos.color = Color.green;
    //            }
    //            else
    //            {
    //                Gizmos.color = Color.red;
    //            }
               
    //        }
    //    }
    //    //#endif
    //}


}
