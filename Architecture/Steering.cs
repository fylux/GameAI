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

    public static Steering operator +(Steering steering1, Steering steering2)
    {
        steering1.linear +=  steering2.linear;
        steering1.angular +=  steering2.angular;
        return steering1;
    }

    public static Steering ApplyPriority(Steering steering, float linearPriority, float angularPriority)
    {
        steering.linear *= linearPriority;
        steering.angular *= angularPriority;
        return steering;
    }

    public static Steering ApplyPriority(Steering steering, float priority)
    {
        steering.linear *= priority;
        steering.angular *= priority;
        return steering;
    }

    override
    public string ToString() {
        return linear + "|" + angular;
    }
}
