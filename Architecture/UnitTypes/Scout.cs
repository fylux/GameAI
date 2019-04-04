﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : AgentUnit
{


    protected new void Start()
    {
        base.Start();
        MaxVelocity = 5;
        MaxAccel = 30;
        MaxRotation = 70;
        MaxAngular = 70;

        cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.25f },
            { NodeT.FOREST, 1.35f },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

        atk = new Dictionary<UnitClass, float>() { //Coste por defecto, para casos de prueba
            { UnitClass.MELEE, 0.75f },
            { UnitClass.RANGED, 1.25f },
            { UnitClass.SCOUT, 1 },
            { UnitClass.ARTIL, 0.5f }
        };
    }
}
