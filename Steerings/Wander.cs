using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : SteeringBehaviour
{

    [SerializeField]
    private float circleRadius = 1f;

    [SerializeField]
    private int wanderCooldown = 90;

    [SerializeField]
    private float maxRadius = 25f;

    private Vector3 wanderForce;
    //private Vector3 inicio;

    private new void Start() {
        base.Start();
        wanderForce = Vector3.zero;
    }
    
    override
    public Steering getSteering() {
        Steering steering = new Steering();

       /* if (npc.velocity == Vector3.zero)
        {
            npc.velocity = new Vector3(1, 0, 0);
            wanderForce = GetRandomWanderForce(npc.velocity);
        }*/

        var desiredVelocity = GetWanderForce(npc.velocity);
        desiredVelocity = desiredVelocity.normalized * maxAccel;

        steering.linear = (desiredVelocity - npc.velocity);

        if (visibleRays) drawRays(npc.position, steering.linear);

        return steering;
    }

    private Vector3 GetWanderForce(Vector3 velocity) {
       /* if (transform.position.magnitude > maxRadius)
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

        /*var displacement = new Vector3(randomPoint.x, randomPoint.y) * circleRadius;
        displacement = Quaternion.LookRotation(velocity) * displacement;

        var wanderForce = circleCenter + displacement;*/

        return Seek.getSteering(randomPoint, npc, maxAccel, visibleRays).linear;
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
