using UnityEngine;
using System.Collections;

public enum NodeT {
    ROAD, GRASS, FOREST, WATER, MOUNTAIN
};

public enum Faction {
    A, B, C
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
    public GameObject influenceTile;

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
    public void SetInfluence(Faction f, float v) {
        influence[(int)f] += v;   //Incremental influence
        //influence[(int)f] = Mathf.Max(influence[(int)f],v); //Flooding, max influence
    }

    public float getInfluence() {
        return Mathf.Min(100f, Mathf.Abs(influence[0]-influence[1])); //Influence of one faction removes influence of another
        //return Mathf.Min(100f, Mathf.Max(influence[0], influence[1])); //Influence of one faction does not affect the other
    }

    public void ResetInfluence() {
        //influence -= new Vector2(Mathf.Max(0.1f, influence[0] * 0.2f), Mathf.Max(0.1f, influence[1] * 0.2f));
        influence = Vector2.zero;
    }

    public Faction getFaction() {
        if (getInfluence() < 1f)//(influence[0] < 0.1f && influence[1] < 0.1f)
            return Faction.C;
        else if (influence[0] > influence[1])
            return Faction.A;
        else
            return Faction.B;
    }

    override
    public string ToString() {
        return "("+gridX + "," + gridY+")";
    }

}
