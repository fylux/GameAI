using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentNPC : Agent {

    List<SteeringBehaviour> steers;

    [SerializeField]
    bool visibleRays;

    Vector3 target;

    Vector3 virtualTarget;

    public Map map;

    protected Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() { //Coste por defecto, para casos de prueba
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

    new
    protected void Start() {
        base.Start();
        steers = new List<SteeringBehaviour>(GetComponents<SteeringBehaviour>());
    }

    override
    protected void ApplyActuator()
    {
        velocity.y = 0;

        NodeT node = map.NodeFromPosition(position).type;
        float tCost = cost[node];

        velocity = Vector3.ClampMagnitude(velocity, (float)MaxVelocity / tCost);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);
    }

    override
    protected void ApplySteering() {
        Steering totalSteering = new Steering();
        foreach (SteeringBehaviour steer in steers) {
            totalSteering += Steering.ApplyPriority(steer.GetSteering(), steer.blendPriority);
        }

        totalSteering.linear.y = 0;

        totalSteering.linear = Vector3.ClampMagnitude(totalSteering.linear, MaxAccel);
        totalSteering.angular = Mathf.Clamp(totalSteering.angular, -MaxAngular, MaxAngular);

        velocity += totalSteering.linear * Time.deltaTime;
        rotation += totalSteering.angular * Time.deltaTime;

        Debug.DrawRay(position, velocity.normalized * 2, Color.green);
    }

    public void SetFormation(Vector3 position, float orientation) {
        GoTo go = gameObject.GetComponent<GoTo>();
        if (go == null)
        {
            go = gameObject.AddComponent<GoTo>();
            go.Init(position, orientation);
            steers.Add(go);
        }
        else
        {
            go.target = position;
            go.orientation = orientation;
            go.active = true;
        }
    }

    public void SetTarget(Vector3 targetPosition) {
        PathfindingManager.RequestPath(position, targetPosition, cost, GoToTarget);
        target = targetPosition;
    }

    void GoToTarget(Vector3[] newPath, bool pathSuccessful) {
        PathFollowing previous = null;
        if (pathSuccessful) {
            foreach (SteeringBehaviour steer in steers) {
                if (steer is PathFollowing) {
                    previous = (PathFollowing)steer;
                    steers.Remove(previous);
                    Destroy(previous);
                    break;
                }
            }
            
            if (newPath.Length > 0) {
                PathFollowing pf = gameObject.AddComponent<PathFollowing>();
                pf.path = newPath;
                pf.visibleRays = visibleRays;
                pf.maxAccel = 50f;
                steers.Add(pf);
            }
        }
        else {
            Debug.Log("Not reachable");
        }
    }

    public void RemoveSteer(SteeringBehaviour steer)
    {
        if (steers.Contains(steer))
            steers.Remove(steer);
    }
    /*
    private void OnMouseEnter() {
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = new Color(1, 0, 0);
    }

    private void OnMouseOver() {
        Renderer rend = GetComponent<Renderer>();
        rend.material.color += new Color(-.5f, 0, -.5f) * Time.deltaTime;
    }

    private void OnMouseExit() {
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = Color.white;
    }*/
}
