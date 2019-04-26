using System.Collections.Generic;
using UnityEngine;

public enum UnitT {
    MELEE, RANGED, SCOUT, ARTIL
};

public abstract class AgentUnit : AgentNPC {
    Location path_target;

    public StrategyT strategy;
    public Faction faction = Faction.A;
    public GameObject SelectCircle;

    public MilitarComponent militar = new MilitarComponent();


    protected Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() { //Coste por defecto, para casos de prueba
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

    public static float[,] atkTable = new float[4, 4] {
            //Melee Ranged  Scout   Artill
            {1,     1.5f,   2 ,     1 	},
            {1, 	0.8f, 	1.5f, 	0.5f},
            {0.75f,	1.25f, 	1, 		0.5f},
            { 0.8f, 1.25f, 	1.5f, 	1	}
        };


    new
    protected void Start() {
        base.Start();
        path_target = null;
        militar.SetAgent(this);
    }

    abstract public UnitT GetUnitType();

    override
    protected void ApplyActuator()
    {// Aqui el Actuator suma los steerings, los aplica a las velocidades, y las limita, teniendo en cuenta los costes
        NodeT node = Map.NodeFromPosition(position).type;
        float tCost = cost[node];

        if (tCost == Mathf.Infinity) //Ignore not walkable terrains
            tCost = 1;

        Steering steering = ApplySteering();

        velocity += steering.linear * Time.deltaTime / tCost;
        rotation += steering.angular * Time.deltaTime / tCost;
        velocity.y = 0;

        velocity = Vector3.ClampMagnitude(velocity, (float)MaxVelocity / tCost);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);

        Debug.DrawRay(position, velocity.normalized * 2, Color.green);
    }

    override
    public Steering PathSteering() {
        if (path_target == null)
            return new Steering();

        return Seek.GetSteering(path_target.position, this, 5, visibleRays);   
    }

    public void SetFormation(Vector3 position, float orientation) {
        /*GoTo go = gameObject.GetComponent<GoTo>();
        if (go == null) {
            go = gameObject.AddComponent<GoTo>();
            go.Init(position, orientation);
            steers.Add(go);
        } else {
            go.target = position;
            go.orientation = orientation;
            go.active = true;
        }*/
    }

    public void SetTarget(Vector3 targetPosition) {
        if (task != null)
            task.Terminate();

        task = new GoTo(this, targetPosition, (bool sucess) => {
            Debug.Log("Task finished");
            task.Terminate();
            task = null;
        });
    }

    public void AttackEnemy(AgentUnit enemy) {
        if (task != null)
            task.Terminate();

        /*task = new Attack(this, enemy, (bool sucess) => {
            Debug.Log("Task finished");
            task.Terminate();
            task = null;
        });*/
        task = new DefendZone(this, position, 6f, (bool sucess) => {
            Debug.Log("Defend finished");
            task.Terminate();
            task = null;
        });
    }

    public void SetTask(Task new_task) {
        if (task != null)
            task.Terminate();

        //Apart from the callback logic we need to Terminate it and set it to null
        var callback = new_task.GetCallback();
        callback += (bool success) => {
                task.Terminate();
                task = null;
            };
        new_task.SetCallback(callback);

        task = new_task;
    }


    public Dictionary<NodeT, float> Cost {
        get { return cost; }
    }

    public float GetDropOff(float locationDistance) {
        //100f is 100% influence
        return 60f / locationDistance;
    }
}
