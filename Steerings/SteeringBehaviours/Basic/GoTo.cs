using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTo : SteeringBehaviour {

    public Vector3 target;
    public float orientation;

    public bool active = true;

    public void Init(Vector3 target, float orient)
    {
        this.target = target;
        orientation = orient;
    }

    public override Steering GetSteering() {
        if (!active)
            return new Steering();
        Steering force = Arrive.GetSteering(target, npc, npc.exteriorRadius, maxAccel) + Align.GetSteering(orientation, npc, npc.interiorAngle, npc.exteriorAngle, 0.1f, visibleRays);
      /*  if (Util.HorizontalDistance(target,npc.position) <= 0.1f && Mathf.Abs(orientation - npc.orientation) <= 1.1f)
            GoalReached();*/

        return force;
    }

    public static Steering GetSteering(Vector3 target, Agent npc, float slowingRadius, float maxAccel) {
       return Arrive.GetSteering(target, npc, npc.exteriorRadius, maxAccel);
    }

  /*  void GoalReached()
    {
        Debug.Log("GOAL REACHED");
        AgentNPC npc = gameObject.GetComponent<AgentNPC>();
        active = false;
    }*/
}
