using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : Steering {

    private float maxDist = 30f;

    override
    protected void move()
    {

        var desiredVelocity = (transform.position - target.transform.position).normalized * MaxVelocity;

        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);

        velocity = Vector3.ClampMagnitude(velocity + steering, MaxVelocity);
        //transform.position += velocity * Time.deltaTime;
        if (Vector3.Distance(target.transform.position, transform.position) < maxDist)
            transform.position += transform.forward * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity.normalized), Time.deltaTime);

        var z = desiredVelocity.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
    }

}
