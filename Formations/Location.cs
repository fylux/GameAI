using UnityEngine;
using System.Collections;

public class Location {
    public Vector3 position;
    public float orientation;

    public Location () {
        position = Vector3.zero;
        orientation = 0f;
    }

    public Location(Vector3 position, float orientation) {
        this.position = position;
        this.orientation = orientation;
    }

   /* void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(position, 0.7f);
    }*/
}
