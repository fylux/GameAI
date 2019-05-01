﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SchedulerStrategy {

    public HashSet<AgentUnit> usableUnits = new HashSet<AgentUnit>();

    protected Faction allyFaction = Faction.A;
    protected Faction enemyFaction = Faction.B;
    protected Node allyBase, enemyBase;

    abstract public void ApplyStrategy();

    public void Initialize(Faction allyFaction) {
        this.allyFaction = allyFaction;
        this.enemyFaction = Util.OppositeFaction(allyFaction);
        
        allyBase = InfoManager.GetWaypoint("base", allyFaction);
        enemyBase = InfoManager.GetWaypoint("base", enemyFaction);

        Debug.Log("Actualizando el valor de las bases de " + this + " y son allyBase: " + allyBase + " y enemyBase: " + enemyBase);
    }
}
