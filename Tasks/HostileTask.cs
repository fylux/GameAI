using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HostileTask : Task {

    public HostileTask(AgentUnit agent, Action<bool> callback) : base(agent, callback) {}

    public abstract void ReceiveAttack(AgentUnit enemy);
}
