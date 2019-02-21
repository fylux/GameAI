using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Evade : SteeringBehaviour
{
    [SerializeField]
    private SeekType seekType = SeekType.REYNOLDS;

    [SerializeField]
    protected Body target;

    [SerializeField]
    private float maxPrediction;

    override
    public Steering Steer()
    {
        Steering steering = new Steering();

        Vector3 direction = target.position - body.position;
        float distance = direction.magnitude;
        float speed = body.velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else
            prediction = distance / speed;

        Vector3 pred_target = target.position + (target.velocity * prediction);

        return Flee.Steer(pred_target,body,MaxAccel,visibleRays, seekType);
    }

}
