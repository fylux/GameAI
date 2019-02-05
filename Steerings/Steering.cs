using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Steering : MonoBehaviour {

    public float MaxVelocity = 3;

    [SerializeField]
    protected bool visibleRays;

    public abstract Vector3 Steer(Vector3 velocity);
}
