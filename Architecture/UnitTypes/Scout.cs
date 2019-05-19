using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : AgentUnit {

    protected new void Start() {
        base.Start();
		MaxVelocity = 2.3f;
        militar.attack = 3;
        militar.attackRange = 1.5f;
        militar.attackSpeed = 1.25f;
        militar.defense = 0;
        militar.health = 12;
        militar.maxHealth = 12;

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
