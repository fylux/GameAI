using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;




public class Pathfinding : MonoBehaviour {

    protected PathfindingManager requestManager;
    protected Map grid;
    protected enum DistanceT {
        EUCLIDEAN, MANHATTAN, CHEBYSHEV
    };

    void Awake() {
        requestManager = GetComponent<PathfindingManager>();
        grid = GetComponent<Map>();
    }
	
    //Simplfies path and does not add the targetNode, since the last position is no the center of targetNode but targetPos
	protected List<Vector3> SimplifyPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 oldDirection = Vector2.zero;
		
		for (int i = 1; i < path.Count; i++) {
			Vector2 newDirection = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
			if (newDirection != oldDirection) {
				waypoints.Add(path[i-1].worldPosition);
			}
			oldDirection = newDirection;
		}

        return waypoints;
	}
	
	float distance(Node nodeA, Node nodeB, DistanceT distanceT = DistanceT.MANHATTAN) {
        Vector2 p = new Vector2(nodeA.gridX, nodeA.gridY);
        Vector2 q = new Vector2(nodeB.gridX, nodeB.gridY);
        //Distance (D) is always 1 in our map
        switch (distanceT) {
            case DistanceT.EUCLIDEAN:
                return Vector2.Distance(p,q);
            case DistanceT.MANHATTAN:
                return Mathf.Abs(p.x - q.x) + Mathf.Abs(p.y - q.y);
            case DistanceT.CHEBYSHEV:
                //return Mathf.Max(Mathf.Abs(p.x - q.x), Mathf.Abs(p.y - q.y)); //Basic Chebychev
                float h_diagonal = Mathf.Min(Mathf.Abs(p.x - q.x), Mathf.Abs(p.y - q.y));
                float h_straigth = Mathf.Abs(p.x - q.x) + Mathf.Abs(p.y - q.y);
                return Mathf.Sqrt(2f) * h_diagonal + (h_straigth - 2 * h_diagonal);
        }
        return 0f;
    }

    protected float hDist(Node nodeA, Node nodeB, DistanceT distanceT = DistanceT.MANHATTAN) {
        return distance(nodeA, nodeB, distanceT);
    }

    protected float realDist(Node nodeA, Node nodeB) {
        return distance(nodeA, nodeB, DistanceT.EUCLIDEAN);
    }



}
