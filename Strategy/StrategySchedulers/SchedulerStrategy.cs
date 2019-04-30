using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SchedulerStrategy {

    public HashSet<AgentUnit> usableUnits = new HashSet<AgentUnit>();

    protected InfoManager info;

    protected Faction faction = Faction.A;

    abstract public void ApplyStrategy();

    protected Node allyBase, enemyBase; 

    public void Initialize(Node allyB, Node enemyB)
    {
        Debug.Log("Actualizando el valor de las bases de " + this + " y son allyBase: " + allyB + " y enemyBase: " + enemyB);
        allyBase = allyB;
        enemyBase = enemyB;
        info = InfoManager.instance;
    }
}
