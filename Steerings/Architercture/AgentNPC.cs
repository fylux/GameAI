using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentNPC : Agent {

    protected List<SteeringBehaviour> steers;

    [SerializeField]
    protected bool visibleRays;
    
    new
    protected void Start() {
        base.Start();
        steers = new List<SteeringBehaviour>(GetComponents<SteeringBehaviour>());
    }


    override
    protected void ApplySteering() {
        Steering totalSteering = new Steering();
        foreach (SteeringBehaviour steer in steers) {
            totalSteering += Steering.ApplyPriority(steer.GetSteering(), steer.blendPriority);
        }
        totalSteering += PathSteering();
        totalSteering.linear.y = 0;

        totalSteering.linear = Vector3.ClampMagnitude(totalSteering.linear, MaxAccel);
        totalSteering.angular = Mathf.Clamp(totalSteering.angular, -MaxAngular, MaxAngular);

        velocity += totalSteering.linear * Time.deltaTime;
        rotation += totalSteering.angular * Time.deltaTime;

        Debug.DrawRay(position, velocity.normalized * 2, Color.green);
    }



    public void RemoveSteer(SteeringBehaviour steer) {
        if (steers.Contains(steer))
            steers.Remove(steer);
    }

    virtual
    public Steering PathSteering() { return new Steering(); }

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
