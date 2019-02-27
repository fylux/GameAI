using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {

	public static Vector3 RotateVector(Vector3 vector, float angle) {
        return Quaternion.Euler(0, angle, 0) * vector;
    }

    public static Vector3 OrientationVector (float orientation) {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f;
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f;
        return vector.normalized;
    }

}
