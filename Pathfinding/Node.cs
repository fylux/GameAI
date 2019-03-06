using UnityEngine;
using System.Collections;

public enum NodeT {
    ROAD, GRASS, FOREST, WATER, MOUNTAIN
};

public class Node : IHeapItem<Node> {
	
	public NodeT type;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public float gCost;
	public float hCost;
	public Node prev;
    public bool first = true;
	int heapIndex;

    public float fCost {
        get { return gCost + hCost; }
    }

    public int HeapIndex {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public Node(int gridX, int gridY, Vector3 worldPos, NodeT type) {
		this.type = type;
        this.worldPosition = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
	}


	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}

    public bool isWalkable() {
        return (type != NodeT.WATER) && (type != NodeT.MOUNTAIN);
    }
}
