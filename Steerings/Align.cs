using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehaviour
{

    // Hay un atributo de la rotacion en la componente transform de un personaje.
    // En la segunda parte podremos usar los steerings que queramos.
    // Los steerings lo que devuelven todos son fuerzas. Tanto angulares como lineares. 
    
        // Tenemos un personaje con una cierta rotación (que será un vector). ¿Cómo obtenemos esa rotación inicial?. Queremos que mire hacia un personaje
        // Necesitamos sacar el ángulo necesario para mirar a ese personaje desde nuestro vector (angulo entre dos vectores)
        // Y una vez tengamos el ángulo, ¿cómo giramos?

        // orientacion es a donde mira y rotacion es la velocidad angular.
        // actualizar rotacion, usando Rotate. Tendremos que hacer un quaternion unitario, y girarlo. Funciones translate y rotate
        // translate y rotate usan velocidad (lienal o angular) y deltaTime. Tacha eso, no se usan ni translate ni rotate.
        // Tendriamos que coger la posicion (u orient), transform.position = p (p = p0 + v*t), transform. 
        // Con rotacion, QuaternionUnitario, y luego Rotate(omega), siendo omega = omegaCero + (w*t)

    public GameObject target;

    [SerializeField]
    float targetRadius;

    [SerializeField]
    float slowRadius;

    [SerializeField]
    float timeToTarget = 0.1f;

    override
	public Steering Steer(Vector3 velocity)
    {
        Steering steering = new Steering();

        float orienTarget = target.GetComponent<Body>().orientation;

        float rotacion = orienTarget - body.orientation;

        rotacion = MapToRange(rotacion);
        float rotationSize = Mathf.Abs(rotacion);

        if (rotationSize < targetRadius)
            return steering;

        float targetRotation;
        if (rotationSize > slowRadius)
            targetRotation = body.MaxRotation;
        else
            targetRotation = body.MaxRotation * rotationSize / slowRadius;

        targetRotation *= rotacion / rotationSize;

        steering.angular = targetRotation - body.rotation;
        steering.angular /= timeToTarget;

        float angularAccel = Mathf.Abs(steering.angular);

        if (angularAccel > body.MaxAngular)
        {
            steering.angular /= angularAccel;
            steering.angular *= body.MaxAngular;
        }

        return steering;



    }

    private float MapToRange (float rotation)
    {
        rotation %= 360.0f;

        if (Mathf.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f)
                rotation += 360.0f;
            else
                rotation -= 360.0f;
        }
        return rotation;
    }
}
