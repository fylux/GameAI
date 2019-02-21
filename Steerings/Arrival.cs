using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrival : SteeringBehaviourTarget {

    [SerializeField]
    float slowingRadius = 10f;

    public override Steering getSteering()
    { 
        return Arrival.GetSteering(target, npc, slowingRadius, maxAccel);
    }

    public static Steering GetSteering(Body target, Body npc, float slowingRadius, float maxAccel)
    {
        Steering steering = new Steering();

        // Calculate the desired velocity
        var desiredVelocity = target.position - npc.position;
        var distance = (target.position - npc.position).magnitude;

        // Check the distance to detect whether the character
        // is inside the slowing area
        if (distance < slowingRadius)
        {
            // Inside the slowing area
            desiredVelocity = desiredVelocity.normalized * maxAccel * (distance / slowingRadius);
        }
        else
        {
            // Outside the slowing area.
            desiredVelocity = desiredVelocity.normalized * maxAccel;
        }

        // Set the steering based on this
        //DrawRays(desiredVelocity);
        steering.linear = desiredVelocity - npc.velocity;
        return steering;
    }
    /*
    private float VectorDistance(Vector3 vector1, Vector3 vector2)
    {
        return (vector1 - vector2).magnitude;
    }*/
}
