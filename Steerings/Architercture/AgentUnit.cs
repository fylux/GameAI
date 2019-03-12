using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AgentUnit : AgentNPC {
    Map map;

    protected Dictionary<NodeT, float> cost;

    new
    protected void Start() {
        base.Start();
        GameObject terrain = GameObject.Find("Terrain");
        map = terrain.GetComponent<Map>();
    }

    override
    protected void ApplyActuator()
    {
        velocity.y = 0;

        NodeT node = map.NodeFromPosition(position).type;
        float tCost = cost[node];

        velocity = Vector3.ClampMagnitude(velocity, (float)MaxVelocity / tCost);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);
    }    
}
