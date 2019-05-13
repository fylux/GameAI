using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GoTo : Task {
    FollowPath followPath;
    Vector3 target;
    float offset;
    bool defensive;
    float reconsiderSeconds;

    bool finished = false;

    public GoTo(AgentUnit agent, Vector3 target, float reconsiderSeconds, float offset, bool defensive, Action<bool> callback) : base(agent,callback) {
        this.offset = offset;
        this.defensive = defensive;
        this.reconsiderSeconds = reconsiderSeconds;

        followPath = new FollowPath(agent, null, (_) => {
            finished = true;
        });

        SetNewTarget(target);
    }

	public GoTo(AgentUnit agent, Vector3 target, Action<bool> callback) : this(agent, target, Mathf.Infinity, 0f, false, callback) { }

    private void ProcessPath(Vector3[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {
            followPath.SetPath(newPath);
        }
        else {
            Debug.Log("Pathfinding was not successful");
        }
    }

    public void FinishPath() {
        followPath.SetPath(null);
    }

    public void SetVisiblePath(bool visible) {
        followPath.SetVisiblePath(visible);
    }

    public void SetNewTarget(Vector3 new_target, bool modify = true) {
        if (modify) {
            do {
                Vector2 offsetXY = UnityEngine.Random.insideUnitCircle * offset;
                target = new Vector3(new_target.x + offsetXY[0], 1f, new_target.z + offsetXY[1]);
            } while (!Map.NodeFromPosition(target, true).isWalkable());
        }
		
        if (defensive)
            PathfindingManager.RequestPath(agent.position, target, agent.Cost, agent.faction, ProcessPath);
        else
            PathfindingManager.RequestPath(agent.position, target, agent.Cost, Faction.C, ProcessPath);
    }

    override
    public Steering Apply() {
        Steering st = new Steering();
        if (IsFinished()) {
            callback(true);
            return st;
        }

		if (followPath.HasPath()) {
			if (Time.fixedTime - timeStamp > reconsiderSeconds) {
				timeStamp = Time.fixedTime;
                SetNewTarget(target, false);
			}

			st = followPath.Apply();
		} else {
			st= Seek.GetSteering(target, agent, 500f); //If path has not been solved yet just do Seek.
		}

        return st;
    }

    override
    protected bool IsFinished() {
        return finished;
    }

    override
    public void Terminate() {
        if (followPath != null) followPath.Terminate();
        agent.RequestStopMoving(); //To remove remaining forces of movement
    }


    override
    public String ToString() {
        return "GoTo -> "+target;
    }
}


