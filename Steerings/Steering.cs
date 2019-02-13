using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Steering : MonoBehaviour {

    public float MaxVelocity = 3;

    public Vector3 vl = Vector3.zero;
    public float va = 0f;

    [SerializeField]
    protected bool visibleRays;

    public abstract void Steer(Vector3 velocity);

    

}
