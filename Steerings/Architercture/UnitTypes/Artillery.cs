using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : AgentNPC {

   Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 6 },
            { NodeT.FOREST, 10 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
    };

    protected new void Start()
    {
        base.Start();
        MaxVelocity = 3;
        maxAccel = 10;
        MaxRotation = 30;
        MaxAngular = 30;
    }
}
