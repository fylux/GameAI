﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SteeringManager : Body {

    private SteeringBehaviour[] steers;

    private new void Start() {
        base.Start();
        steers = GetComponents<SteeringBehaviour>();
    }

    override
    protected void UpdateForces() {
        Vector3 steeringLinear = Vector3.zero;
        float steeringAngular = 0.0f;
        foreach (SteeringBehaviour steer in steers) {
            Steering steering = steer.getSteering();
            steeringLinear += steering.linear * steer.blendPriority;
            steeringAngular += steering.angular * steer.blendPriority;
        }

        steeringLinear = Vector3.ClampMagnitude(steeringLinear, maxAccel);
        steeringAngular = Mathf.Clamp(steeringAngular, -MaxAngular, MaxAngular);

        velocity += steeringLinear * Time.deltaTime;
        rotation += steeringAngular * Time.deltaTime;
    }

}
