using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public Transform player, target;
    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        findPath(player.position,target.position);
    }

    void findPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; ++i) {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    currentNode = openSet[i];
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                retracePath(startNode, targetNode);
                return;
            }
                

            foreach (Node neighbour in grid.getNeighbours(currentNode)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovCostTo = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovCostTo < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovCostTo;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void retracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            Debug.Log("target" + path.Count);
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        
        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstY < dstX)
            return 14 * dstY + 10 * (dstX - dstY);
        else
            return 14 * dstX + 10 * (dstY - dstX);
    }
}
