using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class AgentUnit : AgentNPC {
    Map map;
    Location path_target;

    [SerializeField]
    const float maxHealth = 10f;
    float health;
    public Faction faction = Faction.A;

    public GameObject SelectCircle;

    
    protected Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() { //Coste por defecto, para casos de prueba
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };


    new
    protected void Start() {
        base.Start();
        map = GameObject.Find("Terrain").GetComponent<Map>();
        path_target = null;
        health = MaxLife;
    }

    override
    protected void ApplyActuator()
    {// Aqui el Actuator suma los steerings, los aplica a las velocidades, y las limita, teniendo en cuenta los costes
        NodeT node = map.NodeFromPosition(position).type;
        float tCost = cost[node];

        if (tCost == Mathf.Infinity)
            tCost = 1;

        Steering steering = ApplySteering();

        velocity += steering.linear * Time.deltaTime;
        rotation += steering.angular * Time.deltaTime;

        velocity.y = 0;

        velocity = Vector3.ClampMagnitude(velocity/tCost, (float)MaxVelocity / tCost);
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

        task = new GoToLRTA(this, targetPosition, (bool sucess) => {
            Debug.Log("Task finished");
            task.Terminate();
            task = null;
            StopMoving();
        });
    }

    /*public void Attack(AgentUnit unit) {
        float damage;
        if (Random.Range(0,100) > 99f) {
            damage = attack * 5;
        }
        else {
            damage = attack * Random.Range(-0.8f, 1.2f) * factorTable;
        }
        Console.singleton.Log("Unit caused " + unit.ModifyHealth(-damage) +" damage to Unit");
    }

    public float ModifyHealth(float amount) {
        float damage = amount - defense;
        health = Mathf.Min(health + damage, maxHealth);
        if (health < 0.0f) {
            Console.singleton.Log("Unit died");
        }
        if (health == maxHealth) {
            Console.singleton.Log("Unit has maximun health");
        }
        return damage;
    }*/

    public Dictionary<NodeT, float> Cost {
        get { return cost; }
    }

    public float MaxLife {
        get { return maxHealth; }
    }
    public float Life {
        get { return health; }
    }

    public float GetDropOff(float locationDistance) {
        //100f is 100% influence
        return 60f / locationDistance;
    }
}
