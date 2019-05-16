using System.Collections.Generic;
using UnityEngine;

public enum UnitT {
    MELEE, RANGED, SCOUT, ARTIL
};

public abstract class AgentUnit : AgentNPC {
    //Location path_target;

    public StrategyT strategy;
    public Faction faction = Faction.A;
    public GameObject selectCircle;
    public float attackRange = 5f;

    public GameObject hat;

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
            {1,     1.5f,   2 ,     1   }, //Melee
            {1,     0.8f,   1.5f,   0.5f}, //Ranged
            {0.75f, 1.25f,  1,      0.5f}, //Scout
            { 0.8f, 1.25f,  1.5f,   1   }  //Artill
        };

    public abstract UnitT[] GetPreferredEnemies();
    

    abstract public UnitT GetUnitType();

	public StrategyManager stratManager;

    //Start might be called twice
    new
    public void Start() {
        base.Start();
        //path_target = null;

		if (faction == Faction.A)
			stratManager = GameObject.Find ("downBase").GetComponent<StrategyManager> ();
		else
			stratManager = GameObject.Find ("upBase").GetComponent<StrategyManager> ();

        militar.SetAgent(this);




        //TODO Hasta que se corrija que todo dios tiene select circle
        selectCircle = null;
    }


    override
    protected void ApplyActuator()
    {// Aqui el Actuator suma los steerings, los aplica a las velocidades, y las limita, teniendo en cuenta los costes
        NodeT node = Map.NodeFromPosition(position).type;
        float tCost = cost[node];

        if (tCost == Mathf.Infinity) //Ignore not walkable terrains
            tCost = 1;

        Steering steering = ApplySteering();
		MaxAngular += Face.GetSteering(position + velocity, this, interiorAngle, exteriorAngle, 0.1f, true).angular;

        velocity += steering.linear * Time.deltaTime / tCost;
		rotation +=  steering.angular * Time.deltaTime / tCost;
        velocity.y = 0;

        velocity = Vector3.ClampMagnitude(velocity, (float)MaxVelocity / tCost);
        rotation = Mathf.Clamp(rotation, -MaxRotation, MaxRotation);

        Debug.DrawRay(position, velocity.normalized * 2, Color.green);
    }

    /*override
    public Steering PathSteering() {
        if (path_target == null)
            return new Steering();

        return Seek.GetSteering(path_target.position, this, 5, visibleRays);   
    }*/

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

    public void SetTarget(Vector3 targetPosition) {
        ResetTask();

        if (faction == Faction.A) {
            Debug.Log("Aggresive");
            SetTask(new GoToAggresive(this, targetPosition, 7f, (bool sucess) => {
                Debug.Log("Aggresive terminate");
                ResetTask();
            }));
        }
        else {
            Debug.Log("Passive");
            SetTask(new GoTo(this, targetPosition, (bool sucess) => {
                Debug.Log("Passive finished");
                ResetTask();
            }));
        }


    }

    public void AttackEnemy(AgentUnit enemy) {
        ResetTask();

        SetTask(new Attack(this, enemy, (bool sucess) => {
            Debug.Log("Task finished");
            ResetTask();
        }));
        /* SetTask(new DefendZone(this, position, 6f, (bool sucess) => {
            Debug.Log("Defend finished");
            RemoveTask();
        }));*/
    }


    public Dictionary<NodeT, float> Cost {
        get { return cost; }
    }

    public float GetDropOff(float locationDistance) {
        //100f is 100% influence
        // return 775f / (locationDistance*locationDistance);
        return 200 / locationDistance;
    }

}
