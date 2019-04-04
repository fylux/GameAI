﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : AgentUnit
{

    protected new void Start()
    {
        base.Start();
        MaxVelocity = 3;
        MaxAccel = 10;
        MaxRotation = 30;
        MaxAngular = 30;

        cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 6 },
            { NodeT.FOREST, 10 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

        atk = new Dictionary<UnitClass, float>() { //Coste por defecto, para casos de prueba
            { UnitClass.MELEE, 0.8f },
            { UnitClass.RANGED, 1.25f },
            { UnitClass.SCOUT, 1.5f },
            { UnitClass.ARTIL, 1 }
        };
    }
}
