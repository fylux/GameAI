using System.Collections;
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
    }

    public override UnitT GetUnitType() {
        return UnitT.ARTIL;
    }

    public override UnitT[] GetPreferredEnemies() {
        return new UnitT[] { UnitT.SCOUT, UnitT.RANGED, UnitT.ARTIL, UnitT.MELEE };
    }
}
