using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : AgentUnit {

    protected new void Start() {
        base.Start();
		MaxVelocity = 2.3f;
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

    }

    public override UnitT GetUnitType() {
        return UnitT.SCOUT;
    }

    public override UnitT[] GetPreferredEnemies() {
        return new UnitT[] { UnitT.RANGED, UnitT.SCOUT, UnitT.MELEE, UnitT.ARTIL };
    }
}
