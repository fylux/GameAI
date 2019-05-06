using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : Body {

    public float interiorRadius;
    public float exteriorRadius;

    public float interiorAngle;
    public float exteriorAngle;

    public void DrawGizmoInteriorRadius()
    {
        
    }


    ///
    public void SetColor(Color color) {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.material.color = color;
    }

    public void SetRenderer(bool enabled) {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = enabled;
    }
}
