using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Evade : SteeringBehaviourTarget {
    [SerializeField]
    float maxPrediction;

    override
    public Steering GetSteering() {
        return GetSteering(target, npc, maxAccel, maxPrediction, visibleRays);
    }


    public static Steering GetSteering(Agent target, Agent npc, float maxAccel, float maxPrediction, bool visibleRays = false) {
        Steering steering = -Pursue.GetSteering(target,npc,maxAccel,maxPrediction,false);

        if (visibleRays)
            drawRays(npc.position, steering.linear, Color.magenta);

        return steering;
    }

}
