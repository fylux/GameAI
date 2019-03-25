using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;



public class LRTA : Pathfinding {

    Dictionary<Node, float> hCost;

    Node currentNode, targetNode;
    Vector3 targetPos;

    public void StartPath(Vector3 targetPos) {
        this.targetPos = targetPos;
        Debug.Log(map);
        targetNode = map.NodeFromPosition(targetPos);
        hCost = new Dictionary<Node, float>();
    }

    public Vector3 NextWaypoint(Vector3 currentPos) {
        currentNode = map.NodeFromPosition(currentPos);

        //While u not in T
        if (currentNode != targetNode) {

            HashSet<Node> localSpace = genLocalSearchSpace(currentNode);
            valueUpdateStep(localSpace);

            Node minNode;
            do {
                //argmin_(a in A(u)) {w(u,a) + h(Succ(u,a))}
                map.GetNeighbours(currentNode).ForEach(a => { if (!hCost.ContainsKey(a)) hCost[a] = PathUtil.hDist(a, targetNode); });
                minNode = map.GetNeighbours(currentNode).Where(a => a.isWalkable())
                                                         .OrderByDescending(a => PathUtil.realDist(currentNode, a) + hCost[a])
                                                         .Last();
                //until u not in S_lss
            } while (localSpace.Contains(minNode));

            //u <- a(u)
            return minNode.worldPosition;
        }

        Debug.Log("You've reached the target");
        return targetPos;
    }

    public Vector3[] FindPath(Vector3 currentPos) {
        currentNode = map.NodeFromPosition(currentPos);
        List<Node> path = new List<Node>();

        //While u not in T
        while (currentNode != targetNode && path.Count < 15) {
            HashSet<Node> localSpace = genLocalSearchSpace(currentNode);
            valueUpdateStep(localSpace);

            Node minNode;
            do {
                //argmin_(a in A(u)) {w(u,a) + h(Succ(u,a))}
                map.GetNeighbours(currentNode).ForEach(a => { if (!hCost.ContainsKey(a)) hCost[a] = PathUtil.hDist(a, targetNode); });
                minNode = map.GetNeighbours(currentNode).Where(a => a.isWalkable())
                                                         .OrderByDescending(a => PathUtil.realDist(currentNode, a) + hCost[a])
                                                         .Last();
                //u <- a(u)
                path.Add(minNode);
                currentNode = minNode;

            //until u not in S_lss
            } while (localSpace.Contains(minNode) && minNode != targetNode && path.Count < 15);
        }

        bool reachTarget = (currentNode == targetNode);
        List<Vector3> waypoints = PathUtil.SimplifyPath(PathUtil.RemoveCycles(path), !reachTarget);
        if (reachTarget) waypoints.Add(targetPos);

        return waypoints.ToArray();
    }

    HashSet<Node> genLocalSearchSpace(Node node) {
        HashSet<Node> space = new HashSet<Node>(map.GetNeighbours(node));
        space.Add(node);
        return space;
    }

    void valueUpdateStep(HashSet<Node> nodes) {
        /*S_lss = Local Search Space
         A = Actions (In this problem the action is moving to a Node)
         w(u,a) = Cost from u to a (Distance)
         h(Succ(u,a)) = Cost from u to target (Distance)
         */

        Dictionary<Node,float> temp = new Dictionary<Node, float>();

        //for each u in S_lss
        foreach (Node node in nodes) {
            if (!hCost.ContainsKey(node)) hCost[node] = PathUtil.hDist(node, targetNode);

            //temp(u) = h(u)
            temp.Add(node, hCost[node]);

            //h(u) = inf
            hCost[node] = Mathf.Infinity;    
        }

        //while (exists u in S_lss where h(u) is inf)
        while (nodes.Any(n => hCost[n] == Mathf.Infinity)) { 
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
                if (hCost[node] != Mathf.Infinity) continue;

                //min_(a in A) { w(u,a) + h(Succ(u,a)) }
                map.GetNeighbours(node).ForEach(a => { if (!hCost.ContainsKey(a)) hCost[a] = PathUtil.hDist(a, targetNode); });
                Node minNeighbour = map.GetNeighbours(node).Where(a => a.isWalkable())
                                                            .OrderByDescending(a => PathUtil.realDist(node, a) + hCost[a])
                                                            .Last();
                float minValue = PathUtil.realDist(node, minNeighbour) + hCost[minNeighbour];

                //max(temp(u), _ );
                float result = Mathf.Max(temp[node], minValue);

                if (result < minV) {
                    minV = result;
                    v = node;
                }
            }

            //h(v) = max(temp(u), min_(a in A) { w(u,a) + h(Succ(u,a)) } )
            hCost[v] = minV;

            //if h(v) == inf : return
            if (hCost[v] == Mathf.Infinity) return;
        }
    }

}
