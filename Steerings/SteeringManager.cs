using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    UNIFORM, ACCELERATED
};

public class SteeringManager : Body {

    [SerializeField]
    private MovementType movementType = MovementType.ACCELERATED; //Unused right now

    private SteeringBehaviour[] steers;

    private new void Start()
    {
        base.Start();
        steers = GetComponents<SteeringBehaviour>();
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
            Steering steering = steer.Steer();
            steeringLineal += steering.lineal * steer.blendPriority;
            steeringAngular += steering.angular * steer.blendPriority;
        }

        steeringLineal = Vector3.ClampMagnitude(steeringLineal, MaxAccel);
        steeringAngular = Mathf.Clamp(steeringAngular,-MaxAngular, MaxAngular);

        position += velocity * Time.deltaTime;
        orientation += rotation * Time.deltaTime;
        velocity += steeringLineal * Time.deltaTime;
        rotation += steeringAngular * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, MaxVelocity);
        rotation = Mathf.Clamp(rotation,-MaxRotation, MaxRotation);

        transform.position = position;
        transform.eulerAngles = new Vector3(0, orientation, 0);
    }
}
