using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoTo : Task {

    GameObject empty;
    PathFollowing pathF;
    Vector3 target;


    public GoTo(AgentUnit agent, Vector3 target, Action<bool> callback) : base(agent,callback) {
        this.target = new Vector3(target.x, 111f, target.z);

        empty = new GameObject();
        empty.transform.parent = agent.gameObject.transform;

        pathF = empty.AddComponent<PathFollowing>();
        pathF.SetNPC(agent);
        pathF.path = null;
        pathF.visibleRays = true;
        pathF.maxAccel = 50f;

        PathfindingManager.RequestPath(agent.position, target, agent.Cost, ProcessPath);
    }

    private void ProcessPath(Vector3[] newPath, List<Node> nodesPath, bool pathSuccessful) {
        if (pathSuccessful) {
            pathF.SetPath(newPath);
        }
        else {
            Debug.Log("Pathfinding was not successful");
        }
    }

    public void FinishPath() {
        pathF.SetPath(null);
    }

    public void SetNewTarget(Vector3 target) {
        this.target = new Vector3(target.x, 0f, target.z);
        pathF.path = null;
        PathfindingManager.RequestPath(agent.position, target, agent.Cost, ProcessPath);
    }

    override
    public Steering Apply() {
        if (IsFinished()) callback(true);

        if (pathF.path != null)
            return pathF.GetSteering();
        else
            return Seek.GetSteering(target, agent, 10f); //If path has not been solved yet just do Seek.
    }

    override
    protected bool IsFinished() {
        return Util.HorizontalDist(agent.position, target) < 0.3f;
    }

    override
    public void Terminate() {
        if (empty != null) UnityEngine.Object.Destroy(empty);
        agent.RequestStopMoving(); //To remove remaining forces of movement
    }
}
