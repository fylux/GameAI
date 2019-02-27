using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : SteeringBehaviourTarget
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

    private new void Start() {
        base.Start();
        targetVelocity = target.velocity;
       // targetVelocity = target.GetComponent<Rigidbody>().velocity;
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.position + tv;
    }

    private void Update() {
        tv = targetVelocity * -1;
        tv = tv.normalized * leaderDistance;
        behind = target.position + tv;
    }

    override
    public Steering GetSteering() {
        Steering steering = new Steering();
        steering.linear = FollowLeader(npc.velocity);

        return steering;
    }

    private Vector3 FollowLeader(Vector3 velocity) {
        Vector3 force = new Vector3();

        force += Arrive.GetSteering(target, npc, slowingRadius, maxAccel).linear;
        force += Separation.GetSteering(npc, GameObject.FindGameObjectsWithTag("NPC"), this.gameObject, threshold, decayCoefficient, maxAccel).linear;
        if (OnLeaderSight())
            force += Evade.GetSteering(target,npc,maxAccel, Vector3.Distance(target.transform.position, transform.position) * maxAccel,true).linear;
        force.y = 0;
        return force;
    }

    private bool OnLeaderSight() {
        RaycastHit hit;
        if (Physics.Raycast(target.transform.position + (target.transform.right * 0.47f), target.transform.TransformDirection(Vector3.forward), out hit, 10f)) {

        }
        else if (Physics.Raycast(target.transform.position + (target.transform.right * (-0.47f)), target.transform.TransformDirection(Vector3.forward), out hit, 10f)) {
            
        }
        return false;
    }


    private void DrawRays(Vector3 dv) {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
        Debug.DrawRay(target.transform.position + (target.transform.right * 0.47f), target.transform.forward * 10, Color.yellow);
        Debug.DrawRay(target.transform.position + (target.transform.right * (-0.47f)), target.transform.forward * 10, Color.yellow);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(behind, 0.5f);
    }

}
