using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentNPC : Agent {

    protected List<SteeringBehaviour> steers;
    protected Task task;

    [SerializeField]
    protected bool visibleRays;

    Vector3 virtualTarget;

    new
    protected void Start() {
        base.Start();
        steers = new List<SteeringBehaviour>(GetComponents<SteeringBehaviour>());
        task = null;
    }


    protected Steering ApplySteering() {
        Steering totalSteering = new Steering();
        foreach (SteeringBehaviour steer in steers) {
            totalSteering += Steering.ApplyPriority(steer.GetSteering(), steer.blendPriority);
        }
        //totalSteering += PathSteering();
        if (task != null) totalSteering += task.Apply();
        totalSteering.linear.y = 0;

        totalSteering.linear = Vector3.ClampMagnitude(totalSteering.linear, MaxAccel);
        totalSteering.angular = Mathf.Clamp(totalSteering.angular, -MaxAngular, MaxAngular);

        return totalSteering;
    }

    override
    protected void ApplyActuator() { // Aqui el Actuator suma los steerings, los aplica a las velocidades, y las limita

        Steering steering = ApplySteering();

        velocity += steering.linear * Time.deltaTime;
        rotation += steering.angular * Time.deltaTime;

        velocity.y = 0;

        velocity = Vector3.ClampMagnitude(velocity, MaxVelocity);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);

        Debug.DrawRay(position, velocity.normalized * 2, Color.green);
    }



    public void RemoveSteering(SteeringBehaviour steer) {
        if (steers.Contains(steer)) {
            steers.Remove(steer);
            Destroy(steer);
        }
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
