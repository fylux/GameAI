﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


public class Map {
    public static int mapX, mapY;
    public static Node[,] grid = null;
    static Vector2 gridSize;

    public static HashSet<AgentUnit> unitList;

    public static LayerMask unitsMask, healingMask, terrainMask, influenceMask;


    //This is called from load map
    public static void Init(Node[,] _grid, Vector2 _gridSize) {
        
        grid = _grid;
        gridSize = _gridSize;
        mapX = grid.GetLength(0);
        mapY = grid.GetLength(1);
        unitList = new HashSet<AgentUnit>();
        
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC")) {
            unitList.Add(npc.GetComponent<AgentUnit>());
        }

        unitsMask = LayerMask.GetMask("Unit");
        healingMask = LayerMask.GetMask("Healing");
        terrainMask = LayerMask.GetMask("Terrain");
        influenceMask = LayerMask.GetMask("Influence");
    }

    public static HashSet<AgentUnit> GetAllies(Faction faction) {
        return new HashSet<AgentUnit>(Map.unitList.Where(agent => agent.faction == faction));
    }

    public static HashSet<AgentUnit> GetEnemies(Faction faction) {
        return new HashSet<AgentUnit>(Map.unitList.Where(agent => agent.faction == Util.OppositeFaction(faction)));
    }

    public static int GetMaxSize() {
        return mapX * mapY;
    }

    public static List<Node> GetNeighbours(Node node) {
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

    public static List<Node> GetDirectNeighbours(Node node) {
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

    public static Node NodeFromPosition(Vector3 nodePosition) {
        float percentX = Mathf.Clamp01(nodePosition.x / gridSize.x);
        float percentY = Mathf.Clamp01(nodePosition.z / gridSize.y);

        int x = Mathf.FloorToInt(mapX * percentX);
        int y = Mathf.FloorToInt(mapY * percentY);

        Assert.IsTrue(x < mapX && y < mapY);
        return grid[x, y];
    }


    public static void SetInfluence() {
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                Vector3 size = Vector3.one;
                float influ = grid[x, y].GetInfluence() / 100f;
                float otherColor = Mathf.Max(0f,1f - influ * 4f);

                //otherColor = Mathf.Round(otherColor * 5f) / 5f; //To discretize the range of colors
                Dictionary<Faction, Color> colors = new Dictionary<Faction, Color>() {
                    {Faction.A, new Color(1f, otherColor, otherColor, 1f) },
                    {Faction.B, new Color(otherColor, otherColor, 1f ,1f) },
                    {Faction.C, Color.gray }
                };

                influ = grid[x, y].GetInfluence(Faction.B);
                if (influ > 0.65) {
                    grid[x, y].influenceTile.GetComponent<Renderer>().material.color = Color.black;

                } 
                else if (influ > 0.4) {
                    grid[x, y].influenceTile.GetComponent<Renderer>().material.color = Color.red;

                } else if (influ > 0.27) {
                    grid[x, y].influenceTile.GetComponent<Renderer>().material.color = Color.yellow;

                } else {
                    grid[x, y].influenceTile.GetComponent<Renderer>().material.color = Color.white;
                }
                grid[x, y].influenceTile.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
                //grid[x, y].gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = colors[grid[x, y].getFaction()];
                /*Gizmos.color = colors[grid[x, y].getFaction()];
                Gizmos.DrawCube(grid[x, y].worldPosition, size);*/
            }
        }
    }

    public static void ResetInfluence() {
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                grid[x, y].ResetInfluence();
            }
        }

    }
}
