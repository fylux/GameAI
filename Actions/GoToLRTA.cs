using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoToLRTA : BaseTask {

    GameObject empty;
    PathFollowing pathF;
    Vector3 target;
    LRTA lrta;


    public GoToLRTA(AgentUnit agent, Vector3 target, Action<bool> callback) : base(agent, callback) {
        this.target = target;

        empty = new GameObject();
        empty.transform.parent = agent.gameObject.transform;

        pathF = empty.AddComponent<PathFollowing>();
        pathF.SetNPC(agent);
        pathF.path = null;
        pathF.visibleRays = true;

        lrta = new LRTA();
        lrta.StartPath(target);
        RequestPath();
    }

    private void RequestPath() {
        pathF.SetPath(lrta.FindPath(agent.position));
    }

    override
    public Steering Apply() {
        if (IsFinished()) callback(true);

        if (Time.frameCount % 1 == 0)
            RequestPath();

        if (pathF.path != null)
            return pathF.GetSteering();
        else
            return Seek.GetSteering(target, agent, 10f); //If path has not been solved yet just do Seek.
    }

    override
    protected bool IsFinished() {
        return Util.HorizontalDistance(agent.position, target) < 0.3f;
    }

    override
    public void Terminate() {
        UnityEngine.Object.Destroy(empty);
    }
}
