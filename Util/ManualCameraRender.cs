using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ManualCameraRender : MonoBehaviour {
    private float nextActionTime = 0.0f;
    public float period = 1f;
    Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    void Update() {
        if (Time.time > nextActionTime) {
            cam.Render();
            nextActionTime += period;
        }
    }
}