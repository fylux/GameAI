using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : SteeringBehaviourTarget
{
    [SerializeField]
    private float leaderDistance = 2f;

    private Vector3 tv;
    private Vector3 targetVelocity;
    private Vector3 behind;

    float slowingRadius = 10f;

    private GameObject[] followers;

    [SerializeField]
    private float distanceFollowers;

    [SerializeField]
    private float maxSeparation;

    private new void Start()
    {
        base.Start();
        targetVelocity = target.velocity;
       // targetVelocity = target.GetComponent<Rigidbody>().velocity;
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.position + tv;
        followers = GameObject.FindGameObjectsWithTag("NPC");
    }

    private void Update()
    {
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.position + tv;
    }

    override
    public Steering getSteering()
    {
        Steering steering = new Steering();
        steering.linear = FollowLeader(npc.velocity);

        return steering;
    }

    private Vector3 FollowLeader(Vector3 velocity)
    {
        Vector3 force = new Vector3();

        force += Arrival.GetSteering(target, npc, slowingRadius, maxAccel).linear;
        force += Separation(velocity);
        if (OnLeaderSight())
            force += Evade.getSteering(target,npc,maxAccel, VectorDistance(target.transform.position, transform.position) * maxAccel,true,SeekT.REYNOLDS).linear;
        force.y = 0;
        return force;
    }
/*
    private Vector3 Arrive(Vector3 behind, Vector3 velocity)
    {
        // Calculate the desired velocity
        var desiredVelocity = behind - npc.position;
        var distance = VectorDistance(behind, npc.position);

        // Check the distance to detect whether the character
        // is inside the slowing area
        if (distance < slowingRadius)
        {
            // Inside the slowing area
            desiredVelocity = desiredVelocity.normalized * maxAccel * (distance / slowingRadius);
        }
        else
        {
            // Outside the slowing area.
            desiredVelocity = desiredVelocity.normalized * maxAccel;
      }

        // Set the steering based on this
        DrawRays(desiredVelocity);
        return (desiredVelocity - velocity);
    }
*/
    private Vector3 Separation(Vector3 velocity)
    {
        int numVecinos = 0;
        Vector3 force = new Vector3();

        foreach (GameObject boid in followers) //Comprobar con un SphereCast, en vez de Tag quiza usar Layers
        {
            Body bodi = boid.GetComponent<Body>();
            if (boid != this && VectorDistance(bodi.position, npc.position) <= distanceFollowers)
            {
                force.x += bodi.position.x - npc.position.x;
                force.z += bodi.position.z - npc.position.z;
                numVecinos++;
            }

        }
        if (numVecinos != 0)
        {
            force.x /= numVecinos;
            force.y /= numVecinos;

            force = force * (-1);
        }

        force = force.normalized;
        force = force * maxSeparation;

        return force;

    }

    private bool OnLeaderSight()
    {
        RaycastHit hit;
        if (Physics.Raycast(target.transform.position + (target.transform.right * 0.47f), target.transform.TransformDirection(Vector3.forward), out hit, 10f))
        {
            if (hit.rigidbody == GetComponent<Rigidbody>())
            {
                return true;
            }
        }
        else if (Physics.Raycast(target.transform.position + (target.transform.right * (-0.47f)), target.transform.TransformDirection(Vector3.forward), out hit, 10f))
        {
            if (hit.rigidbody == GetComponent<Rigidbody>())
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 Evading (Vector3 velocity)
    {
        float T = Vector3.Distance(target.transform.position, transform.position) * maxAccel;

        Vector3 posicionFutura = target.transform.position + targetVelocity * T;

        return Flee(posicionFutura);
    }

    private Vector3 Flee (Vector3 velocity)
    {
        var desiredVelocity = (transform.position - target.transform.position ).normalized * maxAccel;

        return (desiredVelocity - velocity);
    }

    private void DrawRays(Vector3 dv)
    {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
        Debug.DrawRay(target.transform.position + (target.transform.right * 0.47f), target.transform.forward * 10, Color.yellow);
        Debug.DrawRay(target.transform.position + (target.transform.right * (-0.47f)), target.transform.forward * 10, Color.yellow);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(behind, 0.5f);
    }

    private float VectorDistance(Vector3 vector1, Vector3 vector2)
    {
        return (vector1 - vector2).magnitude;
    }
}
