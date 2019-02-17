using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacles : SteeringBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float obstacleMaxDistance = 10;

    private Vector3 desiredVelocity = Vector3.zero;

    override
    public  Steering Steer()
    {
        Steering steering = new Steering();

        float widthCharacter = 1f;// temporal
        Vector3 avoidanceForce = Vector3.zero;
        Vector3 leftRay = body.position - (transform.right * widthCharacter / 2) ;
        Vector3 rightRay = body.position + (transform.right * widthCharacter / 2);
        RaycastHit hitInfo;

        if (Physics.Raycast(leftRay,body.velocity, out hitInfo, obstacleMaxDistance, layerMask))
        {
            avoidanceForce = hitInfo.normal;//Vector3.Reflect(transform.forward, hitInfo.normal);
            Debug.DrawLine(leftRay, hitInfo.point, Color.red);
        }
        else if (Physics.Raycast(rightRay, body.velocity, out hitInfo, obstacleMaxDistance, layerMask))
        {
            avoidanceForce = hitInfo.normal;
            Debug.DrawLine(rightRay, hitInfo.point, Color.red);
        }
        else {
            Debug.DrawRay(leftRay, body.velocity * obstacleMaxDistance, Color.green);
            Debug.DrawRay(rightRay, body.velocity * obstacleMaxDistance, Color.green);
        }

        if (avoidanceForce != Vector3.zero) {
            desiredVelocity = (avoidanceForce).normalized * MaxAccel;
            if (visibleRays) drawRays(desiredVelocity);
            steering.lineal =(desiredVelocity - body.velocity);
        }
        else {
            steering.lineal = Vector3.zero;
        }
        return steering;
    }

    private void drawRays(Vector3 dv)
    {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
    }
}
