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
        velocity = Vector3.zero;
        rotation = 0;
        orientation = transform.eulerAngles.y;
        position = transform.position;
    }

    public Vector3 getForward() {
        return Util.rotateVector(Vector3.forward, orientation).normalized;
    }

    public Vector3 getRight() {
        return Util.rotateVector(Vector3.right, orientation).normalized;
    }

    protected void Movement(Vector3 newVelocity, float newRotation)
    {
        position += velocity * Time.deltaTime;
        orientation += rotation * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(newVelocity, MaxVelocity);
        rotation = Mathf.Clamp(newRotation, -MaxRotation, MaxRotation);

        transform.position = position;
        transform.eulerAngles = new Vector3(0, orientation, 0);
    }

    private void OnDrawGizmos() {
        /*Debug.DrawLine(position, position + getForward());
        Debug.DrawLine(position, position + getRight());*/
    }


}
