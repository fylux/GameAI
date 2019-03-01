using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviourTarget {

    override
    public Steering GetSteering() {
        return GetSteering(target.position, npc, maxAccel, true);
    }

    public static Steering GetSteering(Vector3 target, Agent npc, float maxAccel, bool visibleRays = false) {
        Steering steering = -Seek.GetSteering(target, npc, maxAccel, false);

        if (visibleRays)
        {
            drawRays(npc.position, steering.linear, Color.magenta);
            drawRays(npc.position, npc.velocity);
        }
            

        return steering;
    }
}
