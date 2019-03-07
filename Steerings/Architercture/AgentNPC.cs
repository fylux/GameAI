using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentNPC : Agent {

    List<SteeringBehaviour> steers;

    [SerializeField]
    bool visibleRays;

    Vector3 target;

    Vector3 virtualTarget;

    private new void Start() {
        base.Start();
        steers = new List<SteeringBehaviour>(GetComponents<SteeringBehaviour>());
    }

    override
    protected void ApplySteering() {
       
        Steering totalSteering = new Steering();
        foreach (SteeringBehaviour steer in steers) {
            totalSteering += Steering.ApplyPriority(steer.GetSteering(), steer.blendPriority);
            Debug.Log(steer);
        }

        totalSteering.linear = Vector3.ClampMagnitude(totalSteering.linear, maxAccel);
        totalSteering.angular = Mathf.Clamp(totalSteering.angular, -MaxAngular, MaxAngular);

        velocity += totalSteering.linear * Time.deltaTime;
        rotation += totalSteering.angular * Time.deltaTime;

        Debug.DrawRay(position, velocity.normalized * 2, Color.green);
    }

    public void SetFormation(Vector3 position)
    {
        GoTo go = gameObject.GetComponent<GoTo>();
        if (go == null)
        {
            go = gameObject.AddComponent<GoTo>();
            go.Init(position);
            steers.Add(go);
        }
        else
        {
            go.target = position;
            go.active = true;
        } 
    }

    public void SetTarget(Vector3 targetPosition) {
        PathfindingManager.RequestPath(position, targetPosition, GoToTarget);
        target = targetPosition;
    }

    void GoToTarget(Vector3[] newPath, bool pathSuccessful) {
        PathFollowing previous = null;
        if (pathSuccessful) {
            foreach (SteeringBehaviour steer in steers) {
                if (steer is PathFollowing)
                    previous = (PathFollowing)steer;
            }
            
            steers.Remove(previous);
            Destroy(previous);
            if (newPath.Length > 0) {
                PathFollowing pf = gameObject.AddComponent<PathFollowing>();
                pf.path = newPath;
                steers.Add(pf);

            }
        }
        else {
            Debug.Log("Not reachable");
        }
    }

    public void RemoveSteer (SteeringBehaviour steer)
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
