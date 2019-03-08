using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : SteeringBehaviour {

    [SerializeField]
    float arrivalRadius = 0.4f;

    public Vector3[] path;
    int currentPoint;

    new
    void Start() {
        base.Start();
        currentPoint = 0;
    }

    override
    public Steering GetSteering() {
        return getSteering(path, ref currentPoint, arrivalRadius, npc, maxAccel, visibleRays);
    }

    public static Steering getSteering(Vector3[] path, ref int currentPoint, float arrivalRadius, Agent npc, float maxAccel, bool visibleRays) {
        float distance = Util.HorizontalDistance(path[currentPoint], npc.position);
        if (distance < arrivalRadius) {
            currentPoint = Mathf.Min(currentPoint + 1, path.Length - 1);
        }

        return Seek.GetSteering(path[currentPoint], npc, maxAccel /*50*/, visibleRays);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Debug.DrawLine(npc.position, path[currentPoint] + Vector3.up, Color.yellow);
        for (int i = currentPoint; i < path.Length; ++i) {
            if (i + 1 < path.Length)
                Debug.DrawLine(path[i] + Vector3.up, path[i+1] + Vector3.up, Color.white);

            Gizmos.DrawSphere(path[i] + Vector3.up, 0.23f);
        }
 
    }

}
