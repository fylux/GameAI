using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

    public float MaxVelocity = 3;
    public float maxAccel = 15;
    public float MaxRotation = 50;
    public float MaxAngular = 50;

    public float orientation;
    public Vector3 position;
    public float rotation;
    public Vector3 velocity;


    protected void Start() {
      //  velocity = Vector3.zero;
      //  rotation = 0;
        orientation = transform.eulerAngles.y;
        position = transform.position;
    }

    protected void Update() {
        position += velocity * Time.deltaTime;
        orientation += rotation * Time.deltaTime;

        UpdateForces();

        velocity = Vector3.ClampMagnitude(velocity, MaxVelocity);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);

        transform.position = position;
        transform.eulerAngles = new Vector3(0, orientation, 0);
    }

    protected virtual void UpdateForces() { }

    public Vector3 getForward() {
        return Util.RotateVector(Vector3.forward, orientation).normalized;
    }

    public Vector3 getRight() {
        return Util.RotateVector(Vector3.right, orientation).normalized;
    }

    private void OnDrawGizmos() {
        /*Debug.DrawLine(position, position + getForward());
        Debug.DrawLine(position, position + getRight());*/
    }
}
