using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfluenceMap : MonoBehaviour {

    Map map;
    public List<AgentUnit> unitList;

    const int NNodesInfluenceMap = 1000;
    const int RadiusInfluenceMap = 10;
    const int SecondsPerInfluenceUpdate = 2;

    private void Start() {
        map = GameObject.Find("Terrain").GetComponent<Map>();
        unitList = new List<AgentUnit>();
        Array.ForEach(  GameObject.FindGameObjectsWithTag("NPC"),
                        npc => unitList.Add(npc.GetComponent<AgentUnit>()));
    }

    public void Update() {
        if (Mathf.Floor(Time.fixedTime * 1000) % (1000 * SecondsPerInfluenceUpdate) == 0) { //Time is managed in ms
            map.ResetInfluence();
            unitList.ForEach(unit => ComputeInfluenceDijkstra(unit));
            map.SetInfluence();
        }
           
    }

    public void ComputeInfluenceBFS(AgentUnit unit) {
        HashSet<Node> pending = new HashSet<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        Node vert = map.NodeFromPosition(unit.position);
        pending.Add(vert);
            
        // BFS for assigning influence
        for (int i = 1; i <= RadiusInfluenceMap; i++) {
            HashSet<Node> frontier = new HashSet<Node>();
            foreach (Node p in pending) {
                if (visited.Contains(p))
                    continue;
                visited.Add(p);
                p.SetInfluence(unit.faction, unit.GetDropOff(i));
                frontier.UnionWith(map.GetDirectNeighbours(p));
            }
            pending = new HashSet<Node>(frontier);
        }
    }

    public void ComputeInfluenceDijkstra(AgentUnit unit) {

        Node startNode = map.NodeFromPosition(unit.position);
        startNode.gCost = 1;
        Heap<Node> openSet = new Heap<Node>(map.GetMaxSize());
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet.Pop();
            closedSet.Add(currentNode);
            currentNode.SetInfluence(unit.faction, unit.GetDropOff(currentNode.gCost));

            if (closedSet.Count > NNodesInfluenceMap) {
                break;
            }

            foreach (Node neighbour in map.GetNeighbours(currentNode)) {
                if (/*!neighbour.isWalkable() || */closedSet.Contains(neighbour)) {
                    continue;
                }

                //This penaly for the terrain is based on the idea that if you move from road to forest is slower than from forest to road
                float newMovementCostToNeighbour = currentNode.gCost + PathUtil.realDist(currentNode, neighbour) * unit.Cost[neighbour.type];

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = 0;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }
    }
}

