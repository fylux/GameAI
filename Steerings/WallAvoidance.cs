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

public class WallAvoidance : SteeringBehaviour {

    [SerializeField]
    protected SeekT seekT = SeekT.REYNOLDS;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float obstacleMaxDist = 3, avoidDist = 3f, whiskerSeparation = 0.3f;

    override
    public Steering getSteering() {
        return getSteering(npc, maxAccel, layerMask, obstacleMaxDist, avoidDist, whiskerSeparation, visibleRays, seekT);
    }

    public static Steering getSteering(Body npc, float maxAccel, LayerMask layerMask, float obstacleMaxDist, float avoidDist, float whiskerSeparation, bool visibleRays, SeekT seekT) {
        Steering steering = new Steering();

        Vector3 target = Vector3.zero;

        Vector3 leftRay = npc.position + npc.getRight() * whiskerSeparation/2f;
        Vector3 rightRay = npc.position - npc.getRight() * whiskerSeparation/2f;
        Vector3 centerRay = npc.position;

        AvoidanceRay[] rays = { new AvoidanceRay(leftRay, Util.rotateVector(npc.velocity.normalized,30) * obstacleMaxDist/2.2f),
                                new AvoidanceRay(rightRay, Util.rotateVector(npc.velocity.normalized,-30) * obstacleMaxDist/2.2f),
                                new AvoidanceRay(centerRay, npc.velocity.normalized * obstacleMaxDist) };

        RaycastHit hitInfo;

        bool rayHit = false;
        foreach (AvoidanceRay ray in rays) {
            if (Physics.Raycast(ray.startPoint, ray.direction, out hitInfo, ray.length, layerMask) && !rayHit) {
                target = hitInfo.normal * avoidDist + hitInfo.point;
                if (visibleRays) Debug.DrawLine(ray.startPoint, hitInfo.point, Color.red);
                
                rayHit = true;
                steering = Seek.getSteering(target, npc, maxAccel, visibleRays, seekT);
            }
            else if (visibleRays) {
                Debug.DrawRay(ray.startPoint, ray.direction.normalized * ray.length, Color.yellow);
            }
        }
        return steering;
    }
}
