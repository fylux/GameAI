using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoTo : Task {
    GameObject empty;
    PathFollowing pathF;
    Vector3 target;
    float offset;


    public GoTo(AgentUnit agent, Vector3 target, float offset, Action<bool> callback) : base(agent,callback) {
        this.offset = offset;

        empty = new GameObject();
        empty.transform.parent = agent.gameObject.transform;

        pathF = empty.AddComponent<PathFollowing>();
        pathF.SetNPC(agent);
        pathF.visibleRays = true;
        pathF.maxAccel = 50f;

        SetNewTarget(target);
    }

    public GoTo(AgentUnit agent, Vector3 target, Action<bool> callback) : this(agent, target, 0f, callback) { }

    private void ProcessPath(Vector3[] newPath, bool pathSuccessful) {
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

    public void SetNewTarget(Vector3 new_target) {
        do {
            Vector2 offsetXY = UnityEngine.Random.insideUnitCircle * offset;

            target = new Vector3(new_target.x + offsetXY[0], 1f, new_target.z + offsetXY[1]);
        } while(!Map.NodeFromPosition(target, true).isWalkable());

        pathF.path = null;
        PathfindingManager.RequestPath(agent.position, target, agent.Cost, 100f, Util.OppositeFaction(agent.faction), ProcessPath);
    }

    override
    public Steering Apply() {
        if (IsFinished()) callback(true);

        if (pathF.path != null) {
            if (Time.fixedTime - timeStamp > 2) {
                timeStamp = Time.fixedTime;
                PathfindingManager.RequestPath(agent.position, target, agent.Cost, 100f, Faction.B, ProcessPath);
            }

            return pathF.GetSteering();
        }
            
        else
            return Seek.GetSteering(target, agent, 50f); //If path has not been solved yet just do Seek.
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

    public void SetVisiblePath(bool visiblePath) {
        pathF.visibleRays = visiblePath;
    }

    override
    public String ToString() {
        return "GoTo -> "+target;
    }
}
