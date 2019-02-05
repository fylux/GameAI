using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Steering : MonoBehaviour {

    //  public float Mass = 15;

    public float MaxVelocity = 3;
    public float MaxForce = 15;

   // private float minDist = 3f;
   // private float maxDist = 30f;

   // private int wanderCooldown = 120;

    protected Vector3 velocity;
    public Transform target;

    private Rigidbody rigid;

    private void Start()
    {
        velocity = Vector3.zero;
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
           move();
    }

    protected abstract void move();
}
