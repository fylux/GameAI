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
    public abstract void Terminate();
    protected abstract bool IsFinished();

    protected AgentUnit agent;
    protected Action<bool> callback;

    public BaseTask(AgentUnit agent, Action<bool> callback) {
        this.agent = agent;
        this.callback = callback;
    }
}

public abstract class ComplexTask : BaseTask {
    protected List<BaseTask> subTasks;

    public ComplexTask(AgentUnit agent, Action<bool> callback) : base(agent,callback) {
        subTasks = new List<BaseTask>();
    }

    override
    public void Terminate() {
        foreach (BaseTask task in subTasks) {
            task.Terminate();
        }
    }
}