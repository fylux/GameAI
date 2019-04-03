using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class InfoManager : MonoBehaviour {

    public Map map;

    
    public GameObject mid, top, bottom;
    public Dictionary<string, GameObject> waypoints = new Dictionary<string, GameObject>();
    public Dictionary<GameObject, Node> waypointNode = new Dictionary<GameObject,Node>();

    [SerializeField]
    Faction faction;

    [SerializeField]
    float areaSize, sphereSize;

   // public Vector3 position;

    [SerializeField]
    Body allyBase, enemyBase;

    Collider[] hits = new Collider[40]; 

    void Start () {
        map = GameObject.Find("Terrain").GetComponent<Map>();

        waypoints.Add("mid", mid);
        waypoints.Add("top", top);
        waypoints.Add("bottom", bottom);

        waypoints.Add("upMid",mid.transform.Find("UpMid").gameObject);
        waypoints.Add("downMid", mid.transform.Find("DownMid").gameObject);

        waypoints.Add("upTop", top.transform.Find("UpTop").gameObject);
        waypoints.Add("downTop", top.transform.Find("DownTop").gameObject);

        waypoints.Add("upBottom", bottom.transform.Find("UpBottom").gameObject);
        waypoints.Add("downBottom", bottom.transform.Find("DownBottom").gameObject);

        foreach (KeyValuePair<string, GameObject> entry in waypoints)
        {
            waypointNode.Add(entry.Value, map.NodeFromPosition(entry.Value.transform.position));
            Debug.Log("Añadido el waypoint " + entry.Value + " en la posicion " + map.NodeFromPosition(entry.Value.transform.position));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(waypoints["upBottom"].transform.position, sphereSize);
    }

    void Update () {

        //Node nodo = map.NodeFromPosition(position);
        //GetUnitsArea(nodo); //Para probar que funciona
        //ForcesUnitsArea(nodo);
        //Debug.Log("Seguidores de ATKBASE: " + StrategyFollowersArea(nodo, Strategy.ATKBASE));
        //Debug.Log("La influencia en el mapa es de " + GetMapInfluence(Faction.B));
        //Debug.Log("La influencia de A en la base aliada es de " + GetBaseInfluence(allyBase, Faction.A));
        //Debug.Log("La influencia de A en el Waypoint de Mid es de " + GetWaypointInfluence(mid.transform.position, Faction.A));
	}

    /*
     Obtiene la lista de unidades en un area
    */
    //Actualmente, layerMask 9 para unidades
    public HashSet<AgentUnit> GetUnitsArea(Node tile, float areaSize, int layerMask) {
        HashSet<AgentUnit> units = new HashSet<AgentUnit>();

        int found = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, layerMask);

        for (int i = 0; i < found; i++) {
            AgentUnit agent = hits[i].GetComponent<AgentUnit>();
            units.Add(agent);
          //  Debug.Log("Encontrada una unidad: " + agent);
        }

        return units;
    }

    public HashSet<AgentUnit> GetUnitsArea(Node tile, int layerMask)
    {
        return GetUnitsArea(tile, areaSize, layerMask);
    }

    public HashSet<AgentUnit> GetUnitsFactionArea(Node tile, float areaSize, int layerMask, Faction fact)
    {
        HashSet<AgentUnit> units = new HashSet<AgentUnit>();

        int found = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, layerMask);

        for (int i = 0; i < found; i++)
        {
            AgentUnit agent = hits[i].GetComponent<AgentUnit>();
          //  Debug.Log("Encontrada una unidad: " + agent);
            if (agent.faction == fact)
            {
                units.Add(agent);
                
            }   
        }

        return units;
    }

    public HashSet<AgentUnit> GetUnitsFactionArea(Node tile, int layerMask, Faction fact)
    {
        return GetUnitsFactionArea(tile, areaSize, layerMask, fact);
    }

    // Obtiene el numero de uniades aliadas que siguen esa estrategia en un area
    public int StrategyFollowersArea(Node tile, Strategy strat, int layerMask) {
        return GetUnitsArea(tile, layerMask).Count(unit => unit.strategy == strat && unit.faction == faction); ;
    }

    public HashSet<AgentUnit> UnitsNearBase(Faction baseFaction, Faction unitsFaction, float areaSize) {
        Node nodo;

        if (faction == Faction.A)
            nodo = map.NodeFromPosition(allyBase.position);
        else
            nodo = map.NodeFromPosition(enemyBase.position);

        HashSet<AgentUnit> units = GetUnitsFactionArea(nodo, areaSize, 1 << 9, unitsFaction);
       // HashSet<AgentUnit> unitsFound = new HashSet<AgentUnit>(units.Where(u => u.faction == unitsFaction));

       /* foreach (AgentUnit unit in units) { //No necesario con el enfoque de multiples distancias
            unitsFound.Union(GetUnitsArea(map.NodeFromPosition(unit.position), 5)
                            .Where(u => u.faction == unitsFaction));
        }*/

        return units;
    }

    // Obtiene un numero que indica la ventaja militar en un area
    public float MilitaryAdvantage(Node tile, float areaSize, int layerMask, Faction fact)
    {
        HashSet<AgentUnit> units = GetUnitsArea(tile, areaSize, layerMask);

        Vector2 number = new Vector2(0,0);
        Vector2 avgHP = new Vector2(0, 0);
        Vector2 avgATK = new Vector2(0, 0);
        Vector2 melee = new Vector2(0, 0);
        Vector2 ranged = new Vector2(0, 0);
        Vector2 scouts = new Vector2(0, 0);
        Vector2 artill = new Vector2(0, 0);
        Vector2 dmpf = new Vector2(0, 0); // Daño medio por frame (ignorando tablas, generalización)
        Vector2 totalHP = new Vector2(0, 0);

        foreach (AgentUnit unit in units) {
            int i = unit.faction == Faction.A ? 0 : 1;

            number[i]++;
            avgHP[i] += unit.health;
            totalHP[i] += unit.health;
            avgATK[i] += unit.attack;
            dmpf[i] += unit.attack;
            if (unit is Melee)  melee[i]++;
            else if (unit is Ranged) ranged[i]++;
            else if (unit is Scout) scouts[i]++;
            else if (unit is Artillery) artill[i]++;
        }
        for (int i = 0; i <= 1; ++i) {
            avgHP[0] /= number[0];
            avgATK[0] /= number[0];
        }

        /* Debug.Log("Numero de unidades de A: " + number[0] + ", y de B: " + number[1]);
         Debug.Log("HP average de A: " + avgHP[0] + ", y de B: " + avgHP[1]);
         Debug.Log("ATK average de A: " + avgATK[0] + ", y de B: " + avgATK[1]);
         Debug.Log("Melees de A: " + melee[0] + ", rangeds: " + ranged[0] + ", scouts: " + scouts[0] + ", y artilleria: " + artill[0]);
         Debug.Log("Melees de B: " + melee[1] + ", rangeds: " + ranged[1] + ", scouts: " + scouts[1] + ", y artilleria: " + artill[1]);*/

        //Sacar una formula para calcular ese numero a devolver en base a los datos

        Vector2 strength = new Vector2(0, 0);
        strength[0] = totalHP[0] * dmpf[0]; // Considerar usar de algun modo el daño medio y el nº de unidades
        strength[1] = totalHP[1] * dmpf[1];

        if (fact == Faction.A)
            return strength[0] - strength[1]; //Un numero positivo indica ventaja del bando A, un numero negativo indica ventaja del bando B
        else
            return strength[1] - strength[0]; //Como antes pero al reves
    }

    //Funciones que trabajan con influencia:

    //Devuelve un valor entre 0 y 1 que representa el porcentaje
    public float GetMapInfluence(Faction fac) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in map.grid) {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    public float GetAreaInfluence(Faction fac, Node node) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };
        List<Node> nodes = GetNodesInArea(node, areaSize);

        foreach (Node nodo in nodes) {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    float PathInfluence(Faction fac, List<Node> path) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in path) {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }


    public float GetBaseInfluence(Body bs, Faction faction) {
        return GetAreaInfluence(faction, map.NodeFromPosition(bs.position));
    }

    public float GetWaypointInfluence(Vector3 position, Faction faction) {
        return GetAreaInfluence(faction, map.NodeFromPosition(position));
    }


    public List<Node> GetNodesInArea(Node node, float areaSize) {
        List<Node> nodes = new List<Node>();

        int x = node.gridX;
        int y = node.gridY;

        int xstart = Mathf.Max(0, x - (int)(areaSize/2));
        int ystart = Mathf.Max(0, y - (int)(areaSize/2));

        int xend = Mathf.Min(map.mapX, x + (int)areaSize);
        int yend = Mathf.Min(map.mapY, y + (int)areaSize);

        for (int i = xstart; i < xend; i++)  {
            for (int j = ystart; j < yend; j++) {
                nodes.Add(map.grid[i, j]);
                Debug.Log("Añadido el nodo " + map.grid[i, j]);
            }
        }

        return nodes;
    }

}
