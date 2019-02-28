using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pursue : SteeringBehaviourTarget {

    [SerializeField]
    private float maxPrediction;

    override
    public Steering GetSteering() {
        return GetSteering(target.position, target.velocity, npc, maxAccel, maxPrediction, visibleRays);
    }

    public static Steering GetSteering(Vector3 targetPosition, Vector3 targetVelocity, Agent npc, float maxAccel, float maxPrediction, bool visibleRays = false) {
        Vector3 direction = targetPosition - npc.position;
        float distance = direction.magnitude;
        float speed = npc.velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else
            prediction = distance / speed;

        Vector3 pred_target = targetPosition + (targetVelocity * prediction);

        return Seek.GetSteering(pred_target, npc, maxAccel, visibleRays);
    }
}
