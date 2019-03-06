using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

enum DistanceT {
    EUCLIDEAN, MANHATTAN, CHEBYSHEV
};


public class Pathfinding : MonoBehaviour {

    PathfindingManager requestManager;
    Map grid;

    Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f},
            { NodeT.FOREST, 2.5f},
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

    void Awake() {
        requestManager = GetComponent<PathfindingManager>();
        grid = GetComponent<Map>();
    }

    public IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        bool pathSuccess = false;
		
		Node startNode = grid.NodeFromPosition(startPos);
		Node targetNode = grid.NodeFromPosition(targetPos);

        Debug.Log(startNode.type + " -> " + targetNode.type);

        /*Acceleration can make a NPC move to a non accesible area so we should not take it into account when computing the path.*/

        if (/*startNode.isWalkable() && */targetNode.isWalkable()) {
			Heap<Node> openSet = new Heap<Node>(grid.GetMaxSize());
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Node currentNode = openSet.Pop();
                closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.isWalkable() || closedSet.Contains(neighbour)) {
						continue;
					}

                    //This penaly for the terrain is based on the idea that if you move from road to forest is slower than from forest to road
                    float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour, DistanceT.MANHATTAN) * cost[neighbour.type];

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode, DistanceT.MANHATTAN);
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

        Vector3[] waypoints = pathSuccess ? RetracePath(prev, startNode, targetNode) : new Vector3[0];
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    public IEnumerator FindPathLRTA(Vector3 startPos, Vector3 targetPos) {
        List<Node> path = new List<Node>();
        bool pathSuccess = false;

        Node startNode = grid.NodeFromPosition(startPos);
        Node targetNode = grid.NodeFromPosition(targetPos);

        Debug.Log(startNode.type + " -> " + targetNode.type);

        if (targetNode.isWalkable()) {
            Node currentNode = startNode;

            while (true) {
                if (currentNode == targetNode) {
                    pathSuccess = true;
                    break;
                }

                float minF = Mathf.Infinity;
                Node minNode = null;
                foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                    if (!neighbour.isWalkable() ) {
                        continue;
                    }

                    float f;
                    if (neighbour.first) { //TODO Fix
                        f = GetDistance(currentNode, neighbour, DistanceT.MANHATTAN) + GetDistance(neighbour, targetNode, DistanceT.MANHATTAN);
                        neighbour.first = false;
                    } else {
                        f = GetDistance(currentNode, neighbour, DistanceT.MANHATTAN) + neighbour.hCost;
                    }
                    if (f < minF) {
                        minF = f;
                        minNode = neighbour;
                    }
                }
                if (minNode == null) {
                    Debug.LogError("Callejon sin salida");
                }
                currentNode.hCost = minF;

                currentNode = minNode;
                path.Add(currentNode);
            }
        }
        yield return null;

        Vector3[] waypoints = pathSuccess ? SimplifyPath(path) : new Vector3[0];
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    //TODO Right now the Local Search Space is only the neigbourhood
    List<Node> genLocalSearchSpace(Node node) {
        List<Node> space = new List<Node>();

        space = grid.GetNeighbours(node);

        return space;
    }

    void valueUpdateStep(List<Node> nodes, Node targetNode) {
        /*S_lss = Local Search Space
         A = Actions (In this problem the action is moving to a Node)
         w(u,a) = Cost from u to a (Distance)
         h(Succ(u,a)) = Cost from u to target (Distance)
         */

        Dictionary<Node,float> temp = new Dictionary<Node, float>();

        //for each u in S_lss
        foreach (Node node in nodes) {
            //temp(u) = h(u)
            temp.Add(node, node.hCost);

            //h(u) = inf
            node.hCost = Mathf.Infinity;    
        }

        //while (exists u in S_lss where h(u) is inf)
        while (nodes.Any(n => n.hCost == Mathf.Infinity)) { 
            /*v = argmin_(u in S_lss | where h(u) = inf)
            {max(
                temp(u),
                min_(a in Actions) { 
                    w(u,a) + h(Succ(u,a)) 
                }
            )}*/
            float minV = Mathf.Infinity;
            Node v = null;
            foreach (Node node in nodes) {
                //... where h(u) = inf ...
                if (node.hCost != Mathf.Infinity) continue;

                //min_(a in A) { w(u,a) + h(Succ(u,a)) }
                Node minNeighbour = grid.GetNeighbours(node).OrderByDescending(a => GetDistance(node, a) + GetDistance(a, targetNode)).Last();
                float minValue = GetDistance(node, minNeighbour) + GetDistance(minNeighbour, targetNode);

                //max(temp(u), _ );
                float result = Mathf.Max(temp[node], minValue);

                if (result < minV) {
                    minV = result;
                    v = node;
                }
            }

            //h(v) = max(temp(u), min_(a in A) { w(u,a) + h(Succ(u,a)) } )
            v.hCost = minV;

            //if h(v) == inf : return
            if (v.hCost == Mathf.Infinity) return;
        }
    }

    Vector3[] RetracePath(Dictionary<Node,Node> prev, Node startNode, Node targetNode) {
        List<Node> path = new List<Node>();
        for (Node currentNode = targetNode; currentNode != startNode; currentNode = prev[currentNode]) {
            path.Add(currentNode);
        }
        path.Reverse();

        Vector3[] waypoints = SimplifyPath(path);
		return waypoints;
	}
	
	Vector3[] SimplifyPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 oldDirection = Vector2.zero;
		
		for (int i = 1; i < path.Count; i++) {
			Vector2 newDirection = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
			if (newDirection != oldDirection) {
				waypoints.Add(path[i-1].worldPosition);
			}
			oldDirection = newDirection;
		}
        waypoints.Add(path[path.Count - 1].worldPosition);

        return waypoints.ToArray();
	}
	
	float GetDistance(Node nodeA, Node nodeB, DistanceT distanceT = DistanceT.MANHATTAN) {
        Vector2 p = new Vector2(nodeA.gridX, nodeA.gridY);
        Vector2 q = new Vector2(nodeB.gridX, nodeB.gridY);
        switch (distanceT) {
            case DistanceT.EUCLIDEAN:
                return Vector2.Distance(p,q);
            case DistanceT.MANHATTAN:
                return Mathf.Abs(p.x - q.x) + Mathf.Abs(p.y - q.y);
            case DistanceT.CHEBYSHEV:
                return Mathf.Max(Mathf.Abs(p.x - q.x), Mathf.Abs(p.y - q.y));
        }
        return 0f;
    }
	
	
}
