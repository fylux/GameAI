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
        if (targetNode.isWalkable()) {
            this.targetPos = targetPos;
            targetNode = grid.NodeFromPosition(targetPos);
            hCost = new Dictionary<Node, float>();
        }
    }

    public Vector3 NextWaypoint(Vector3 currentPos) {
        currentNode = grid.NodeFromPosition(currentPos);

        //While u not in T
        if (currentNode != targetNode) {

            HashSet<Node> localSpace = genLocalSearchSpace(currentNode);
            valueUpdateStep(localSpace);

            Node minNode;
            do {
                //argmin_(a in A(u)) {w(u,a) + h(Succ(u,a))}
                grid.GetNeighbours(currentNode).ForEach(a => { if (!hCost.ContainsKey(a)) hCost[a] = hDist(a, targetNode); });
                minNode = grid.GetNeighbours(currentNode).Where(a => a.isWalkable())
                                                         .OrderByDescending(a => realDist(currentNode, a) + hCost[a])
                                                         .Last();
                //until u not in S_lss
            } while (localSpace.Contains(minNode));

            //u <- a(u)
            return minNode.worldPosition;
        }

        Debug.Log("You've reached the target");
        return targetPos;
    }

    HashSet<Node> genLocalSearchSpace(Node node) {
        HashSet<Node> space = new HashSet<Node>(grid.GetNeighbours(node));
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
                grid.GetNeighbours(node).ForEach(a => { if (!hCost.ContainsKey(a)) hCost[a] = hDist(a, targetNode); });
                Node minNeighbour = grid.GetNeighbours(node).Where(a => a.isWalkable())
                                                            .OrderByDescending(a => realDist(node, a) + hCost[a])
                                                            .Last();
                float minValue = realDist(node, minNeighbour) + hCost[minNeighbour];

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
