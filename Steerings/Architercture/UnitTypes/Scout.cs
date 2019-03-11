using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : AgentNPC {


    protected new void Start()
    {
        base.Start();
        MaxVelocity = 5;
        maxAccel = 30;
        MaxRotation = 70;
        MaxAngular = 70;

        cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.25f },
            { NodeT.FOREST, 1.35f },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };
    }
}
