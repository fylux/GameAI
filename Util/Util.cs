using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {

	public static Vector3 RotateVector(Vector3 vector, float angle) {
        return Quaternion.Euler(0, angle, 0) * vector;
    }

    public static Vector3 OrientationToVector (float orientation) {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f;
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f;
        return vector.normalized;
    }

    public static float HorizontalDistance(Vector3 v1, Vector3 v2) {
        return new Vector3(v1.x - v2.x, 0, v1.z - v2.z).magnitude;
    }

    public static int NodeDistance (Node node1, Node node2)
    {
        return Mathf.Abs(node1.gridX - node2.gridX) + Mathf.Abs(node1.gridY - node2.gridY);
    }

    public static Body GetCloserBody(List<Body> bodies, Node tile)
    {
        int minDist = 100;
        Body selected = null;

        int dist;
        foreach (Body body in bodies)
        {
            if ((dist = NodeDistance(Map.NodeFromPosition(body.position),tile)) < minDist)
            {
                minDist = dist;
                selected = body;
            }
        }

        return selected;
    }

    public static Faction OppositeFaction(Faction faction) {
        if (faction == Faction.A)
            return Faction.B;
        if (faction == Faction.B)
            return Faction.A;
        return Faction.C;
    }
}
