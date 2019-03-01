using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : SteeringBehaviour {

    /*[SerializeField]
    private float targetRadius;

    [SerializeField]
    private float slowRadius;*/

    [SerializeField]
    private float timeToTarget;

    [SerializeField]
    private float offset;   //Distancia desde el personaje hasta el circulo

    [SerializeField]
    private float radius;   //Radio del circulo

    [SerializeField]
    private float wanderRate;   //Máxima variación en la trayectoria del errante

    [SerializeField]
    private int wanderCooldown = 90;

    private float wanderOrientation; //Orientacion del personaje

    private Steering wanderForce;

    private new void Start() {
        base.Start();
        wanderOrientation = Random.Range(-1.0f, 1.0f) * wanderRate;
        wanderForce = GetRandomWanderForce();
    }

    public override Steering GetSteering() {
        if (Time.frameCount % wanderCooldown == 0) {
            wanderForce = GetRandomWanderForce();
        }

        if (visibleRays)
            drawRays(npc.position, Util.OrientationToVector(wanderForce.angular), Color.magenta);
        Debug.Log(wanderForce.angular);
        return wanderForce;
    }

    private Steering GetRandomWanderForce() {
        Steering steering = new Steering();

        wanderOrientation += Random.Range(-1.0f, 1.0f) * wanderRate;
        float targetOrientation = wanderOrientation + npc.orientation;

        Vector3 centroCirculo = npc.position + offset * Util.OrientationToVector(npc.orientation);
        Vector3 target = centroCirculo + radius * Util.OrientationToVector(targetOrientation);


        steering = Face.GetSteering(target, npc, npc.interiorAngle, npc.exteriorAngle, timeToTarget, false);

        steering.linear = maxAccel * npc.getForward();

        return steering;
    }





    /* wanderOrientation = npc.orientation;
         float rand = ((float)Random.Range(0, 10000)) / 10000;
         wanderOrientation += rand * wanderRate;

         float targetOrientation = wanderOrientation + npc.orientation;
         //Calculamos el centro del circulo
         Vector3 centroCirculo = npc.position + offset * npc.getForward();

         //Obtenemos la orientacion del objetivo en forma de vector
         Vector3 targetOrientationV = Util.rotateVector(npc.getForward(), wanderOrientation).normalized;

         Vector3 target = centroCirculo + radius * targetOrientationV;

         steering = Face.GetSteering(target, npc, targetRadius, slowRadius, timeToTarget);

         steering.linear = maxAccel * npc.getForward();*/
}
