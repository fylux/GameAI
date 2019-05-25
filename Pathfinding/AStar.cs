using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class AStar {

    Node startNode, targetNode;
    Vector3 targetPos;
    Dictionary<Node, Node> prev;
    PathfindingManager requestManager;


    public IEnumerator FindPath(PathRequest request, Action<Vector3[], bool> finishedPath) {
        prev = new Dictionary<Node, Node>();
        bool pathSuccess = false;

        Faction faction = request.faction;
        AgentNPC unit = request.unit;
		startNode = Map.NodeFromPosition(request.start);
        startNode.gCost = 0;
        targetPos = request.end;
        targetNode = Map.NodeFromPosition(targetPos);

        /*Acceleration can make a NPC move to a non accesible area so we should not take it into account when computing the path.*/

        if (/*startNode.isWalkable() && */targetNode.isWalkable()) {
			Heap<Node> openSet = new Heap<Node>(Map.GetMaxSize());
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);

            HashSet<Node> toReset = new HashSet<Node>();
			
			while (openSet.Count > 0) {
				Node currentNode = openSet.Pop();
                closedSet.Add(currentNode);
                toReset.Add(currentNode);
				
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in Map.GetNeighbours(currentNode)) {
					if (!neighbour.isWalkable() || closedSet.Contains(neighbour)) continue;

                    float r = 0;
                    if (faction != Faction.C) {
                        float z;
                        if (unit is AgentUnit && ((AgentUnit) unit).GetUnitType() != UnitT.RANGED)
                            z = neighbour.GetRawInfluence(Util.OppositeFaction(faction), Map.clusterInfluence);
                        else
                            z = neighbour.GetRawInfluence(Util.OppositeFaction(faction), Map.generalRangedInfluence);

                        if (z > 2/4f) r = 4;
                        else if (z > 2/ 8f) r = 1;
                    }
                    


                    //This penaly for the terrain is based on the idea that if you move from road to forest is slower than from forest to road
                    float newMovementCostToNeighbour = currentNode.gCost + r  + PathUtil.realDist(currentNode, neighbour) * request.cost[neighbour.type];


                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        toReset.Add(neighbour);

                        neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = PathUtil.hDist(neighbour, targetNode);
                        prev[neighbour] = currentNode;

                        if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                        else openSet.UpdateItem(neighbour);
                    }
				}
            }
            foreach (var node in toReset) {
                node.gCost = 0;
                node.hCost = 0;
            }

        }
        yield return null;

        
        List<Vector3> waypoints = new List<Vector3>();
        List<Node> nodesPath = new List<Node>();
        if (pathSuccess) {
            nodesPath = RetracePath();
            waypoints = new List<Vector3>(nodesPath.Select(n => n.worldPosition));//PathUtil.SimplifyPath(nodesPath);
            //To prevent units from going through the exact same path
            float offsetX = UnityEngine.Random.Range(-0.5f, 0.5f);
            float offsetY = UnityEngine.Random.Range(-0.5f, 0.5f);
            for (int i = 0; i < waypoints.Count; ++i) {
                waypoints[i] += new Vector3(offsetX, 0, offsetY);
            }

            waypoints.Add(targetPos);
        }
        finishedPath(waypoints.ToArray(), pathSuccess);
    }
    
   List<Node> RetracePath() {
        List<Node> path = new List<Node>();
        //StartNode is not added since we are already on it
        for (Node currentNode = targetNode; currentNode != startNode; currentNode = prev[currentNode]) {
            path.Add(currentNode);
        }
        path.Reverse();

        return path;
	}
}
