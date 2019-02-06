using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdAlign : Steering {

    GameObject[] seguidores;

    [SerializeField]
    float followersRadius = 3f;

    private void Start()
    {
        seguidores = GameObject.FindGameObjectsWithTag("NPC");
    }

    override
    public Vector3 Steer(Vector3 velocity)
    {
        int numVecinos = 0;
        Vector3 force = new Vector3();

        foreach (GameObject boid in seguidores)
        {
            if (boid != this && Vector3.Distance(boid.transform.position, transform.position) <= followersRadius)
            {
                force.x += boid.transform.position.x;
                force.z += boid.transform.position.z;
                numVecinos++;
            }

        }

        if (numVecinos == 0)
            return force;

        force.x /= numVecinos;
        force.y /= numVecinos;

        force = force.normalized;

        return force;

    }
}
