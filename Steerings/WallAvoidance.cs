using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AvoidanceRay {
    public Vector3 startPoint;
    public Vector3 direction;
    public float length;
    
    public AvoidanceRay(Vector3 startPoint, Vector3 direction) {
        this.startPoint = startPoint;
        this.direction = direction;
        length = direction.magnitude;
    }
}

public class WallAvoidance : SteeringBehaviour
{
    //MaxAccel is usless

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float obstacleMaxDistance = 10, avoidDistance = 1f, whiskerSeparation = 0.35f;

    private Vector3 desiredVelocity = Vector3.zero;

    override
    public Steering Steer()
    {
        Steering steering = new Steering();

        Vector3 target = Vector3.zero;

        Vector3 leftRay = body.position + body.getRight() * whiskerSeparation/2f;
        Vector3 rightRay = body.position - body.getRight() * whiskerSeparation/2f;
        Vector3 centerRay = body.position;

        AvoidanceRay[] rays = { new AvoidanceRay(leftRay, Util.rotateVector(body.velocity.normalized,30) * obstacleMaxDistance/2.2f),
                                new AvoidanceRay(rightRay, Util.rotateVector(body.velocity.normalized,-30) * obstacleMaxDistance/2.2f),
                                new AvoidanceRay(centerRay, body.velocity.normalized * obstacleMaxDistance) };

        RaycastHit hitInfo;

        bool rayHit = false;
        foreach (AvoidanceRay ray in rays) {
            if (Physics.Raycast(ray.startPoint, ray.direction, out hitInfo, ray.length, layerMask) && !rayHit) {
                target = hitInfo.normal * avoidDistance + hitInfo.point;
                if (visibleRays) Debug.DrawLine(ray.startPoint, hitInfo.point, Color.red);
                
                rayHit = true;
            }
            else if (visibleRays) {
                Debug.DrawRay(ray.startPoint, ray.direction.normalized * ray.length, Color.green);
            }
        }

       /* if (avoidanceForce != Vector3.zero) {
            Seek seek = new Seek(target);
            steering.lineal = seek.Steer();
        }*/
        return steering;
    }
}
