using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {

	public static Vector3 rotateVector(Vector3 vector, float angle) {
        return Quaternion.Euler(0, angle, 0) * vector;
    }
}
