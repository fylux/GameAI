using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class AgentUnit : AgentNPC {
    Map map;

    protected Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() { //Coste por defecto, para casos de prueba
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

    Location path_target;

    new
    protected void Start() {
        base.Start();
        GameObject terrain = GameObject.Find("Terrain");
        map = terrain.GetComponent<Map>();
        path_target = null;
    }

    override
    protected void ApplyActuator() {
        NodeT node = map.NodeFromPosition(position).type;
        float tCost = cost[node];

        velocity = Vector3.ClampMagnitude(velocity, (float)MaxVelocity / tCost);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);
    }

    override
    public Steering PathSteering() {
        if (path_target == null)
            return new Steering();

        return Seek.GetSteering(path_target.position, this, 5, visibleRays);   
    }

    public void SetFormation(Vector3 position, float orientation) {
        GoTo go = gameObject.GetComponent<GoTo>();
        if (go == null) {
            go = gameObject.AddComponent<GoTo>();
            go.Init(position, orientation);
            steers.Add(go);
        } else {
            go.target = position;
            go.orientation = orientation;
            go.active = true;
        }
    }

    public void SetTarget(Vector3 targetPosition) {
        Debug.Log("b");
        //PathfindingManager.RequestPath(position, targetPosition, cost, GoToTarget);
        path_target = new Location();
        path_target.position = targetPosition;
    }

    void GoToTarget(Vector3[] newPath, bool pathSuccessful) {
        if (!pathSuccessful) {
            Debug.Log("Not reachable");
            return;
        }

        //Remove previous PathFollowing
        RemoveSteering(steers.Find(steer => steer is PathFollowing));

        if (newPath.Length > 0) {
            PathFollowing pf = gameObject.AddComponent<PathFollowing>();
            pf.path = newPath;
            pf.visibleRays = visibleRays;
            pf.maxAccel = 50f;
            steers.Add(pf);
        }
    }
}
