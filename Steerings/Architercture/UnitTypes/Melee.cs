﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : AgentNPC {


    protected new void Start()
    {
        base.Start();
        MaxVelocity = 4;
        maxAccel = 20;
        MaxRotation = 60;
        MaxAngular = 60;

        cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };
    }
}
