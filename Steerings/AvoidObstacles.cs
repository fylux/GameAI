using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacles : Steering
{
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float obstacleMaxDistance = 10;

    private Vector3 desiredVelocity = Vector3.zero;


    public override Vector3 Steer(Vector3 velocity)
    {
        float widthCharacter = 1f;// temporal
        Vector3 avoidanceForce = Vector3.zero;
        Vector3 leftRay = transform.position - (transform.right * widthCharacter / 2) ;
        Vector3 rightRay = transform.position + (transform.right * widthCharacter / 2);
        RaycastHit hitInfo;

        if (Physics.Raycast(leftRay,transform.forward, out hitInfo, obstacleMaxDistance, layerMask))
        {
            avoidanceForce = hitInfo.normal;//Vector3.Reflect(transform.forward, hitInfo.normal);
            Debug.DrawLine(leftRay, hitInfo.point, Color.red);
        }
        else if (Physics.Raycast(rightRay, transform.forward, out hitInfo, obstacleMaxDistance, layerMask))
        {
            avoidanceForce = hitInfo.normal;
            Debug.DrawLine(rightRay, hitInfo.point, Color.red);
        }
        else {
            Debug.DrawRay(leftRay, transform.forward * obstacleMaxDistance, Color.green);
            Debug.DrawRay(rightRay, transform.forward * obstacleMaxDistance, Color.green);
        }

        if (avoidanceForce != Vector3.zero) {
            desiredVelocity = (avoidanceForce).normalized * MaxVelocity;

            if (visibleRays) drawRays(desiredVelocity);
            return (desiredVelocity - velocity);
        }
        else {
            return Vector3.zero;
        }
    }

    private void drawRays(Vector3 dv)
    {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
    }
}
