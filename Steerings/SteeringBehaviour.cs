using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour {

    protected Body body;
    public float MaxAccel = 3;
    public float blendPriority = 1;

    [SerializeField]
    protected bool visibleRays;

    public abstract Steering Steer(Vector3 velocity);

    protected void Start()
    {
        body = GetComponent<Body>();
    }

}
