using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public bool walkable;
    public Vector3 worldPosition;
    public bool visited;

    public Node(bool walkable, Vector3 worldPosition)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.visited = false;
    }
}
