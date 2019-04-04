using System.Collections.Generic;
using UnityEngine;

public enum UnitClass
{
    MELEE, RANGED, SCOUT, ARTIL
};

public abstract class AgentUnit : AgentNPC {
    Location path_target;

    public Strategy strategy;

    public Faction faction = Faction.A;

    public GameObject SelectCircle;

    
    protected Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() { //Coste por defecto, para casos de prueba
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.5f },
            { NodeT.FOREST, 2 },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

    protected Dictionary<UnitClass, float> atk = new Dictionary<UnitClass, float>() { //Coste por defecto, para casos de prueba
            { UnitClass.MELEE, 1 },
            { UnitClass.RANGED, 1 },
            { UnitClass.SCOUT, 1 },
            { UnitClass.ARTIL, 1 }
        };


    new
    protected void Start() {
        base.Start();
        path_target = null;
        health = maxHealth;
    }

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

    [SerializeField]
    const int maxHealth = 10;
    public int health = 10;
    public int attack = 6;
    public int defense = 3;
    

    public void Attack(AgentUnit unit) {
        float damage;
        if (Random.Range(0,100) > 99f) {
            damage = attack * 5;
        }
        else {
            damage = attack * Random.Range(0.8f, 1.2f) /** factorTable*/;
        }
        unit.ReceiveAttack((int)Mathf.Round(damage));
    }

    public float ReceiveAttack(int amount) {
        int damage = Mathf.Max(0, amount - defense);
        Console.Log("Unit caused "+damage+" damage");
        health = health - damage;
        if (health < 0) {
            Console.Log("Unit died");
            this.gameObject.SetActive(false);
        }
        //Request to update selection text
        return damage;
    }

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
