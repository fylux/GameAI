using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

    public float MaxVelocity = 3;
    public float MaxAccel = 15;
    public float MaxRotation = 50;
    public float MaxAngular = 50;

    public float orientation;
    public Vector3 position;
    public float rotation;
    public Vector3 velocity;

    protected void Start() {
        velocity = Vector3.zero;
        rotation = 0;
        orientation = transform.eulerAngles.y;
        position = transform.position;
    }

}
