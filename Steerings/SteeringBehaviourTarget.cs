using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviourTarget : SteeringBehaviour
{
    public Body target;

    public override abstract Steering Steer();

    public Steering Steer (Body body)
    {
        target = body;
        return Steer();
    }

    public Steering Steer(Vector3 position)
    {
        target = body;
        return Steer();
    }
}
