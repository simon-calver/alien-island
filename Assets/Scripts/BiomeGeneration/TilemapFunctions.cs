using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class TilemapFunctions : MonoBehaviour
{

    public static void DrawBiomeTilemap(int[,] map, Tilemap[] biomeTilemap, TileBase[] biomeTiles, Tilemap[] detailTilemap, TileBase[] detailTiles, bool[,] structureMap, int[] tileTypes)
    {
        // Find how many tile types there are 
        //int tileTypes = biomeTiles.Length;

        // Size of the grid
        int x_bound = map.GetUpperBound(0);
        int y_bound = map.GetUpperBound(1);

        // The integers in map should correspond to what tile is to be drawn where
        for (int x = 0; x < x_bound; x++)
        {
            for (int y = 0; y < y_bound; y++)
            {                                             
                // If any of the tiles around this have a higher index this tile is also plotted there so there is an overlap
                for (int i = Mathf.Max(0, x - 1); i <= Mathf.Min(x_bound - 1, x + 1); i++)
                {
                    for (int j = Mathf.Max(0, y - 1); j <= Mathf.Min(y_bound - 1, y + 1); j++)
                    {
                        if (map[i, j] <= map[x, y])
                        {
                            biomeTilemap[map[x, y]].SetTile(new Vector3Int(i, j, 0), biomeTiles[map[x, y]]);
                        }
                    }

                }

                if (map[x, y] == 3 & !structureMap[x, y])
                {
                    detailTilemap[0].SetTile(new Vector3Int(x, y, 0), detailTiles[0]);
                }
            }

        }


    }

    public static void DrawBridgeDetails(int[,] map, Tilemap detailTilemap, TileBase bridgeDetailTiles, int bridgeInd)//, Tilemap[] detailTilemap, TileBase[] detailTiles, bool[,] structureMap)
    {
        // Size of the grid
        int x_bound = map.GetUpperBound(0);
        int y_bound = map.GetUpperBound(1);

        // The integers in map should correspond to what tile is to be drawn where
        for (int x = 0; x < x_bound; x++)
        {
            for (int y = 0; y < y_bound; y++)
            {
                if (map[x, y] == bridgeInd)
                {
                    detailTilemap.SetTile(new Vector3Int(x, y, 0), bridgeDetailTiles);
                }
            }
        }
    }





        public static void DrawRoadTilemap(int[,] map, Tilemap[] roadTilemap, TileBase[] roadTiles)//, Tilemap[] detailTilemap, TileBase[] detailTiles, bool[,] structureMap)
    {
        // Find how many tile types there are 
        int tileTypes = roadTiles.Length;

        // Size of the grid
        int x_bound = map.GetUpperBound(0);
        int y_bound = map.GetUpperBound(1);

        // The integers in map should correspond to what tile is to be drawn where
        for (int x = 0; x < x_bound; x++)
        {
            for (int y = 0; y < y_bound; y++)
            {
                // If any of the tiles around this have a higher index this tile is also plotted there so there is an overlap
                for (int i = Mathf.Max(0, x - 1); i <= Mathf.Min(x_bound - 1, x + 1); i++)
                {
                    for (int j = Mathf.Max(0, y - 1); j <= Mathf.Min(y_bound - 1, y + 1); j++)
                    {
                        if (map[i, j] >= map[x, y])
                        {
                            roadTilemap[map[x, y]].SetTile(new Vector3Int(i, j, 0), roadTiles[map[x, y]]);
                        }
                    }
                }
            }

        }


    }




}
