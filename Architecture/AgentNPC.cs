﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentNPC : Agent {

    protected List<SteeringBehaviour> steers;

    protected Task task;
    public string taskName = "";

    [SerializeField]
    protected bool visibleRays;

    Vector3 virtualTarget;

    protected Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() { //Coste por defecto, para casos de prueba
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

    public Dictionary<NodeT, float> Cost {
        get { return cost; }
    }

    //Start might be called twice
    new
    protected void Start() {
        base.Start();
        steers = new List<SteeringBehaviour>(GetComponents<SteeringBehaviour>());

        MaxRotation = 300;
        MaxAngular = 6000;
    }


    protected Steering ApplySteering() {
        Steering totalSteering = new Steering();
        foreach (SteeringBehaviour steer in steers) {
            totalSteering += Steering.ApplyPriority(steer.GetSteering(), steer.blendPriority);
        }
        //totalSteering += PathSteering();
        if (task != null) {
            taskName = task.ToString();
            totalSteering += task.Apply();
        }
        else {
            taskName = "Idle";
        }
        totalSteering.linear.y = 0;

        totalSteering.linear = Vector3.ClampMagnitude(totalSteering.linear, MaxAccel);
        totalSteering.angular = Mathf.Clamp(totalSteering.angular, -MaxAngular, MaxAngular);

        return totalSteering;
    }

    override
    protected void ApplyActuator() { // Aqui el Actuator suma los steerings, los aplica a las velocidades, y las limita

        Steering steering = ApplySteering();

        velocity += steering.linear * Time.deltaTime;
        rotation += steering.angular * Time.deltaTime;

        velocity.y = 0;

        velocity = Vector3.ClampMagnitude(velocity, MaxVelocity);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);

        Debug.DrawRay(position, velocity.normalized * 2, Color.green);
    }

    public void RemoveSteering(SteeringBehaviour steer) {
        if (steers.Contains(steer)) {
            steers.Remove(steer);
            Destroy(steer);
        }
    }

    virtual
    public Steering PathSteering() { return new Steering(); }

    public void SetTask(Task new_task) {
        ResetTask();

        //Apart from the callback logic we need to Terminate it and set it to null
        new_task.SetCallback(((_) => { ResetTask(); }) + new_task.GetCallback());

        task = new_task;

        taskName = task.ToString();
    }

    public void ResetTask() {
        if (task != null) {
            task.Terminate();
            task = null;
            taskName = "";
        }   
    }

	public void SetFormation(Vector3 position, float orientation) {
		GoForm go = gameObject.GetComponent<GoForm>();
		if (go == null) {
			go = gameObject.AddComponent<GoForm>();
			go.Init(position, orientation);
			steers.Add(go);
		} else {
			go.target = position;
			go.orientation = orientation;
			go.active = true;
		}
	}

    public Task GetTask() {
        return task;
    }
    public bool HasTask<T>() {
        return task != null && task is T;
    }
    public bool HasTask<A, B>() {
        return task != null && (task is A || task is B);
    }
    public bool HasTask<A, B, C>() {
        return task != null && (task is A || task is B || task is C);
    }

}
