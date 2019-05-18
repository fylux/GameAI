using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : AgentUnit {

    protected new void Start()
    {
        base.Start();
        // Los atributos de los Ranged son con los que hemos estado trabajando, los default
        MaxVelocity = 1.8f;
        militar.attack = 2;
        militar.attackRange = 4;
        militar.attackSpeed = 1f;
        militar.defense = 0;
        militar.health = 8;
        militar.maxHealth = 8;

        cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.75f },
            { NodeT.FOREST, 2.5f },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

    }

    public override UnitT GetUnitType() {
        return UnitT.RANGED;
    }

    public override UnitT[] GetPreferredEnemies() {
        return new UnitT[] { UnitT.SCOUT, UnitT.MELEE, UnitT.RANGED, UnitT.ARTIL };
    }
}
