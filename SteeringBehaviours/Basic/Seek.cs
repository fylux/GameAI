using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviourTarget {

    override
    public Steering GetSteering() {
        return GetSteering(target.position, npc, maxAccel, visibleRays);
    }

    public static Steering GetSteering(Vector3 target, Agent npc, float maxAccel, bool visibleRays = false) {
        Steering steering = new Steering();

        var desiredVelocity = (target - npc.position).normalized * maxAccel;

        steering.linear = (desiredVelocity - npc.velocity); //Reynolds
        //steering.linear = desiredVelocity; //Millington

        if (visibleRays) {
            drawRays(npc.position, steering.linear, Color.magenta);
        }
            

        return steering;
    }
}
