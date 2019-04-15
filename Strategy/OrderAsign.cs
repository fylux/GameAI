﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrderAsign : MonoBehaviour {

    public HashSet<AgentUnit> usableUnits;

    protected InfoManager info;

    protected Faction faction = Faction.A;

    abstract public void ApplyStrategy();
}