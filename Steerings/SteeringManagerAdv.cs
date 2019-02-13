using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManagerAdv : MonoBehaviour {

    public float MaxVelocity = 3;
    public float MaxForce = 15;

    protected Vector3 velocity;
 //   public Transform target;
     Steering[] steers;

    [SerializeField]
    float smooth;

   // protected Rigidbody rigid;

    private void Start()
    {
        velocity = Vector3.zero;
        //      rigid = GetComponent<Rigidbody>();ç
        steers = GetComponents<Steering>();
    }

    private void Update()
    {
           Move();
    }

    protected void Move()
    {
        var steeringLineal = Vector3.zero;
        var steeringAngular = Vector3.zero;
        foreach (Steering steer in steers)
        {
            steer.Steer(velocity);
            steeringLineal += steer.vl;
            steeringAngular += steer.va;
        }
        //       steering += steer.Steer(velocity);

        steeringLineal = Vector3.ClampMagnitude(steeringLineal, MaxForce);

        velocity = Vector3.ClampMagnitude(velocity + steeringLineal, MaxVelocity);
        //transform.position += velocity * Time.deltaTime * velocity.magnitude;
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity.normalized), Time.deltaTime * smooth);
        // Las dos líneas superiores hay que modificarlas


        //¿NO TRABAJAR CON QUATERNIONS? Probablemente haya que trabajar con ángulos y productos entre vectores
        // Habrá que trabajar con paso de vectores a radianes y de radianes a vectores

        // El personaje debería de tener un ángulo en sus atributos, que controlará su orientación. Saldrá algo raro como que
        // en cada frame el personaje vuelva a orientación 0º y gire a lo que le digamos.

        // Trabajar con Rotation Euler
    }
}
