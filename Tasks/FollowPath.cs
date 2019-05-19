using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FollowPath : Task {
    GameObject empty;
    public PathFollowing pathF;

    public FollowPath(AgentUnit agent, Vector3[] path, FollowT type, Action<bool> callback) : base(agent,callback) {
        empty = new GameObject();
        if (agent == null) {
            Debug.Log(agent);
            Debug.LogError("Follow path en unidad muerta ");
        }
        else
            empty.transform.parent = agent.gameObject.transform;

        if (type == FollowT.STAY) {
            pathF = empty.AddComponent<PathFollowingAdvanced>();
        }
        else {
            pathF = empty.AddComponent<PathFollowing>();
            pathF.type = type;
        }
        pathF.SetNPC(agent);
        pathF.SetPath(path);
        
        pathF.visibleRays = true;
        pathF.maxAccel = 700f;
    }

    public FollowPath(AgentUnit agent, Vector3[] path, Action<bool> callback) : this(agent, path, FollowT.STAY, callback) { }



    public void SetPath(Vector3[] path) {

        if (path != null) {
            if (path.Length == 0) {
                Debug.LogError("0 length path, unit "+agent);
            }

            Vector3 closestPoint = path.OrderBy(p => Util.HorizontalDist(p, agent.position)).First();
            for (int i = 0; i < path.Length; ++i) {
                if (closestPoint == path[i]) {
                    pathF.currentPoint = i;
                    break;
                }
            }

            path = PathUtil.SimplifyPath(path, true).ToArray();
        }

        pathF.SetPath(path);

    }

    public bool HasPath() {
        return pathF.path != null;
    }

    override
    public Steering Apply() {
        Steering st = new Steering();
        if (IsFinished()) {
            callback(true);
            return st;
        }

		if (pathF.path != null) {
			st = pathF.GetSteering();
		}

        st += GetAvoidCollisions();
        st += WallAvoidance.GetSteering(agent, 10000f, Map.terrainMask, 0.7f, 0.7f, 0.5f, false);
        

        return st;
    }

    override
    protected bool IsFinished() {
        return pathF.path != null && (pathF.path.Length-1 == pathF.currentPoint) && pathF.type == FollowT.STAY;
    }

    override
    public void Terminate() {
        if (empty != null) UnityEngine.Object.Destroy(empty);
        agent.RequestStopMoving(); //To remove remaining forces of movement
    }

    public void SetVisiblePath(bool visiblePath) {
        pathF.visibleRays = visiblePath;
    }

    public Steering GetAvoidCollisions() {
        Steering steering = new Steering();
        var units = Info.GetUnitsArea(agent.position, 0.4f);
        foreach (var unit in units) {
            Vector3 direction = agent.position - unit.position;
            float distance = direction.magnitude;
            if (agent != unit) {
                if (AngleDir(new Vector2(agent.velocity.x, agent.velocity.z), new Vector2(-direction.x, -direction.z)) > 0f) {
                    direction = Quaternion.Euler(0, 45, 0) * direction.normalized;
                } else {
                    direction = Quaternion.Euler(0, -45, 0) * direction.normalized;
                }
                steering.linear += 1000f * direction;
            }


        }
        if (steering.linear.magnitude > 0) {
            //Debug.DrawRay(agent.position, steering.linear.normalized, Color.blue);
            //agent.hat.GetComponent<Renderer>().material.color = Color.black;
        } else {
            //agent.hat.GetComponent<Renderer>().material.color = MilitaryResourcesAllocator.strategyColor[agent.strategy];
        }

        return steering;
    }

    //Positive means (left?) and negative means (right?)
    public float AngleDir(Vector2 A, Vector2 B) {
        return -A.x * B.y + A.y * B.x;
    }

    override
    public string ToString() {
        return "FollowPath -> "+pathF.path;
    }
}


