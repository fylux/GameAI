using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


public class Map : MonoBehaviour {
    [SerializeField]
    bool generateMap;

    public string fileNameToLoad;

    public int mapX, mapY;

    public GameObject A, B, C, D, E, influenceTile;

    int[,] tiles;

    public Node[,] grid = null;
    Vector2 gridSize;
    float nodeX, nodeY;

    /*The offset is fundamental because by default objects are position based on their center. Thus, if we put a plane of 1x1 in (0,0), the upper left part
    will be in (-0.5,-0.5). Since we want the terrain to be in the range from 0 to gridSize, we need to add an offset*/
    Vector3 offset;


    void Awake() {
        this.transform.position = Vector3.zero;
        Debug.Log(Application.dataPath + "/" + fileNameToLoad);

        Vector3 size = A.GetComponent<Renderer>().bounds.size;
        nodeX = size.x;
        nodeY = size.z;

        offset = new Vector3(size.x, 0, size.z) / 2f ;
        gridSize = new Vector2(mapX * nodeX, mapY * nodeY);

        tiles = Load(Application.dataPath + "/" + fileNameToLoad);
        if (tiles == null || tiles.GetLength(0) != mapX && tiles.GetLength(1) != mapY) {
            Debug.LogError("Size of the map does not match: " + tiles.GetLength(0) + "x" + tiles.GetLength(1));
        }

        BuildMap();
        if (generateMap) GenerateMap();
    }

    void BuildMap() {
        grid = new Node[mapX, mapX];

        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                Vector3 position = offset + (Vector3.right * x * nodeX) + (Vector3.forward * y * nodeY);
                NodeT type = (NodeT)tiles[x, y];
                grid[x, y] = new Node(x, y, position, type);
                if (!generateMap) grid[x, y].gameObject = transform.GetChild(x * mapY + y).gameObject;
            }
        }
    }

    void GenerateMap() {
        Dictionary<int, GameObject> terrainType = new Dictionary<int, GameObject>() { { 0, A }, { 1, B }, { 2, C }, { 3, D }, { 4, E } };
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                GameObject TilePrefab = Instantiate(terrainType[tiles[x, y]], offset + new Vector3(x * nodeX, 0, y * nodeY), Quaternion.Euler(90, 0, 0));
                TilePrefab.transform.parent = this.transform;

                GameObject influenceTileObj = Instantiate(influenceTile, offset + new Vector3(x * nodeX, -0.5f, y * nodeY), Quaternion.Euler(90, 0, 0));
                influenceTileObj.transform.parent = TilePrefab.transform;
                grid[x, y].gameObject = TilePrefab;
            }
        }
    }

    int[,] Load(string filePath) {
        try {
            using (StreamReader sr = new StreamReader(filePath)) {
                string[] lines = sr.ReadToEnd().Split(new[] {'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
                int[,] tiles = new int[lines.Length, mapX];

                for (int i = 0; i < lines.Length; i++) {
                    string[] nums = lines[i].Split(new[] { ',' });
                    for (int j = 0; j < Mathf.Min(nums.Length, mapX); j++) {
                        if (!int.TryParse(nums[j], out tiles[i, j])) {
                            Debug.LogError("Cannot parse" + nums[j]);
                        }
                    }
                }
                return tiles;
            }
        } catch (IOException e) {
            Debug.Log(e.Message);
            return null;
        }
    }

    public int GetMaxSize() {
        return mapX * mapY;
    }

    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < mapX && checkY >= 0 && checkY < mapY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public List<Node> GetDirectNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();
        
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;
                if (Mathf.Abs(x) > 0 && Mathf.Abs(y) > 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < mapX && checkY >= 0 && checkY < mapY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node NodeFromPosition(Vector3 nodePosition) {
        float percentX = Mathf.Clamp01(nodePosition.x / gridSize.x);
        float percentY = Mathf.Clamp01(nodePosition.z / gridSize.y);

        int x = Mathf.FloorToInt(mapX * percentX);
        int y = Mathf.FloorToInt(mapY * percentY);

        Assert.IsTrue(x < mapX && y < mapY);
        return grid[x, y];
    }


    public void SetInfluence() {
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                Vector3 size = Vector3.one;
                float influ = grid[x, y].getInfluence() / 100f;
                float otherColor = Mathf.Max(0f,1f - influ * 4f);
                //otherColor = Mathf.Round(otherColor * 5f) / 5f; //To discretize the range of colors
                Dictionary<Faction, Color> colors = new Dictionary<Faction, Color>() {
                    {Faction.A, new Color(1f, otherColor, otherColor, 1f) },
                    {Faction.B, new Color(otherColor, otherColor, 1f ,1f) },
                    {Faction.C, Color.gray }
                };

                grid[x, y].gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = colors[grid[x, y].getFaction()];
                /*Gizmos.color = colors[grid[x, y].getFaction()];
                Gizmos.DrawCube(grid[x, y].worldPosition, size);*/
            }
        }
    }

    public void ResetInfluence() {
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                grid[x, y].ResetInfluence();
            }
        }

    }
}
