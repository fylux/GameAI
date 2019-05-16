using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SchedulerStrategy {

    public HashSet<AgentUnit> usableUnits = new HashSet<AgentUnit>();

    protected Faction allyFaction = Faction.A;
    protected Faction enemyFaction = Faction.B;
    protected Vector3 allyBase, enemyBase;

    abstract public void ApplyStrategy();

    public void Initialize(Faction allyFaction) {
        this.allyFaction = allyFaction;
        this.enemyFaction = Util.OppositeFaction(allyFaction);
        
        allyBase = Info.GetWaypoint("base", allyFaction);
        enemyBase = Info.GetWaypoint("base", enemyFaction);
    }

    public virtual void Reset() { usableUnits = new HashSet<AgentUnit>(); }
	public virtual void RemoveUnit(AgentUnit unit) {
		usableUnits.Remove (unit);
	} 
}
