using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LWYG : SteeringBehaviour {

    [SerializeField]
    private float maxPrediction;

    [SerializeField]
    protected Body target;

    public override Steering getSteering()
    {
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

        // return Face.getSteering(...), siendo el target nuestro pred_target

        return steering;
    }
}
