using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering {
    public Vector3 linear;
    public float angular;

    public Steering() {
        linear = Vector3.zero;
        angular = 0.0f;
    }

    public static Steering operator -(Steering steering) {
        steering.linear = -steering.linear;
        steering.angular = -steering.angular;
        return steering;
    }
}
