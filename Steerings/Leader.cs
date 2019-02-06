using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : Steering {


    [SerializeField]
    float leaderDistance = 2f;

    Vector3 tv;
    Vector3 targetVelocity;
    Vector3 behind;

    float slowingRadius = 10f;

    public Transform target;

    GameObject[] seguidores;
    [SerializeField]
    float distanciaFollowers;
    [SerializeField]
    float separacionMaxima;

    private void Start()
    {
        targetVelocity = target.GetComponent<Rigidbody>().velocity;
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.transform.position + tv;
        seguidores = GameObject.FindGameObjectsWithTag("NPC");
    }

    private void Update()
    {
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.transform.position + tv;
    }

    override
    public Vector3 Steer(Vector3 velocity)
    {
        
        return FollowLeader(velocity);
    }

    private Vector3 FollowLeader(Vector3 velocity)
    {
        Vector3 force = new Vector3();

        force += Arrival(behind, velocity);
        force += Separation(velocity);
        if (OnLeaderSight())
            force += Evade(velocity);
        force.y = 0;
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
        DrawRays(desired_velocity);
        return (desired_velocity - velocity);
    }

    private Vector3 Separation(Vector3 velocity)
    {
        int numVecinos = 0;
        Vector3 force = new Vector3();

        foreach (GameObject boid in seguidores)
        {
            if (boid != this && Vector3.Distance(boid.transform.position, transform.position) <= distanciaFollowers)
            {
                force.x += boid.transform.position.x - this.transform.position.x;
                force.z += boid.transform.position.z - this.transform.position.z;
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
        force = force * separacionMaxima;

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

    private Vector3 Evade (Vector3 velocity)
    {
        float T = Vector3.Distance(target.transform.position, transform.position) * MaxVelocity;

        Vector3 posicionFutura = target.transform.position + targetVelocity * T;

        return Flee(posicionFutura);
    }

    private Vector3 Flee (Vector3 velocity)
    {
        var desiredVelocity = (transform.position - target.transform.position ).normalized * MaxVelocity;

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
}
