using UnityEngine;
using System.Collections;

public enum NodeT {
    ROAD, GRASS, FOREST, WATER, MOUNTAIN
};

public enum Faction {
    A, B, C
}

public enum InfluenceT {
    ACCUMULATE, MAXIMUM
}


public class Node : IHeapItem<Node> {

    public GameObject gameObject;

    public NodeT type;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public float gCost;
	public float hCost;

    protected Vector2 influence;
    public Renderer influenceTile;

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


    //Influence
    public void SetInfluence(Faction f, float v, Vector2[,] influenceMap, InfluenceT type) {
        if (type == InfluenceT.ACCUMULATE)
            influenceMap[gridX, gridY][(int)f] += v;
        else if (type == InfluenceT.MAXIMUM)
            influenceMap[gridX, gridY][(int)f] = Mathf.Max(influenceMap[gridX, gridY][(int)f], v);

        //100% is the maximun influence
        influenceMap[gridX, gridY][(int)f] = Mathf.Min(100, influenceMap[gridX, gridY][(int)f]);
    }

    public float GetRawInfluence(Faction f, Vector2[,] influenceMap) {
        return influenceMap[gridX, gridY][(int)f] / 100f;
    }

    public float GetNetInfluence(Faction f, Vector2[,] influenceMap) {
        return (influenceMap[gridX, gridY][(int)f] - influenceMap[gridX, gridY][1-(int)f]) / 100f;
    }

    public Faction GetMostInfluentFaction(Vector2[,] influenceMap) {
        if (influenceMap[gridX, gridY][(int)Faction.A] >= influenceMap[gridX, gridY][(int)Faction.B])
            return Faction.A;
        else
            return Faction.B;
    }


    override
    public string ToString() {
        return "("+gridX + "," + gridY+")";
    }

}
