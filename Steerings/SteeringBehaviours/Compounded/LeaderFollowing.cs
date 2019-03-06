using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderFollowing : SteeringBehaviourTarget
{
    [SerializeField]
    float leaderDistance = 2f;

    Vector3 tv;
    Vector3 behind;

    float slowingRadius = 10f;

    [SerializeField]
    float threshold;

    [SerializeField]
    float decayCoefficient;

    [SerializeField]
    float arrivePriority;
    [SerializeField]
    float separationPriority;
    [SerializeField]
    float evadePriority;

    new void Start() {
        base.Start();
        tv = target.velocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.position + tv;
    }

    void Update() {
        tv = target.velocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.position + tv;
        DrawRays();
    }

    override
    public Steering GetSteering() {
        return GetSteering(target, npc, behind, slowingRadius, maxAccel, threshold, decayCoefficient, visibleRays, arrivePriority, separationPriority, evadePriority);
    }

    public static Steering GetSteering(Agent target, Agent npc, Vector3 behind, float slowingRadius, float maxAccel, float threshold, float decayCoefficient, bool visibleRays, float arrivePriority, float separationPriority, float evadePriority)
    {
        Steering steering = new Steering();
        steering.linear += Arrive.GetSteering(behind, npc, slowingRadius, maxAccel).linear * arrivePriority;
        steering.linear += Separation.GetSteering(npc, threshold, decayCoefficient, maxAccel, visibleRays).linear * separationPriority;
        if (OnLeaderSight(target))
            steering.linear += Evade.GetSteering(target, npc, maxAccel, Vector3.Distance(target.position, npc.position) * maxAccel, true).linear * evadePriority;

        return steering;
    }

    static bool OnLeaderSight(Agent target) {
        RaycastHit hit;
        int layerMask = 1 << 9;
        if (Physics.Raycast(target.position + (target.getRight() * 0.47f), target.getForward(), out hit, 10f, layerMask)) {
            Debug.Log("HIT");
            return true;
        }
        else if (Physics.Raycast(target.position + (target.getRight() * (-0.47f)), target.getForward(), out hit, 10f, layerMask)) {
            Debug.Log("HIT");
            return true;
        }
        return false;
    }

    
    void DrawRays() {
      /*  var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);*/
        Debug.DrawRay(target.transform.position + (target.getRight() * 0.47f), target.getForward() * 10, Color.yellow);
        Debug.DrawRay(target.transform.position + (target.getRight() * (-0.47f)), target.getForward() * 10, Color.yellow);
    }
    /*
    private void OnDrawGizmos() {
        Gizmos.DrawSphere(behind, 0.5f);
    }*/

}
