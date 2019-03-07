using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTo : SteeringBehaviour {

    public Vector3 target;

    public bool active = true;

    public void Init(Vector3 target)
    {
        this.target = target;
    }

    public override Steering GetSteering() {
        if (!active)
            return new Steering(); //Podriamos devolver aquí 1 - npc.velocity?
        Steering force = Arrive.GetSteering(target, npc, npc.exteriorRadius, maxAccel);
        if (Util.HorizontalDistance(target,npc.position) <= 0.001f)
            GoalReached();

        return force;
    }

    public static Steering GetSteering(Vector3 target, Agent npc, float slowingRadius, float maxAccel) {
       return Arrive.GetSteering(target, npc, npc.exteriorRadius, maxAccel);
    }

    void GoalReached()
    {
        Debug.Log("GOAL REACHED");
        AgentNPC npc = gameObject.GetComponent<AgentNPC>();
        active = false;
    }
}
