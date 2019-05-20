using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap : MonoBehaviour {


    const int NNodesInfluenceMap = 1000;
    const int RadiusInfluenceMap = 15;
    const int SecondsPerInfluenceUpdate = 1;

    private void Start() {
       
    }

    public void Update() {
        if (Mathf.Floor(Time.fixedTime * 1000) % (1000 * SecondsPerInfluenceUpdate) == 0) { //Time is managed in ms
            Map.ResetInfluence();
            foreach (AgentUnit unit in Map.unitList) {
                ComputeInfluenceDijkstra(unit, Map.generalInfluence);
                //ComputeInfluenceDijkstraRanged(unit, Map.generalInfluence);
                ComputeInfluenceBFS(unit, Map.clusterInfluence);
            }
            Map.DrawInfluence();
        }
           
    }

    /*public IEnumerator UpdateInfluence() {
        map.ResetInfluence();
        yield return null;

        foreach (AgentUnit unit in unitList) {
            ComputeInfluenceDijkstra(unit);
            yield return null;
        }
        map.SetInfluence();
        //Call update minimap camera
    }*/

    public void ComputeInfluenceBFS(AgentUnit unit, Vector2[,] influenceMap) {
        HashSet<Node> pending = new HashSet<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        Node vert = Map.NodeFromPosition(unit.position);
        pending.Add(vert);
            
        // BFS for assigning influence
        for (int i = 1; i <= RadiusInfluenceMap; i++) {
            HashSet<Node> frontier = new HashSet<Node>();
            foreach (Node p in pending) {
                if (visited.Contains(p))
                    continue;
                visited.Add(p);
                p.SetInfluence(unit.faction, unit.GetDropOff(Util.HorizontalDist(p.worldPosition, unit.position)), influenceMap, InfluenceT.MAXIMUM);
                frontier.UnionWith(Map.GetDirectNeighbours(p));
            }
            pending = new HashSet<Node>(frontier);
        }
    }

    public void ComputeInfluenceDijkstra(AgentUnit unit, Vector2[,] influenceMap) {
        Node startNode = Map.NodeFromPosition(unit.position);
        startNode.gCost = 1;
        Heap<Node> openSet = new Heap<Node>(Map.GetMaxSize());
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet.Pop();
            closedSet.Add(currentNode);
            currentNode.SetInfluence(unit.faction, unit.GetDropOff(currentNode.gCost), influenceMap, InfluenceT.ACCUMULATE);

            if (closedSet.Count > NNodesInfluenceMap) {
                break;
            }

            foreach (Node neighbour in Map.GetNeighbours(currentNode)) {
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

