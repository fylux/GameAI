using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : AgentNPC {

   Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
    };
}
