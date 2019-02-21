using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType {
    UNIFORM, ACCELERATED
};

public class SteeringManager : Body {

    [SerializeField]
    private MovementType movementType = MovementType.ACCELERATED; //Unused right now

    private SteeringBehaviour[] steers;

    private new void Start() {
        base.Start();
        steers = GetComponents<SteeringBehaviour>();
    }

    private void Update() {
           Move();
    }

    protected void Move() {
        Vector3 steeringLinear = Vector3.zero;
        float steeringAngular = 0.0f;
        foreach (SteeringBehaviour steer in steers) {
            Steering steering = steer.getSteering();
            steeringLinear += steering.linear * steer.blendPriority;
            steeringAngular += steering.angular * steer.blendPriority;
        }

        steeringLinear = Vector3.ClampMagnitude(steeringLinear, maxAccel);
        steeringAngular = Mathf.Clamp(steeringAngular, -MaxAngular, MaxAngular);

        Vector3 newVelocity = velocity + steeringLinear * Time.deltaTime;
        float newRotation = rotation + steeringAngular * Time.deltaTime;
        Movement(newVelocity, newRotation);
    }
}
