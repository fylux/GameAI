using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrderAsign {

    public HashSet<AgentUnit> usableUnits = new HashSet<AgentUnit>();

    protected InfoManager info;

    protected Faction faction = Faction.A;

    abstract public void ApplyStrategy();
}
