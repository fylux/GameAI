using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : SteeringBehaviourTarget {

    //Tener en cuenta que aquí el target es el enemigo del que nos escondemos

    [SerializeField]
    float distanceBoundary = 0.6f;

    [SerializeField]
    float faceTimeToTarget = 0.1f;

    [SerializeField]
    float maxDist;

    [SerializeField]
    float evadePrediction;

    private static Vector3 GetHidingPosition(Agent obstacle, Vector3 targetPosition, float distanceBoundary)
    {
        float distAway = obstacle.interiorRadius + distanceBoundary;

        Vector3 direction = obstacle.position - targetPosition;  //La direccion entre el enemigo del que nos queremos esconder y el muro
        direction.Normalize();

        return obstacle.position + direction * distAway;
    }

    public override Steering GetSteering()
    {
        return Hide.GetSteering(target, npc, maxDist, distanceBoundary, maxAccel, evadePrediction, faceTimeToTarget);
    }

    public static Steering GetSteering(Agent target, Agent npc, float maxDist, float distanceBoundary, float maxAccel, float evadePrediction, float faceTimeToTarget )
    {
        float minDist = maxDist;
        Vector3 bestHidingSpot = Vector3.zero;
        bool changed = false;

        int layerMask = 1 << 8;
        Collider[] hits = Physics.OverlapSphere(npc.position, minDist + distanceBoundary + 0.5f, layerMask);
        foreach (Collider coll in hits)
        {
            Vector3 hidingSpot = Hide.GetHidingPosition(coll.GetComponent<Agent>(), target.position, distanceBoundary);
            float distance = Vector3.Distance(hidingSpot, npc.position);
            if (distance < minDist)
            {
                minDist = distance;
                bestHidingSpot = hidingSpot;
                changed = true;
                Debug.Log("Escondite encontrado!");
            }
        }

        if (changed == false)
        {
            Debug.Log("HUIR");
            return Evade.GetSteering(target, npc, maxAccel, evadePrediction, true);
        }
        else
        {
            return Arrive.GetSteering(bestHidingSpot, npc, npc.exteriorRadius, maxAccel);
        }
    }
}
