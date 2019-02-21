using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : SteeringBehaviourTarget {


    override
    public Steering Steer()
    {
        Steering steering = new Steering();

        Vector3 direction = target.position - body.position;

        if (direction.magnitude <= 0.0f)
            return steering;

        float targetOrientation = Mathf.Atan2(direction.x, direction.z);
        targetOrientation *= Mathf.Rad2Deg;

        // virtualTarget.GetComponent<Body>.orientation = targetOrientation
        // return Align.Steer(...), siendo el target con el que nos vamos a alinear el virtualTarget

        return steering;
    }


}
