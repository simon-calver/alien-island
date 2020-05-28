using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

// Use this to run code from the editor
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BiomeGenerator : MonoBehaviour
{
    // Tilemaps for the biomes, they all should be on the same grid. And the corressponding
    // rule tiles
    public Tilemap[] biomeTilemap = new Tilemap[2];
    public TileBase[] biomeTiles = new TileBase[1];

    // The tiles with colliders are added to a seperate tilemap with different physics
    //public Tilemap[] colliderTilemap = new Tilemap[1];
    //public TileBase[] colliderTiles = new TileBase[2];

    // Size of the map
    public int width = 50;
    public int height = 40;

    // Parameters to modify the Perlin noise. These could be randomised
    public Vector2 heightOffset = new Vector2(0.0f, 0.0f);
    public Vector2 biomeOffset = new Vector2(0.0f, 0.0f);
    public float [] weights = new float[4];

    public bool usePredefined;
    public int[] tileTypes = new int[1];
    // Boundaries in the height map for rock and water
    private float water_bound = 0.25f;
    private float rock_bound = 0.45f;

    // The float array defines the height of the island, the int arrays are used to define 
    // the biome types 
    public float[,] map;
    public int[,] biomeMap;

    TerrainFunctions terrainFunctions = new TerrainFunctions();

    // Generate the random terrain in the editor
    [ExecuteInEditMode]
    public void GenerateMap()
    {
        // Remove existing tiles from the map
        ClearMap();

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

        bool[,] temp = new bool[width, height];
        // Draw the biome tiles on to the tilemap
        TilemapFunctions.DrawBiomeTilemap(biomeMap, biomeTilemap, biomeTiles, biomeTilemap, biomeTiles, temp, tileTypes);

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
    }

}

// This runs the code in the editor
[CustomEditor(typeof(BiomeGenerator))]
public class BiomeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Reference to the BiomeGenerator script
        BiomeGenerator levelGen = (BiomeGenerator)target;

        // Add buttons to run the functions
        if (GUILayout.Button("Generate"))
        {
            levelGen.GenerateMap();
        }

        if (GUILayout.Button("Clear"))
        {
            levelGen.ClearMap();
        }

    }
}


