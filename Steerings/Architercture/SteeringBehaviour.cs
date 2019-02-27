using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour {

    protected Agent npc;

    [SerializeField]
    protected float maxAccel = 3;

    [SerializeField]
    public float blendPriority = 1;

    [SerializeField]
    protected bool visibleRays;


    public abstract Steering GetSteering();

    protected void Start() {
        npc = GetComponent<Agent>();
    }

    protected static void drawRays(Vector3 position, Vector3 ray) {
        drawRays(position, ray, Color.green);
    }

    protected static void drawRays(Vector3 position, Vector3 ray, Color color) {
        Debug.DrawRay(position, ray.normalized * 2, color);
    }
}
