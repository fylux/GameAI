using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour {

    [SerializeField]
    protected Body body;

    [SerializeField]
    protected float MaxAccel = 3;

    [SerializeField]
    public float blendPriority = 1;

    [SerializeField]
    protected bool visibleRays;


    public abstract Steering Steer();

    protected void Start() {
        body = GetComponent<Body>();
    }

}
