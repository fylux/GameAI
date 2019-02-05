using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : Steering {


    [SerializeField]
    float leaderDistance = 5f;

    Vector3 tv;
    Vector3 targetVelocity;
    Vector3 behind;

    float slowingRadius = 5f;

    public Transform target;

    private void Start()
    {
        targetVelocity = target.GetComponent<Rigidbody>().velocity;
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.transform.position + tv;
    }

    //TO_DO: Hacer que el lider se pueda mover con  las teclas, ver el radio de slow, ver que se ralentiza al entrar a ese radio, ver el punto behind

    override
    public Vector3 Steer(Vector3 velocity)
    {
        return FollowLeader(velocity);
    }

    private Vector3 FollowLeader(Vector3 velocity)
    {
        Vector3 force = new Vector3();

        force += Arrival(behind, velocity);

        return force;
    }

    private Vector3 Arrival(Vector3 behind, Vector3 velocity)
    {
        // Calculate the desired velocity
        var desired_velocity = behind - transform.position;
        var distance = Vector3.Distance(behind, transform.position);

        // Check the distance to detect whether the character
        // is inside the slowing area
        if (distance < slowingRadius)
        {
            // Inside the slowing area
            desired_velocity = desired_velocity.normalized * MaxVelocity * (distance / slowingRadius);
}
        else
        {
            // Outside the slowing area.
            desired_velocity = desired_velocity.normalized * MaxVelocity;
      }

        // Set the steering based on this
        return (desired_velocity - velocity);
    }
}
