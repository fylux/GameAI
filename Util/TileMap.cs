using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileMap : MonoBehaviour {

    public string fileNameToLoad;

    public int mapWidth;
    public int mapHeight;

    public GameObject A;
    public GameObject B;

    private int[,] tiles;

    void Awake() {
        Debug.Log(Application.dataPath + "/" + fileNameToLoad);
        tiles = Load(Application.dataPath + "/" + fileNameToLoad);
        BuildMap();
    }

    void BuildMap() {
        Debug.Log("Building Map...");
        Vector3 size = A.GetComponent<Renderer>().bounds.size;
        for (int i = 0; i < tiles.GetLength(0); i++) {
            for (int j = 0; j < tiles.GetLength(1); j++) {
                if (tiles[i, j] == 0) {
                    GameObject TilePrefab = Instantiate(A, new Vector3(j*size.x, 0, i*size.z), Quaternion.identity) as GameObject;
                    TilePrefab.transform.parent = this.transform;
                }
                else if(tiles[i, j] == 1) {
                    GameObject TilePrefab = Instantiate(B, new Vector3(j*size.x, 0, i*size.z), Quaternion.identity) as GameObject;
                    TilePrefab.transform.parent = this.transform;
                } 
            }
        }
        Debug.Log("Building Completed!");
    }

    private int[,] Load(string filePath) {
        try {
            Debug.Log("Loading File...");
            using (StreamReader sr = new StreamReader(filePath)) {
                string input = sr.ReadToEnd();
                string[] lines = input.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                int[,] tiles = new int[lines.Length, mapWidth];
                Debug.Log("Parsing...");
                for (int i = 0; i < lines.Length; i++) {
                    string[] nums = lines[i].Split(new[] { ',' });
                    for (int j = 0; j < Mathf.Min(nums.Length, mapWidth); j++) {
                        int val;
                        if (!int.TryParse(nums[j], out tiles[i, j])) {
                            Debug.LogError("Cannot parse"+ nums[j]);
                        }
                    }
                }
                Debug.Log("Parsing Completed!");
                return tiles;
            }
        } catch (IOException e) {
            Debug.Log(e.Message);
            return null;
        }
    }
}
