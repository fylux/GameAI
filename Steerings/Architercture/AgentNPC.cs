using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentNPC : Agent {

    private SteeringBehaviour[] steers;

    private new void Start() {
        base.Start();
        steers = GetComponents<SteeringBehaviour>();
    }

    override
    protected void ApplySteering() {
        Vector3 steeringLinear = Vector3.zero;
        float steeringAngular = 0.0f;
        foreach (SteeringBehaviour steer in steers) {
            Steering steering = steer.GetSteering();
            steeringLinear += steering.linear * steer.blendPriority;
            steeringAngular += steering.angular * steer.blendPriority;
        }

        steeringLinear = Vector3.ClampMagnitude(steeringLinear, maxAccel);
        steeringAngular = Mathf.Clamp(steeringAngular, -MaxAngular, MaxAngular);

        velocity += steeringLinear * Time.deltaTime;
        rotation += steeringAngular * Time.deltaTime;
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
