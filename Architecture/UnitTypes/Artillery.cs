using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : AgentUnit
{

    protected new void Start()
    {
        base.Start();
		MaxVelocity = 1.5f;
        militar.attack = 4;
        militar.attackRange = 6;
        militar.attackSpeed = 0.5f;
        militar.defense = 1;
        militar.health = 10;
        militar.maxHealth = 10;

        cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 2.5f },
            { NodeT.FOREST, 6 },
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
