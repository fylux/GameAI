using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : AgentNPC {

   Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.25f },
            { NodeT.FOREST, 1.35f },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
    };
}
