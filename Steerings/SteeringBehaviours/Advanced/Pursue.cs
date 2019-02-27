using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pursue : SteeringBehaviourTarget {

    [SerializeField]
    private float maxPrediction;

    override
    public Steering GetSteering() {
        return GetSteering(target, npc, maxAccel, maxPrediction, visibleRays);
    }

    public static Steering GetSteering(Body target, Body npc, float maxAccel, float maxPrediction, bool visibleRays = false) {
        Vector3 direction = target.position - npc.position;
        float distance = direction.magnitude;
        float speed = npc.velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else
            prediction = distance / speed;

        Vector3 pred_target = target.position + (target.velocity * prediction);

        return Seek.GetSteering(pred_target, npc, maxAccel, visibleRays);
    }
}
