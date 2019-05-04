using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LRTA  {

    const int lookahead = 7;

    Dictionary<Node, float> hCost;
    Dictionary<NodeT, float> cost;

    Node currentNode, targetNode;
    Vector3 targetPos;

    public void StartPath(Vector3 targetPos, Dictionary<NodeT, float> cost) {
        this.targetPos = targetPos;
        this.cost = cost;

        targetNode = Map.NodeFromPosition(targetPos);
        hCost = new Dictionary<Node, float>();
    }

    public Vector3[] FindPath(Vector3 currentPos) {
        currentNode = Map.NodeFromPosition(currentPos);
        List<Node> path = new List<Node>();

        //While u not in T
        while (currentNode != targetNode && path.Count < lookahead) {
            HashSet<Node> localSpace = genLocalSearchSpace(currentNode);
            valueUpdateStep(localSpace);

            Node minNode;
            do {
                //argmin_(a in A(u)) {w(u,a) + h(Succ(u,a))}
                Map.GetNeighbours(currentNode).ForEach(a => { if (!hCost.ContainsKey(a)) hCost[a] = PathUtil.hDist(a, targetNode); });
                minNode = Map.GetNeighbours(currentNode).Where(a => a.isWalkable())
                                                         .OrderByDescending(a => PathUtil.realDist(currentNode, a) * cost[a.type] + hCost[a])
                                                         .Last();
                //u <- a(u)
                path.Add(minNode);
                currentNode = minNode;

            //until u not in S_lss
            } while (localSpace.Contains(minNode) && minNode != targetNode && path.Count < lookahead);
        }

        bool reachTarget = (currentNode == targetNode);
        List<Vector3> waypoints = PathUtil.SimplifyPath(PathUtil.RemoveCycles(path), !reachTarget);
        if (reachTarget) waypoints.Add(targetPos);

        return waypoints.ToArray();
    }

    HashSet<Node> genLocalSearchSpace(Node node) {
        HashSet<Node> space = new HashSet<Node>(Map.GetNeighbours(node));
        space.Add(node); //with u in S_lss
        space.Remove(targetNode);//and T & S_lss = 0
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
                Map.GetNeighbours(node).ForEach(a => { if (!hCost.ContainsKey(a)) hCost[a] = PathUtil.hDist(a, targetNode); });
                Node minNeighbour = Map.GetNeighbours(node).OrderByDescending(a => PathUtil.realDist(node, a) * cost[a.type] + hCost[a]).Last();
                float minValue = PathUtil.realDist(node, minNeighbour) * cost[minNeighbour.type] + hCost[minNeighbour];

                //max(temp(u), _ );
                float result = Mathf.Max(temp[node], minValue);

                if (result < minV) {
                    minV = result;
                    v = node;
                }
            }

            //h(v) = max(temp(u), min_(a in A) { w(u,a) + h(Succ(u,a)) } )
            hCost[v] = minV;
            Console.Log(v.ToString() + " = " + minV);

            //if h(v) == inf : return
            if (hCost[v] == Mathf.Infinity) return;
        }
    }

}
