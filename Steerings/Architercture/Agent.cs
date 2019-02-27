using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : Body {

    [SerializeField]
    private float interiorRadius;
    [SerializeField]
    private float exteriorRadius;

    [SerializeField]
    private float interiorAngle;
    [SerializeField]
    private float exteriorAngle;

    public void DrawGizmoInteriorRadius()
    {
        
    }
}
