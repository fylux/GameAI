using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentNPC : Agent {

    List<SteeringBehaviour> steers;

    [SerializeField]
    bool visibleRays;

    Vector3 target;
    bool goTo = false;

    private new void Start() {
        base.Start();
        steers = new List<SteeringBehaviour>(GetComponents<SteeringBehaviour>());
    }

    override
    protected void ApplySteering() {
       
        Steering totalSteering = new Steering();
        foreach (SteeringBehaviour steer in steers) {
            totalSteering += Steering.ApplyPriority(steer.GetSteering(), steer.blendPriority);
        }
        /*if (goTo) {
            totalSteering += Seek.GetSteering(target, this, 5, true);
        }*/

        totalSteering.linear = Vector3.ClampMagnitude(totalSteering.linear, maxAccel);
        totalSteering.angular = Mathf.Clamp(totalSteering.angular, -MaxAngular, MaxAngular);

        velocity += totalSteering.linear * Time.deltaTime;
        rotation += totalSteering.angular * Time.deltaTime;

        Debug.DrawRay(position, velocity.normalized * 2, Color.green);
    }


    public void SetTarget(Vector3 position) {
        PathfindingManager.RequestPath(transform.position, position, GoToTarget);
        target = position;
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
                goTo = true;
            }
        }
        else {
            Debug.Log("Not reachable");
        }
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
