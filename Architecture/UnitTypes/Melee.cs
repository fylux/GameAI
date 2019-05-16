using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : AgentUnit
{

    protected new void Start() {
        base.Start();
		MaxVelocity = 1.8f;
        MaxAccel = 20;
        MaxRotation = 60;
        MaxAngular = 60;
        attackRange = 1.5f;

        cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };
    }

    public override UnitT GetUnitType() {
        return UnitT.MELEE;
    }

    public override UnitT[] GetPreferredEnemies() {
        return new UnitT[] { UnitT.SCOUT, UnitT.RANGED, UnitT.ARTIL, UnitT.MELEE };
    }
}
