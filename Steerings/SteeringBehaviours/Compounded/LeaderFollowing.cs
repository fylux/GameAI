using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderFollowing : SteeringBehaviourTarget
{
    [SerializeField]
    private float leaderDistance = 2f;

    private Vector3 tv;
    private Vector3 targetVelocity;
    private Vector3 behind;

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

    private new void Start() {
        base.Start();
        targetVelocity = target.velocity;
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.position + tv;
    }

    private void Update() {
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.position + tv;
        DrawRays();
    }

    override
    public Steering GetSteering() {
        Steering steering = new Steering();
        steering.linear += Arrive.GetSteering(behind, npc, slowingRadius, maxAccel).linear * arrivePriority;
        steering.linear += Separation.GetSteering(npc, threshold, decayCoefficient, maxAccel).linear * separationPriority;
        if (OnLeaderSight())
            steering.linear += Evade.GetSteering(target, npc, maxAccel, Vector3.Distance(target.position, npc.position) * maxAccel, true).linear * evadePriority;

        return steering;
    }

     /*Vector3 FollowLeader(Vector3 velocity) {
        Vector3 force = new Vector3();

        force += Arrive.GetSteering(behind, npc, slowingRadius, maxAccel).linear * arrivePriority; 
        force += Separation.GetSteering(npc, threshold, decayCoefficient, maxAccel).linear * separationPriority;
        if (OnLeaderSight())
            force += Evade.GetSteering(target, npc,maxAccel, Vector3.Distance(target.position, npc.position) * maxAccel,true).linear * evadePriority;
        force.y = 0;
        return force;
    }*/

    private bool OnLeaderSight() {
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

    
    private void DrawRays() {
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
