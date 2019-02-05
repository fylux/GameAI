using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacles : Steering
{
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float boundingSphereRadius = 1;

    [SerializeField]
    private float obstacleMaxDistance = 10;

    [SerializeField]
    private float maxFloorAngle = 45;

    private Vector3 desiredVelocity = Vector3.zero;


    public override Vector3 Steer(Vector3 velocity)
    {
        Vector3 avoidanceForce = Vector3.zero;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        // Calculate avoidance force
        if (Physics.SphereCast(ray, boundingSphereRadius, out hitInfo, obstacleMaxDistance, layerMask)) {
            /*if (Vector3.Angle(hitInfo.normal, transform.up) > maxFloorAngle)
            {*/
            avoidanceForce = Vector3.Reflect(transform.forward, hitInfo.normal);
                /*if (Vector3.Dot(avoidanceForce, transform.forward) < -0.9f)
                {
                    avoidanceForce = transform.right;
                }*/
            //}
        }

        if (avoidanceForce != Vector3.zero) {
            desiredVelocity = (avoidanceForce).normalized * MaxVelocity ;
 
            if (visibleRays) drawRays(desiredVelocity) ;
            return (desiredVelocity - velocity) * 1f / hitInfo.distance;
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
