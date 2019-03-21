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
    protected void ApplyActuator() {
        NodeT node = map.NodeFromPosition(position).type;
        float tCost = cost[node];

        velocity = Vector3.ClampMagnitude(velocity, (float)MaxVelocity / tCost);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);
    }

    override
    public Steering PathSteering() {
        if (path_target == null)
            return new Steering();

        return Seek.GetSteering(path_target.position, this, 5, visibleRays);   
    }

    public void SetFormation(Vector3 position, float orientation) {
        GoTo go = gameObject.GetComponent<GoTo>();
        if (go == null) {
            go = gameObject.AddComponent<GoTo>();
            go.Init(position, orientation);
            steers.Add(go);
        } else {
            go.target = position;
            go.orientation = orientation;
            go.active = true;
        }
    }

    public void SetTarget(Vector3 targetPosition) {
        PathfindingManager.RequestPath(position, targetPosition, cost, GoToTarget);
        /*path_target = new Location();
        path_target.position = targetPosition;*/
    }

    void GoToTarget(Vector3[] newPath, bool pathSuccessful) {
        if (!pathSuccessful) {
            Debug.Log("Not reachable");
            return;
        }

        //Remove previous PathFollowing
        RemoveSteering(steers.Find(steer => steer is PathFollowing));

        if (newPath.Length > 0) {
            PathFollowing pf = gameObject.AddComponent<PathFollowing>();
            pf.path = newPath;
            pf.visibleRays = visibleRays;
            pf.maxAccel = 50f;
            steers.Add(pf);
        }
    }

    /*public void Attack(AgentUnit unit) {
        float damage;
        if (Random.Range(0,100) > 99f) {
            damage = attackDamage * 5;
        }
        else {
            damage = attackDamage * Random.Range(-0.8f, 1.2f) * factorTble;
        }
        unit.ModifyHealth(-damage);
    }

    public void ModifyHealth(float amount) {
        health = Mathf.Min(health + amount, maxHealth);
        if (health < 0.0f) {
            Console.singleton.Log("Unit died");
        }
        if (health == maxHealth) {
            Console.singleton.Log("Unit has maximun health");
        }
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
