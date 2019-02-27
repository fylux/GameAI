using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : SteeringBehaviour {

    [SerializeField]
    float targetRadius;

    [SerializeField]
    float slowRadius;

    [SerializeField]
    float timeToTarget;

    [SerializeField]
    float offset;   //Distancia desde el personaje hasta el circulo

    [SerializeField]
    float radius;   //Radio del circulo

    [SerializeField]
    float wanderRate;   //Máxima variación en la trayectoria del errante

    float wanderOrientation; //Orientacion del personaje

    [SerializeField]
    int wanderCooldown = 90;

    Steering wanderForce;

    private new void Start()
    {
        base.Start();
        float wanderOrientation = Random.Range(-1.0f, 1.0f) * wanderRate;
        wanderForce = GetRandomWanderForce();
    }

    public override Steering GetSteering()
    {
        
        if (Time.frameCount % wanderCooldown == 0)
        {
            wanderForce = GetRandomWanderForce();
        }

        return wanderForce;
    }

    private Steering GetRandomWanderForce()
    {
        Steering steering = new Steering();

        wanderOrientation += Random.Range(-1.0f, 1.0f) * wanderRate;
        float targetOrientation = wanderOrientation + npc.orientation;

        Vector3 centroCirculo = npc.position + offset * Util.OrientationVector(npc.orientation);
        Vector3 target = centroCirculo + radius * Util.OrientationVector(targetOrientation);


        steering = Face.GetSteering(target, npc, targetRadius, slowRadius, timeToTarget);

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
