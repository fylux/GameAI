using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : SteeringBehaviour {

    [SerializeField]
    protected SeekT seekT = SeekT.REYNOLDS;
    [SerializeField]
    private float arrivalRadius = 2f;

    private List<Vector3> path;
    private int currentPoint;

    new private void Start() {
        base.Start();
    }

    override
    public Steering getSteering() {
        return getSteering(path, ref currentPoint, arrivalRadius, npc, maxAccel, visibleRays, seekT);
    }

    public static Steering getSteering(List<Vector3> path, ref int currentPoint, float arrivalRadius, Body npc, float maxAccel, bool visibleRays, SeekT seekT) {
        float distance = Vector3.Distance(path[currentPoint], npc.position);
        if (distance < arrivalRadius)
            currentPoint = Mathf.Min(currentPoint + 1, path.Count - 1);

        return Seek.getSteering(path[currentPoint], npc, maxAccel, visibleRays, seekT);
    }

}
