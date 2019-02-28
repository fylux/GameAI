using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviourTarget {

    [SerializeField]
    float slowingRadius = 10f;

    public override Steering GetSteering() { 
        return Arrive.GetSteering(target.position, npc, slowingRadius, maxAccel);
    }

    public static Steering GetSteering(Vector3 targetPosition, Agent npc, float slowingRadius, float maxAccel) {
        Steering steering = new Steering();

        // Calculate the desired velocity
        var desiredVelocity = targetPosition - npc.position;
        var distance = (targetPosition - npc.position).magnitude;

        // Check the distance to detect whether the character
        // is inside the slowing area
        if (distance < slowingRadius) {
            // Inside the slowing area
            desiredVelocity = desiredVelocity.normalized * maxAccel * (distance / slowingRadius);
        }
        else {
            // Outside the slowing area.
            desiredVelocity = desiredVelocity.normalized * maxAccel;
        }

        // Set the steering based on this
        //DrawRays(desiredVelocity);
        steering.linear = desiredVelocity - npc.velocity;
        return steering;
    }
}
