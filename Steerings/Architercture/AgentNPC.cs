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
        Steering totalSteering = new Steering();
        foreach (SteeringBehaviour steer in steers) {
            totalSteering += Steering.ApplyPriority(steer.GetSteering(), steer.blendPriority);
        }

        totalSteering.linear = Vector3.ClampMagnitude(totalSteering.linear, maxAccel);
        totalSteering.angular = Mathf.Clamp(totalSteering.angular, -MaxAngular, MaxAngular);

        velocity += totalSteering.linear * Time.deltaTime;
        rotation += totalSteering.angular * Time.deltaTime;
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
