using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{
    [SerializeField]
    private Body target;

    [SerializeField]
    private float timeToTarget = 0.1f;

    override
    public Steering Steer()
    {
        Steering steering = new Steering();
        steering.lineal = (target.velocity - body.velocity) / timeToTarget;

        if (steering.lineal.magnitude > MaxAccel)
            steering.lineal = (steering.lineal).normalized * MaxAccel;  

        return steering;
    }
}
