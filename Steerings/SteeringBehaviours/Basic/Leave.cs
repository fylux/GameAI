using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : SteeringBehaviourTarget {

    override
    public Steering GetSteering() {
        return GetSteering(target.position, npc, maxAccel, visibleRays);
    }

    public static Steering GetSteering(Vector3 target, Body npc, float maxAccel, bool visibleRays = false) {
        return Flee.GetSteering(target, npc, maxAccel, visibleRays);
    }
}
