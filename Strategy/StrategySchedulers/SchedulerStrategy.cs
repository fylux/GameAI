using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SchedulerStrategy {

    public HashSet<AgentUnit> usableUnits = new HashSet<AgentUnit>();

    protected InfoManager info;

    protected Faction faction = Faction.A;

    abstract public void ApplyStrategy();

    protected Node allyBase, enemyBase; 

    public void SetBases(Node allyB, Node enemyB)
    {
        allyBase = allyB;
        enemyBase = enemyB;
    }
}
