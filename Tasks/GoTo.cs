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

    public bool finished = false;
    public bool processing = false;

    static int n = 0;

    public GoTo(AgentUnit agent, Vector3 target, float reconsiderSeconds, float offset, bool defensive, Action<bool> callback) : base(agent,callback) {
        this.offset = offset;
        this.defensive = defensive;
        this.reconsiderSeconds = Mathf.Infinity;//reconsiderSeconds;

        followPath = new FollowPath(agent, null, (_) => {
            finished = true;
        });

        SetNewTarget(target);
        n++;

        //Debug.Log("numero gotos " + n);
    }

	public GoTo(AgentUnit agent, Vector3 target, Action<bool> callback) : this(agent, target, Mathf.Infinity, 0f, false, callback) { }

    private void ProcessPath(Vector3[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {
            followPath.SetPath(newPath);
        }
        else {
            Debug.Log(Time.frameCount +" Pathfinding was not successful");
        }
        processing = false;
    }

    public void FinishPath() {
        followPath.SetPath(null);
        finished = true;
    }

    public void SetVisiblePath(bool visible) {
        followPath.SetVisiblePath(visible);
    }

    public void SetNewTarget(Vector3 new_target, bool modify = true) {
        if (modify) {
            int i = 0;
            do {
                Vector2 offsetXY = UnityEngine.Random.insideUnitCircle * offset;
                target = Map.Clamp(new Vector3(new_target.x + offsetXY[0], 1f, new_target.z + offsetXY[1]));
                i++;
            } while (!Map.NodeFromPosition(target).isWalkable() && i < 20);
            if (!Map.NodeFromPosition(target).isWalkable()) {
                Debug.Log("GoTo target not walkable "+new_target); //error
                finished = true;
                return;
            }
            
        }

        processing = true;
        if (defensive)
			PathfindingManager.RequestPath(agent, target, agent.faction, ProcessPath);
        else
			PathfindingManager.RequestPath(agent, target, Faction.C, ProcessPath);

        finished = false;
    }

    override
    public Steering Apply() {
        Steering st = new Steering();
        if (IsFinished()) {
            callback(true);
            return st;
        }

        if (processing) {
            agent.hat.GetComponent<Renderer>().material.color = Color.white;
        }

		if (followPath.HasPath()) {
			if (Time.fixedTime - timeStamp > reconsiderSeconds) {
				timeStamp = Time.fixedTime;
                SetNewTarget(target, false);
			}
//            Debug.Log(Time.frameCount + " " + agent.name + " has path");
            st = followPath.Apply();
		} else {
//            Debug.Log(Time.frameCount + " " + agent.name + " NO has path");
            //st= Seek.GetSteering(target, agent, 500f); //If path has not been solved yet just do Seek.
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
        return "GoTo -> "+ target + " current point "+followPath.pathF.currentPoint+", processing "+processing;
    }
}


