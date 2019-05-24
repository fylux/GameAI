using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ManualCameraRender : MonoBehaviour {
    private float nextActionTime = 0.0f;
    public float period = 1f;
    Camera cam;
    public static ManualCameraRender singleton;

    void Start() {
        cam = GetComponent<Camera>();
        cam.enabled = false;
        singleton = this;
    }

    public void Draw() {
        cam.Render();
    }

    /*void Update() {
        if (Time.fixedTime > nextActionTime) {
            cam.Render();
            nextActionTime += period;
        }
    }*/
}