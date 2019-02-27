using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAlign : SteeringBehaviour
{
    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float targetRadius;

    [SerializeField]
    private float slowRadius;

    [SerializeField]
    private float timeToTarget = 0.1f;

    override
	public Steering GetSteering()
    {
        Steering steering = new Steering();

        float orienTarget = target.GetComponent<Body>().orientation;

        float rotacion = orienTarget - npc.orientation;

        rotacion = MapToRange(rotacion);
        float rotationSize = Mathf.Abs(rotacion);

        if (rotationSize < targetRadius)
            return steering;

        float targetRotation;
        if (rotationSize > slowRadius)
            targetRotation = npc.MaxRotation;
        else
            targetRotation = npc.MaxRotation * rotationSize / slowRadius;

        targetRotation *= rotacion / rotationSize;

        steering.angular = targetRotation - npc.rotation;
        steering.angular /= timeToTarget;

        float angularAccel = Mathf.Abs(steering.angular);

        if (angularAccel > npc.MaxAngular)
        {
            steering.angular /= angularAccel;
            steering.angular *= npc.MaxAngular;
        }

        return steering;



    }

    private float MapToRange (float rotation)
    {
        rotation += 180.0f;

        rotation %= 360.0f;

        if (Mathf.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f)
                rotation += 360.0f;
            else
                rotation -= 360.0f;
        }
        return rotation;
    }
}
