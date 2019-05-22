using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidUnits : SteeringBehaviour{
    override
    public Steering GetSteering() {
        return GetSteering(npc, maxAccel, visibleRays);
    }

    public static Steering GetSteering(Agent npc, float maxAccel, bool visibleRays = false) {
        Steering steering = new Steering();
        foreach (var unit in Info.GetUnitsArea(npc.position, 0.4f)) {
            Vector3 direction = npc.position - unit.position;
            float distance = direction.magnitude;
            if (npc != unit) {
                if (AngleDir(new Vector2(npc.velocity.x, npc.velocity.z), new Vector2(-direction.x, -direction.z)) > 0f) {
                    direction = Quaternion.Euler(0, 45, 0) * direction.normalized;
                } else {
                    direction = Quaternion.Euler(0, -45, 0) * direction.normalized;
                }
                steering.linear += maxAccel * direction;
            }
        }
        if (steering.linear.magnitude > 0) {
            //Debug.DrawRay(npc.position, steering.linear.normalized, Color.blue);
        }

        return steering;
    }

    //Positive means (left?) and negative means (right?)
    static float AngleDir(Vector2 A, Vector2 B) {
        return -A.x * B.y + A.y * B.x;
    }
}
