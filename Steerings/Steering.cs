using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Steering : MonoBehaviour {

    public float MaxVelocity = 3;

    public Vector3 vl;
    public Vector3 va;

    [SerializeField]
    protected bool visibleRays;

    public abstract void Steer(Vector3 velocity);

    

}
