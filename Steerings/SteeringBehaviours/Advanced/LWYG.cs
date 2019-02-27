using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LWYG : SteeringBehaviourTarget {

    [SerializeField]
    private float targetRadius, slowRadius;

    [SerializeField]
    private float timeToTarget = 0.1f;

    [SerializeField]
    private float maxPrediction;

    public override Steering GetSteering() {
        Steering steering = new Steering();

        Vector3 direction = target.position - npc.position;
        float distance = direction.magnitude;
        float speed = npc.velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else
            prediction = distance / speed;

        Vector3 pred_target = target.position + (target.velocity * prediction);
        
        return Face.GetSteering(pred_target, npc, targetRadius, slowRadius, timeToTarget);
    }
}
