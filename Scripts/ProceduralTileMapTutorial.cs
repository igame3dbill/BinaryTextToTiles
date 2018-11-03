using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.Tilemaps
{

    public class ProceduralTileMapTutorial : MonoBehaviour
    {
        [SerializeField] Tilemap tilemap;
        [SerializeField] TileBase tile;
        [SerializeField] int height;
        [SerializeField] int width;
        [SerializeField] bool empty;
        [SerializeField] float seed;
        /// <summary>
        /// Tutorial from https://blogs.unity3d.com/2018/05/29/procedural-patterns-you-can-use-with-tilemaps-part-i/
        /// </summary>
        /// <returns></returns>
        private void Start()
        {
            seed = Time.fixedTime;
            int[,] map = new int[width, height];
            map = GenerateArray(width, height, empty);
           
            map = PerlinNoise(map, seed);
            RenderMap(map, tilemap, tile);
         
           //UpdateMap(map, tilemap);
        }

    // GenerateArray creates a new int array of the size given to it. 
    // We can also say whether the array should be full or empty (1 or 0). Here’s the code:
    public static int[,] GenerateArray(int width, int height, bool empty)
        {
            int[,] map = new int[width, height];
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    if (empty)
                    {
                        map[x, y] = 0;
                    }
                    else
                    {
                        map[x, y] = 1;
                    }
                }
            }
            return map;
        }
       
        //Render our map to the tilemap. 
        //cycle through the width and height of the map, only placing tiles if the array has a 1 at the location we are checking.
        public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
        {
            //Clear the map (ensures we dont overlap)
            tilemap.ClearAllTiles();
            //Loop through the width of the map
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                //Loop through the height of the map
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    // 1 = tile, 0 = no tile
                    if (map[x, y] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                }
            }
        }

        //to update the map, rather than rendering again. This way we can use less resources as we aren’t redrawing every single tile and its tile data.
        public static void UpdateMap(int[,] map, Tilemap tilemap) //Takes in our map and tilemap, setting null tiles where needed
        {
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    //We are only going to update the map, rather than rendering again
                    //This is because it uses less resources to update tiles to null
                    //As opposed to re-drawing every single tile (and collision data)
                    if (map[x, y] == 0)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                    }
                }
            }
        }

        // simplest form of implementing Perlin Noise into level generation.
        public static int[,] PerlinNoise(int[,] map, float seed)
        {
            int newPoint;
            //Used to reduced the position of the Perlin point
            float reduction = 0.5f;
            //Create the Perlin
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));

                //Make sure the noise starts near the halfway point of the height
                newPoint += (map.GetUpperBound(1) / 2);
                for (int y = newPoint; y >= 0; y--)
                {
                    map[x, y] = 1;
                }
            }
            return map;
        }
    

    // SmootherPerlinNoise
    // Set intervals to record the Perlin height, then smooth between the points.
    public static int[,] PerlinNoiseSmooth(int[,] map, float seed, int interval)
    {
        //Smooth the noise and store it in the int array
        if (interval > 1)
        {
            int newPoint, points;
            //Used to reduced the position of the Perlin point
            float reduction = 0.5f;

            //Used in the smoothing process
            Vector2Int currentPos, lastPos;
            //The corresponding points of the smoothing. One list for x and one for y
            List<int> noiseX = new List<int>();
            List<int> noiseY = new List<int>();

            //Generate the noise
            for (int x = 0; x < map.GetUpperBound(0); x += interval)
            {
                newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, (seed * reduction))) * map.GetUpperBound(1));
                noiseY.Add(newPoint);
                noiseX.Add(x);
            }

            points = noiseY.Count;

            //Start at 1 so we have a previous position already
            for (int i = 1; i < points; i++)
            {
                //Get the current position
                currentPos = new Vector2Int(noiseX[i], noiseY[i]);
                //Also get the last position
                lastPos = new Vector2Int(noiseX[i - 1], noiseY[i - 1]);

                //Find the difference between the two
                Vector2 diff = currentPos - lastPos;

                //Set up what the height change value will be 
                float heightChange = diff.y / interval;
                //Determine the current height
                float currHeight = lastPos.y;

                //Work our way through from the last x to the current x
                for (int x = lastPos.x; x < currentPos.x; x++)
                {
                    for (int y = Mathf.FloorToInt(currHeight); y > 0; y--)
                    {
                        map[x, y] = 1;
                    }
                    currHeight += heightChange;
                }
            }
        }

        else
        {
            //Defaults to a normal Perlin gen
            map = PerlinNoise(map, seed);
        }

        return map;
         }
        public static int[,] PerlinNoiseCave(int[,] map, float modifier, bool edgesAreWalls)
        {
            int newPoint;
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {

                    if (edgesAreWalls && (x == 0 || y == 0 || x == map.GetUpperBound(0) - 1 || y == map.GetUpperBound(1) - 1))
                    {
                        map[x, y] = 1; //Keep the edges as walls
                    }
                    else
                    {
                        //Generate a new point using Perlin noise, then round it to a value of either 0 or 1
                        newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
                        map[x, y] = newPoint;
                    }
                }
            }
            return map;
        }
    }


}
