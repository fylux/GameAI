using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public interface Task {
    Steering Apply();
    void Terminate();
}

public abstract class BaseTask : Task {
    public abstract Steering Apply();
    protected abstract bool IsFinished();

    protected AgentUnit agent;
    protected Action<bool> callback;

    public BaseTask(AgentUnit agent, Action<bool> callback) {
        this.agent = agent;
        this.callback = callback;
    }

    virtual public void Terminate() {}
}
