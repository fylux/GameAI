using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Evade : SteeringBehaviourTarget {
    [SerializeField]
    private float maxPrediction;

    override
    public Steering GetSteering() {
        return GetSteering(target.position, target.velocity, npc, maxAccel, maxPrediction, visibleRays);
    }


    public static Steering GetSteering(Vector3 targetPosition, Vector3 targetVelocity, Agent npc, float maxAccel, float maxPrediction, bool visibleRays = false) {
        return -Pursue.GetSteering(targetPosition, targetVelocity,npc,maxAccel,maxPrediction,visibleRays);
    }

}
