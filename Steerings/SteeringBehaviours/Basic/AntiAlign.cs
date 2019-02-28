using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAlign : SteeringBehaviour
{
    [SerializeField]
    private Agent target;

  /*  [SerializeField]
    private float targetRadius;

    [SerializeField]
    private float slowRadius;*/

    [SerializeField]
    private float timeToTarget = 0.1f;

    override
	public Steering GetSteering()
    {
        return Align.GetSteering(target.orientation + 180.0f, npc, npc.interiorAngle, npc.exteriorAngle, timeToTarget);
    }

    public static Steering GetSteering(Agent target, Agent npc, float targetRadius, float slowRadius, float timeToTarget)
    {
        return Align.GetSteering(target.orientation + 180.0f, npc, targetRadius, slowRadius, timeToTarget);
    }


}
