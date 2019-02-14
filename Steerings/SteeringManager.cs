using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager : MonoBehaviour {

    public float MaxVelocity = 3;
    public float MaxForce = 15;

    protected Vector3 velocity;
    //   public Transform target;
    SteeringBehaviour[] steers;

    [SerializeField]
    float smooth;

   // protected Rigidbody rigid;

    private void Start()
    {
        velocity = Vector3.zero;
        //      rigid = GetComponent<Rigidbody>();ç
        steers = GetComponents<SteeringBehaviour>();
    }

    private void Update()
    {
           Move();
    }

    protected void Move()
    {
      /*  var steering = Vector3.zero;

        foreach (Steering steer in steers)
              steering += steer.Steer(velocity);

        steering = Vector3.ClampMagnitude(steering, MaxForce);

        velocity = Vector3.ClampMagnitude(velocity + steering, MaxVelocity);
        //transform.position += velocity * Time.deltaTime * velocity.magnitude;
        transform.position += transform.forward * Time.deltaTime * velocity.magnitude;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity.normalized), Time.deltaTime * smooth);
        //¿NO TRABAJAR CON QUATERNIONS? Probablemente haya que trabajar con ángulos y productos entre vectores
        // Habrá que trabajar con paso de vectores a radianes y de radianes a vectores

        // El personaje debería de tener un ángulo en sus atributos, que controlará su orientación. Saldrá algo raro como que
        // en cada frame el personaje vuelva a orientación 0º y gire a lo que le digamos.

        // Trabajar con Rotation Euler*/
    }
}
