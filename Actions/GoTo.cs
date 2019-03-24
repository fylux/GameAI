using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoTo : BaseTask {

    GameObject empty;
    PathFollowing pathF;
    Vector3 target;


    public GoTo(AgentUnit agent, Vector3 target, Action<bool> callback) : base(agent,callback) {
        this.target = target;

        empty = new GameObject();
        empty.transform.parent = agent.gameObject.transform;

        pathF = empty.AddComponent<PathFollowing>();
        pathF.SetNPC(agent);
        pathF.path = null;
        pathF.visibleRays = true;

        PathfindingManager.RequestPath(agent.position, target, agent.Cost, ProcessPath);
    }

    private void ProcessPath(Vector3[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {
            pathF.path = newPath;    
        }
        else {
            Debug.Log("Pathfinding was not successful");
        }
    }

    override
    public Steering Apply() {
        if (IsFinished()) {
            callback(true);
        }

        if (pathF.path != null)
            return pathF.GetSteering();
        else
            return new Steering();
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
