using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public abstract class Task {
    public abstract Steering Apply();
    protected abstract bool IsFinished();

    protected AgentUnit agent;
    protected Action<bool> callback;

    public Task(AgentUnit agent, Action<bool> callback) {
        this.agent = agent;
        this.callback = callback;
    }

    virtual public void Terminate() {}

    public void SetCallback(Action<bool> callback) {
        this.callback = callback;
    }
    public Action<bool> GetCallback() {
        return callback;
    }
}
