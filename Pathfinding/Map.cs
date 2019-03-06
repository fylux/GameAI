using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour {
    [SerializeField]
    bool generateMap;

    public string fileNameToLoad;

    public int mapX, mapY;

    public GameObject A, B, C, D, E;

    private int[,] tiles;

    public Node[,] grid;
    Vector2 gridSize;
    float nodeX, nodeY;

    /*The offset is fundamental because by default objects are position based on their center. Thus, if we put a plane of 1x1 in (0,0), the upper left part
    will be in (-0.5,-0.5). Since we want the terrain to be in the range from 0 to gridSize, we need to add an offset*/
    Vector3 offset;

    public Text textGrid;


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
        gameObject.isStatic = true;
    }

    void BuildMap() {
        
        grid = new Node[mapX, mapX];

        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                Vector3 position = offset + (Vector3.right * x * nodeX) + (Vector3.forward * y * nodeY);
                NodeT type = (NodeT)tiles[x, y];
                grid[x, y] = new Node(x, y, position, type);
            }
        }

        if (!generateMap)
            return;

        Dictionary<int, GameObject> terrainType = new Dictionary<int, GameObject>() {{0, A}, {1, B}, {2, C}, {3, D}, {4, E}};
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                GameObject TilePrefab = Instantiate(terrainType[tiles[x, y]], offset + new Vector3(x * nodeX, 0, y * nodeY), Quaternion.identity);
                TilePrefab.transform.parent = this.transform;
            }
        }
    }

    private int[,] Load(string filePath) {
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

    public Node NodeFromPosition(Vector3 nodePosition) {
        float percentX = Mathf.Clamp01(nodePosition.x / gridSize.x);
        float percentY = Mathf.Clamp01(nodePosition.z / gridSize.y);

        int x = Mathf.FloorToInt(mapX * percentX);
        int y = Mathf.FloorToInt(mapY * percentY);

        return grid[x, y];
    }

    /*
    void Update() {
        textGrid.text = "";
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                textGrid.text += grid[x, y].hCost.ToString("F1");
            }
            textGrid.text += "\n";
        }
    }*/
}
