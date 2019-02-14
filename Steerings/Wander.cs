using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : SteeringBehaviour
{

    public float CircleRadius = 1f;
    private int wanderCooldown = 90;
    public float MaxRadius = 25f;

    public Vector3 wanderForce;

    private Vector3 inicio;

    private void Start()
    {
        wanderForce = Vector3.zero;
    }
    
    override
    public Steering Steer(Vector3 velocity)
    {

        Steering steering = new Steering();

        if (velocity == Vector3.zero)
        {
            velocity = new Vector3(1, 0, 0);
            wanderForce = GetRandomWanderForce(velocity);
        }

        var desiredVelocity = GetWanderForce(velocity);
        desiredVelocity = desiredVelocity.normalized * MaxVelocity;

        if (visibleRays) drawRays(desiredVelocity);

        steering.lineal = (desiredVelocity - velocity);

        return steering;
    }

    private Vector3 GetWanderForce(Vector3 velocity)
    {
       /* if (transform.position.magnitude > MaxRadius)
        {
            var directionToCenter = (inicio - transform.position).normalized;
            wanderForce = velocity.normalized + directionToCenter;
        }
        else */if (Time.frameCount % wanderCooldown == 0)
        {
            wanderForce = GetRandomWanderForce(velocity);
        }

        wanderForce.y = 0; 
        return wanderForce;
    }

    private Vector3 GetRandomWanderForce(Vector3 velocity)
    {
        var circleCenter = velocity.normalized;
        var randomPoint = Random.insideUnitCircle * 10;

        var displacement = new Vector3(randomPoint.x, randomPoint.y) * CircleRadius;
        displacement = Quaternion.LookRotation(velocity) * displacement;

        var wanderForce = circleCenter + displacement;
        return wanderForce;
    }

    private void drawRays(Vector3 dv)
    {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
    }

    /* Version giro bruso
     * 
     *  var desiredVelocity = GetWanderForce();
        desiredVelocity = desiredVelocity.normalized * MaxVelocity;

        var steeringForce = desiredVelocity - velocity;
        steeringForce = Vector3.ClampMagnitude(steeringForce, MaxForce);

        velocity = Vector3.ClampMagnitude(velocity + steeringForce, MaxVelocity);
        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity.normalized;

        Debug.DrawRay(transform.position, velocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
     * 
     */
}
