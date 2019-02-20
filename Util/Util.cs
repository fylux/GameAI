using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {

	public static Vector3 rotateVector(Vector3 v, float angle) {
        angle = -angle * Mathf.PI / 180f;
        float px = v.x * Mathf.Cos(angle) - v.z * Mathf.Sin(angle);
        float pz = v.x * Mathf.Sin(angle) + v.z * Mathf.Cos(angle);
        return new Vector3(px, v.y, pz);
        //return Quaternion.Euler(0, angle, 0) * v;
    }
}
