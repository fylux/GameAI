using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager : MonoBehaviour {

    public float MaxVelocity = 3;
    public float MaxForce = 15;

    protected Vector3 velocity;
 //   public Transform target;
     Steering[] steers;

    [SerializeField]
    float smooth;

   // protected Rigidbody rigid;

    private void Start()
    {
        velocity = Vector3.zero;
        //      rigid = GetComponent<Rigidbody>();ç
        steers = GetComponents<Steering>();
    }

    private void Update()
    {
           Move();
    }

    protected void Move()
    {
        var steering = Vector3.zero;
        foreach (Steering steer in steers)
            steering += steer.Steer(velocity);

        steering = Vector3.ClampMagnitude(steering, MaxForce);

        velocity = Vector3.ClampMagnitude(velocity + steering, MaxVelocity);
        //transform.position += velocity * Time.deltaTime;
        transform.position += transform.forward * Time.deltaTime * MaxVelocity;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity.normalized), Time.deltaTime * smooth);
    }
}
