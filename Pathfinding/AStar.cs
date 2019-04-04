using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class AStar : Pathfinding {

    Node startNode, targetNode;
    Vector3 targetPos;
    Dictionary<Node, Node> prev;
    PathfindingManager requestManager;


    // AStar() : base() {}

    public IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, Dictionary<NodeT, float> cost, Action<Vector3[], bool> FinishedProcessing) {
        prev = new Dictionary<Node, Node>();
        bool pathSuccess = false;

        this.targetPos = targetPos;
		startNode = Map.NodeFromPosition(startPos);
        startNode.gCost = 0;
		targetNode = Map.NodeFromPosition(targetPos);

        //Debug.Log(startNode.type + " -> " + targetNode.type);

        /*Acceleration can make a NPC move to a non accesible area so we should not take it into account when computing the path.*/

        if (/*startNode.isWalkable() && */targetNode.isWalkable()) {
			Heap<Node> openSet = new Heap<Node>(Map.GetMaxSize());
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Node currentNode = openSet.Pop();
                closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in Map.GetNeighbours(currentNode)) {
					if (!neighbour.isWalkable() || closedSet.Contains(neighbour)) {
						continue;
					}

                    //This penaly for the terrain is based on the idea that if you move from road to forest is slower than from forest to road
                    float newMovementCostToNeighbour = currentNode.gCost + PathUtil.realDist(currentNode, neighbour) * cost[neighbour.type];

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = PathUtil.hDist(neighbour, targetNode);
                        prev[neighbour] = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
				}
			}
		}
		yield return null;

        Vector3[] waypoints = pathSuccess ? RetracePath() : new Vector3[0];
        FinishedProcessing(waypoints, pathSuccess);
    }
    
    Vector3[] RetracePath() {
        List<Node> path = new List<Node>();
        //StartNode is not added since we are already on it
        for (Node currentNode = targetNode; currentNode != startNode; currentNode = prev[currentNode]) {
            path.Add(currentNode);
        }
        path.Reverse();

        List<Vector3> waypoints = PathUtil.SimplifyPath(path);
        waypoints.Add(targetPos);
		return waypoints.ToArray();
	}
}
