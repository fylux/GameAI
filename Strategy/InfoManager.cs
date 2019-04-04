using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class InfoManager : MonoBehaviour {

    public LayerMask unitsMask;

    public GameObject mid, top, bottom;
    public Dictionary<string, Node> waypoints = new Dictionary<string, Node>();


    [SerializeField]
    Faction faction;

    [SerializeField]
    float areaSize, sphereSize;

    public Body allyBase, enemyBase;

    Collider[] hits = new Collider[40];

    public HashSet<AgentUnit> allies;
    public HashSet<AgentUnit> enemies;

    public void Initialize ()
    { // Equivalente al Start. Se necesita coordinacion entre los Starts de StrategyLayer e InfoManager
        unitsMask = LayerMask.GetMask("Unit");

        GameObject mid = GameObject.Find("mid");
        GameObject top = GameObject.Find("top");
        GameObject bottom = GameObject.Find("bottom");

        waypoints.Add("mid", Map.NodeFromPosition(mid.transform.position));
        waypoints.Add("top", Map.NodeFromPosition(top.transform.position));
        waypoints.Add("bottom", Map.NodeFromPosition(bottom.transform.position));

        waypoints.Add("upMid", Map.NodeFromPosition(mid.transform.Find("UpMid").transform.position));
        waypoints.Add("downMid", Map.NodeFromPosition(mid.transform.Find("DownMid").transform.position));

        waypoints.Add("upTop", Map.NodeFromPosition(top.transform.Find("UpTop").transform.position));
        waypoints.Add("downTop", Map.NodeFromPosition(top.transform.Find("DownTop").transform.position));

        waypoints.Add("upBottom", Map.NodeFromPosition(bottom.transform.Find("UpBottom").transform.position));
        waypoints.Add("downBottom", Map.NodeFromPosition(bottom.transform.Find("DownBottom").transform.position));

       /* foreach (KeyValuePair<string, GameObject> entry in waypoints)
        {
            waypointNode.Add(entry.Value, Map.NodeFromPosition(entry.Value.transform.position));
            Debug.Log("Añadido el waypoint " + entry.Value + " en la posicion " + Map.NodeFromPosition(entry.Value.transform.position));
        }*/


        allies = new HashSet<AgentUnit>(); //ESTO DE AQUI ES PROVISIONAL, YA QUE EN EL JUEGO FINAL NO HABRA UNIDADES DIRECTAMENTE EN EL MAPA AL PRINCIPIO
        enemies = new HashSet<AgentUnit>();
        HashSet<AgentUnit> units = GetUnitsArea(waypoints["mid"], 80);
        foreach (AgentUnit unit in units)
        {
            if (unit.faction == faction)
            {
                allies.Add(unit);
            }
            else
                enemies.Add(unit);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(enemyBase.position, sphereSize);
    }

    void Update () {
        //Node nodo = map.NodeFromPosition(position);
        //GetUnitsArea(nodo); //Para probar que funciona
        //ForcesUnitsArea(nodo);
        //Debug.Log("Seguidores de ATKBASE: " + StrategyFollowersArea(nodo, Strategy.ATKBASE));
        //Debug.Log("La influencia en el mapa es de " + GetMapInfluence(Faction.B));
        //Debug.Log("La influencia de A en la base aliada es de " + GetBaseInfluence(allyBase, Faction.A));
        //Debug.Log("La influencia de A en el Waypoint de Mid es de " + GetWaypointInfluence(mid.transform.position, Faction.A));
        //Debug.Log("La unidad enemiga más cercana a la base aliada es " + SelectClosestUnit(map.NodeFromPosition(allyBase.position), 30));

    }

    /*
     Obtiene la lista de unidades en un area
    */
    //Actualmente, layerMask 9 para unidades
    public HashSet<AgentUnit> GetUnitsArea(Node tile, float areaSize) {
        HashSet<AgentUnit> units = new HashSet<AgentUnit>();

     //   Debug.Log("Obteniendo unidades desde el punto " + tile + " con un area de " + areaSize);
        int found = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, unitsMask);

        foreach (Collider hit in hits) {
            AgentUnit agent = hit.GetComponent<AgentUnit>();
            units.Add(agent);
     //       Debug.Log("Encontrada una unidad: " + agent);
        }

        return units;
    }

    public HashSet<AgentUnit> GetUnitsArea(Node tile)
    {
        return GetUnitsArea(tile, areaSize);
    }

    public HashSet<AgentUnit> GetUnitsFactionArea(Node tile, float areaSize, Faction fact)
    {
        HashSet<AgentUnit> units = new HashSet<AgentUnit>();

        int found = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, unitsMask);

        for (int i = 0; i < found; i++)
        {
            AgentUnit agent = hits[i].GetComponent<AgentUnit>();
            if (agent.faction == fact)
            {
                units.Add(agent);
            }   
        }

        return units;
    }

    public HashSet<AgentUnit> GetUnitsFactionArea(Node tile, Faction fact)
    {
        return GetUnitsFactionArea(tile, areaSize, fact);
    }

    //CUIDADO: Puede devolver null si no hay unidades en ese rango
    public AgentUnit SelectClosestUnit(Node tile, float areaSize, Faction fact)
    {
        HashSet<AgentUnit> units = GetUnitsFactionArea(tile, areaSize, fact);

        int minDist = 10000;
        AgentUnit closestUnit = null;
        int newDist;
        foreach (AgentUnit unit in units)
        {
            if ((newDist = Map.NodeFromPosition(unit.position).DistanceTo(tile)) < minDist)
            {
                minDist = newDist;
                closestUnit = unit;
            }
        }

        return closestUnit;
    }

    public AgentUnit SelectClosestUnit(Node tile, float areaSize)
    {
        HashSet<AgentUnit> units = GetUnitsArea(tile, areaSize);

        int minDist = 10000;
        AgentUnit closestUnit = null;
        int newDist;
        foreach (AgentUnit unit in units)
        {
            if ((newDist = Map.NodeFromPosition(unit.position).DistanceTo(tile)) < minDist)
            {
                minDist = newDist;
                closestUnit = unit;
            }
        }

        return closestUnit;
    }

    // Obtiene el numero de uniades aliadas que siguen esa estrategia en un area
    public int StrategyFollowersArea(Node tile, Strategy strat) {
        return GetUnitsArea(tile).Count(unit => unit.strategy == strat && unit.faction == faction); ;
    }

    public HashSet<AgentUnit> UnitsNearBase(Faction baseFaction, Faction unitsFaction, float areaSize) {
        Node nodo;


        if (baseFaction == Faction.A)
            nodo = Map.NodeFromPosition(allyBase.position);
        else
            nodo = Map.NodeFromPosition(enemyBase.position);

        HashSet<AgentUnit> units = GetUnitsFactionArea(nodo, areaSize, unitsFaction);
       // HashSet<AgentUnit> unitsFound = new HashSet<AgentUnit>(units.Where(u => u.faction == unitsFaction));

       /* foreach (AgentUnit unit in units) { //No necesario con el enfoque de multiples distancias
            unitsFound.Union(GetUnitsArea(map.NodeFromPosition(unit.position), 5)
                            .Where(u => u.faction == unitsFaction));
        }*/

        return units;
    }

    // Obtiene un numero que indica la ventaja militar en un area
    public float AreaMilitaryAdvantage(Node tile, float areaSize, Faction fact)
    {
        HashSet<AgentUnit> units = GetUnitsArea(tile, areaSize);

        return MilitaryAdvantage(units, fact);
    }

    public float MilitaryAdvantage(HashSet<AgentUnit> units, Faction fact)
    {
        Vector2 number = new Vector2(0, 0);
        Vector2 HP = new Vector2(0.00000000001f, 0.0000000001f); // Para evitar divisiones por cero
        Vector2 ATK = new Vector2(0.0000000001f, 0.0000000001f);
        Vector2 melee = new Vector2(0, 0);
        Vector2 ranged = new Vector2(0, 0);
        Vector2 scouts = new Vector2(0, 0);
        Vector2 artill = new Vector2(0, 0);

        foreach (AgentUnit unit in units)
        {
            int i = unit.faction == Faction.A ? 0 : 1;

            number[i]++;
            HP[i] += unit.health;
            ATK[i] += unit.attack;
            if (unit is Melee) melee[i]++;
            else if (unit is Ranged) ranged[i]++;
            else if (unit is Scout) scouts[i]++;
            else if (unit is Artillery) artill[i]++;
        }

         Debug.Log("Numero de unidades de A: " + number[0] + ", y de B: " + number[1]);
        /* Debug.Log("HP de A: " + HP[0] + ", y de B: " + HP[1]);
         Debug.Log("ATK de A: " + ATK[0] + ", y de B: " + ATK[1]);
         Debug.Log("Melees de A: " + melee[0] + ", rangeds: " + ranged[0] + ", scouts: " + scouts[0] + ", y artilleria: " + artill[0]);
         Debug.Log("Melees de B: " + melee[1] + ", rangeds: " + ranged[1] + ", scouts: " + scouts[1] + ", y artilleria: " + artill[1]);*/


        if (fact == Faction.A) // >1 indica ventaja, <1 implica desventaja
        {
            float result = Mathf.Sqrt(HP[0] / HP[1] * ATK[0] / ATK[1]); //TODO: Aplicar tablas
                                                                        //   Debug.Log("La ventaja es de " + result);
            return result;
        }
        else
        {
            float result = Mathf.Sqrt(HP[1] / HP[0] * ATK[1] / ATK[0]);
            //   Debug.Log("La ventaja es de " + result);
            return result;
        }
    }

    //Funciones que trabajan con influencia:

    //Devuelve un valor entre 0 y 1 que representa el porcentaje
    public float GetMapInfluence(Faction fac) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in Map.grid) {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    public float GetAreaInfluence(Faction fac, Node node, float areaSize)
    {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };
        List<Node> nodes = GetNodesInArea(node, areaSize);

        foreach (Node nodo in nodes)
        {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    public float GetAreaInfluence(Faction fac, List<Node> nodes)
    {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in nodes)
        {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    public float GetAreaInfluence(Faction fac, Node node) {
        return GetAreaInfluence(fac, node, areaSize);
    }

    float PathInfluence(Faction fac, List<Node> path) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in path) {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }


    public float GetBaseInfluence(Body bs, Faction faction) {
        return GetAreaInfluence(faction, Map.NodeFromPosition(bs.position));
    }

    public float GetWaypointInfluence(Vector3 position, Faction faction) {
        return GetAreaInfluence(faction, Map.NodeFromPosition(position));
    }


    public List<Node> GetNodesInArea(Node node, float areaSize) {
        List<Node> nodes = new List<Node>();

        int x = node.gridX;
        int y = node.gridY;

        int xstart = Mathf.Max(0, x - (int)(areaSize / 2));
        int ystart = Mathf.Max(0, y - (int)(areaSize / 2));

        int xend = Mathf.Min(Map.mapX, x + (int)areaSize);
        int yend = Mathf.Min(Map.mapY, y + (int)areaSize);

        for (int i = xstart; i < xend; i++) {
            for (int j = ystart; j < yend; j++) {
                nodes.Add(Map.grid[i, j]);
             //   Debug.Log("Añadido el nodo " + map.grid[i, j]);
            }
        }

        return nodes;
    }

}
