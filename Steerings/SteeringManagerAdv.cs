using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManagerAdv : MonoBehaviour {

    public float MaxVelocity = 3;
    public float MaxForce = 15;
    public float MaxRotation = 50;
    public float MaxAngularAccel = 50;

    public float angle;
    public Vector3 pos;

    public float rotacion;
    public Vector3 velocity;
    //   public Transform target;
    SteeringBehaviour[] steers;

    [SerializeField]
    float smooth;

   // protected Rigidbody rigid;

    private void Start()
    {
        velocity = Vector3.zero;
        //rigid = GetComponent<Rigidbody>();ç
        steers = GetComponents<SteeringBehaviour>();

        rotacion = 0;
        angle = 70;
        pos = transform.position;
        transform.eulerAngles = new Vector3(0, angle, 0);
    }

    private void Update()
    {
           Move();
    }

    protected void Move()
    {
        Vector3 steeringLineal = Vector3.zero;
        float steeringAngular = 0.0f;
        foreach (SteeringBehaviour steer in steers)
        {
            Steering steering = steer.Steer(velocity);
            steeringLineal += steering.lineal;
            steeringAngular += steering.angular;
        }

        steeringLineal = Vector3.ClampMagnitude(steeringLineal, MaxForce);
        steeringAngular = Mathf.Clamp(steeringAngular,-MaxRotation, MaxRotation);

        pos = pos + steeringLineal * Time.deltaTime;
        transform.position = pos;

        angle = angle + steeringAngular * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, angle, 0);
    }
}
