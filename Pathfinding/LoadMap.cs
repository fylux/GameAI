using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class LoadMap : MonoBehaviour {
    [SerializeField]
    bool generateMap;

    [SerializeField]
    string fileNameToLoad;

    [SerializeField]
    int mapX, mapY;

    [SerializeField]
    GameObject Road, Grass, Forest, Water, Mountain, influenceTile, influenceGrid;

    NodeT[,] tiles;
    float nodeSizeX, nodeSizeY;

    /*The offset is fundamental because by default objects are position based on their center. Thus, if we put a plane of 1x1 in (0,0), the upper left part
    will be in (-0.5,-0.5). Since we want the terrain to be in the range from 0 to gridSize, we need to add an offset*/
    Vector3 offset;


    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Info.GetWaypoint("base",Faction.B).worldPosition, 10);
    }*/

    void Awake() {
        this.transform.position = Vector3.zero;
        Debug.Log("Loading map from "+Application.dataPath + "/" + fileNameToLoad);

        Vector3 size = Road.GetComponent<Renderer>().bounds.size;
        nodeSizeX = size.x;
        nodeSizeY = size.z;

        offset = new Vector3(size.x, 0, size.z) / 2f;
        Vector2 gridSize = new Vector2(mapX * nodeSizeX, mapY * nodeSizeY);

        tiles = Load(Application.dataPath + "/" + fileNameToLoad);
        if (tiles == null || tiles.GetLength(0) != mapX && tiles.GetLength(1) != mapY) {
            Debug.LogError("Size of the map does not match: " + tiles.GetLength(0) + "x" + tiles.GetLength(1));
        }
        if (generateMap)
            {
                GenerateMap();
            }

        Map.Init(BuildMap(), gridSize);
        Info.Init();
    }

    Node[,] BuildMap() {
        Node[,] grid = new Node[mapX, mapX];

        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                Vector3 position = offset + (Vector3.right * x * nodeSizeX) + (Vector3.forward * y * nodeSizeY);
                NodeT type = (NodeT)tiles[x, y];
                grid[x, y] = new Node(x, y, position, type);
                grid[x, y].influenceTile = influenceGrid.transform.GetChild(x * mapY + y).gameObject;
            }
        }
        return grid;
    }

    void GenerateMap() {
        Dictionary<NodeT, GameObject> terrainType = new Dictionary<NodeT, GameObject>()
        { { NodeT.ROAD, Road }, { NodeT.GRASS, Grass }, { NodeT.FOREST, Forest }, { NodeT.WATER, Water }, { NodeT.MOUNTAIN, Mountain } };

        bool[,] used = new bool[mapX, mapX];
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                GameObject influenceTileObj = Instantiate(influenceTile, offset + new Vector3(x * nodeSizeX, -0.5f, y * nodeSizeY), Quaternion.Euler(90, 0, 0));
                influenceTileObj.transform.parent = influenceGrid.transform;
                influenceTileObj.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);

                if (used[x, y]) {
                    continue;
                }

                int x1 = x, y1 = y;
                NodeT type = tiles[x, y];

                while (x1 + 1 < mapX && Enumerable.Range(y, y1 - y + 1).All(i => tiles[x1 + 1, i] == type && !used[x1 + 1, y])) {
                    for (int i = y; i <= y1; ++i) {
                        used[x1 + 1, i] = true;
                    }
                    x1++;
                }

                while (y1 + 1 < mapY && Enumerable.Range(x, x1-x + 1).All(i => tiles[i, y1 + 1] == type && !used[i, y1 + 1])) {
                    for (int i = x; i <= x1; ++i) {
                        used[i, y1 + 1] = true;
                    }
                    y1++;
                }

                int rows = x1 - x + 1;
                int cols = y1 - y + 1;

                float middleX = (float)(x + x1) / 2f;
                float middleY = (float)(y + y1) / 2f;

                GameObject TilePrefab = Instantiate(terrainType[tiles[x, y]], offset + new Vector3(middleX * nodeSizeX, 0, middleY * nodeSizeY), Quaternion.Euler(90, 0, 0));
                TilePrefab.transform.localScale = new Vector3(rows, cols, 1f);
                TilePrefab.transform.parent = this.transform;
            }
        }
    }

    NodeT[,] Load(string filePath) {
        try {
            using (StreamReader sr = new StreamReader(filePath)) {
                string[] lines = sr.ReadToEnd().Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                NodeT[,] tiles = new NodeT[lines.Length, mapX];

                for (int i = 0; i < lines.Length; i++) {
                    string[] nums = lines[i].Split(new[] { ',' });
                    for (int j = 0; j < Mathf.Min(nums.Length, mapX); j++) {
                        int indexType;
                        if (!int.TryParse(nums[j], out indexType)) {
                            Debug.LogError("Cannot parse" + nums[j]);
                        }
                        tiles[i, j] = (NodeT)indexType;
                    }
                }
                return tiles;
            }
        } catch (IOException e) {
            Debug.Log(e.Message);
            return null;
        }
    }
}
