using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoTo : Task {
    GameObject empty;
    PathFollowing pathF;
    Vector3 target;
    float offset;
    bool defensive;
    float reconsiderSeconds;

    public GoTo(AgentUnit agent, Vector3 target, float reconsiderSeconds, float offset, bool defensive, Action<bool> callback) : base(agent,callback) {
        this.offset = offset;
        this.defensive = defensive;
        this.reconsiderSeconds = reconsiderSeconds;

        empty = new GameObject();
        empty.transform.parent = agent.gameObject.transform;

        pathF = empty.AddComponent<PathFollowing>();
        pathF.SetNPC(agent);
        pathF.visibleRays = true;
        pathF.maxAccel = 50f;

        SetNewTarget(target);
    }

    public GoTo(AgentUnit agent, Vector3 target, Action<bool> callback) : this(agent, target, Mathf.Infinity, 0f, false, callback) { }

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
        if (defensive)
            PathfindingManager.RequestPath(agent.position, target, agent.Cost, 100f, Util.OppositeFaction(agent.faction), ProcessPath);
        else
            PathfindingManager.RequestPath(agent.position, target, agent.Cost, 100f, Faction.C, ProcessPath);
    }

    override
    public Steering Apply() {
        Steering st = new Steering();
        if (IsFinished()) {
            callback(true);
            return st;
        }

        if (pathF.path != null) {
            if (Time.fixedTime - timeStamp > reconsiderSeconds) {
                timeStamp = Time.fixedTime;
                PathfindingManager.RequestPath(agent.position, target, agent.Cost, 100f, Faction.B, ProcessPath);
            }

            st= pathF.GetSteering();
        }
            
        else
            st= Seek.GetSteering(target, agent, 500f); //If path has not been solved yet just do Seek.

        st += GetSeparation(2f);

        return st;
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

    public Steering GetSeparation( float decayCoefficient) {
        Steering steering = new Steering();
        var units = Info.GetUnitsArea(agent.position, 0.4f);
        foreach (var unit in units) {
            Vector3 direction = agent.position - unit.position;
            float distance = direction.magnitude;
            if (agent != unit) {
                if (AngleDir2(new Vector2(agent.velocity.x, agent.velocity.z), new Vector2(-direction.x,-direction.z)) > 0f/*AngleDir(agent.velocity, unit.position - agent.position, Vector3.up) == 1.0f*/) {
                    direction = Quaternion.Euler(0, 75, 0) * direction.normalized;
                }
                else {
                    direction = Quaternion.Euler(0, -75, 0) * direction.normalized;
                }
                steering.linear += 10000f * direction;

            }
            
            
        }
        if (steering.linear.magnitude > 0) {
            Debug.DrawRay(agent.position, steering.linear.normalized, Color.blue);
            agent.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else {
            agent.GetComponent<Renderer>().material.color = Color.red;
        }
            //steering.linear *= (-1);

            return steering;
    }

    public float AngleDir2(Vector2 A, Vector2 B) {
        return -A.x * B.y + A.y * B.x;
    }


}


