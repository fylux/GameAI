using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour {

    public float MaxVelocity = 3;

    [SerializeField]
    protected bool visibleRays;

    public abstract Steering Steer(Vector3 velocity);

    

}
