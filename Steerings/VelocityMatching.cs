using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{
    [SerializeField]
    private Body target;

    [SerializeField]
    private float timeToTarget = 0.1f;

    override
    public Steering getSteering() {
        return getSteering(npc, target, maxAccel, timeToTarget, visibleRays);
    }

    public static Steering getSteering(Body npc, Body target, float maxAccel, float timeToTarget, bool visibleRay) {
        Steering steering = new Steering();
        steering.linear = (target.velocity - npc.velocity) / timeToTarget;

        if (steering.linear.magnitude > maxAccel)
            steering.linear = (steering.linear).normalized * maxAccel;  

        return steering;
    }
}
