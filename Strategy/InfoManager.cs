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

    public void Initialize()
    { // Equivalente al Start. Se necesita coordinacion entre los Starts de StrategyLayer e InfoManager
        unitsMask = LayerMask.GetMask("Unit");

        GameObject mid = GameObject.Find("Mid");
        GameObject top = GameObject.Find("Top");
        GameObject bottom = GameObject.Find("Bottom");

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


        allies = new HashSet<AgentUnit>(Map.unitList.Where(agent => agent.faction == faction));
        enemies = new HashSet<AgentUnit>(Map.unitList);
        enemies.ExceptWith(allies);
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
        //Debug.Log("Obteniendo unidades desde el punto " + tile + " con un area de " + areaSize);
        Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, unitsMask);
        return new HashSet<AgentUnit>(hits.Select(hit => hit.GetComponent<AgentUnit>()));
    }

    public HashSet<AgentUnit> GetUnitsArea(Node tile) {
        return GetUnitsArea(tile, areaSize);
    }

    public HashSet<AgentUnit> GetUnitsFactionArea(Node tile, float areaSize, Faction fact) {
        Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, unitsMask);
        return new HashSet<AgentUnit>(hits.Select(hit => hit.GetComponent<AgentUnit>()).Where(unit => unit.faction == fact));
    }

    public HashSet<AgentUnit> GetUnitsFactionArea(Node tile, Faction fact) {
        return GetUnitsFactionArea(tile, areaSize, fact);
    }

    //CUIDADO: Puede devolver null si no hay unidades en ese rango
    public AgentUnit SelectClosestUnit(Node tile, float areaSize, Faction fact) {
        HashSet<AgentUnit> units = GetUnitsFactionArea(tile, areaSize, fact);
        return units.OrderBy(unit => Util.NodeDistance(tile, Map.NodeFromPosition(unit.position))).First();
    }

    public AgentUnit SelectClosestUnit(Node tile, float areaSize) {
        HashSet<AgentUnit> units = GetUnitsArea(tile, areaSize);
        return units.OrderBy(unit => Util.NodeDistance(tile, Map.NodeFromPosition(unit.position))).First();
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

        return GetUnitsFactionArea(nodo, areaSize, unitsFaction); 
    }

    // Obtiene un numero que indica la ventaja militar en un area
    public float AreaMilitaryAdvantage(Node tile, float areaSize, Faction fact) {
        return MilitaryAdvantage(GetUnitsArea(tile, areaSize), fact);
    }

    public float MilitaryAdvantage(HashSet<AgentUnit> units, Faction fact) {
        Vector2 number = new Vector2(0, 0);
        Vector2 HP = new Vector2(0.00000000001f, 0.0000000001f); // Para evitar divisiones por cero
        Vector2 ATK = new Vector2(0.0000000001f, 0.0000000001f);
        Vector2 melee = new Vector2(0, 0);
        Vector2 ranged = new Vector2(0, 0);
        Vector2 scouts = new Vector2(0, 0);
        Vector2 artill = new Vector2(0, 0);


        foreach (AgentUnit unit in units) {
            int i = unit.faction == Faction.A ? 0 : 1;

            number[i]++;
            HP[i] += unit.militar.health;
            ATK[i] += unit.militar.attack;
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


        float advA = (float)(GetAvgAdvantage(UnitT.MELEE, (int)melee[1], (int)ranged[1], (int)scouts[1], (int)artill[1]) * melee[0]
                     + GetAvgAdvantage(UnitT.RANGED, (int)melee[1], (int)ranged[1], (int)scouts[1], (int)artill[1]) * ranged[0]
                     + GetAvgAdvantage(UnitT.SCOUT, (int)melee[1], (int)ranged[1], (int)scouts[1], (int)artill[1]) * scouts[0]
                     + GetAvgAdvantage(UnitT.ARTIL, (int)melee[1], (int)ranged[1], (int)scouts[1], (int)artill[1]) * artill[0]) / number[0];
        float advB = (float)(GetAvgAdvantage(UnitT.MELEE, (int)melee[0], (int)ranged[0], (int)scouts[0], (int)artill[0]) * melee[1]
                     + GetAvgAdvantage(UnitT.RANGED, (int)melee[0], (int)ranged[0], (int)scouts[0], (int)artill[0]) * ranged[1]
                     + GetAvgAdvantage(UnitT.SCOUT, (int)melee[0], (int)ranged[0], (int)scouts[0], (int)artill[0]) * scouts[1]
                     + GetAvgAdvantage(UnitT.ARTIL, (int)melee[0], (int)ranged[0], (int)scouts[0], (int)artill[0]) * artill[1]) / number[1];

        Debug.Log("La ventaja gracias a las tablas de A es de " + advA + ", y la de B es " + advB);

        float result;

        if (fact == Faction.A) // >1 indica ventaja, <1 implica desventaja
        {
            if (number[0] == 0) return 0;
            if (number[1] == 0) return Mathf.Infinity;
            result = Mathf.Sqrt(HP[0] / HP[1] * (ATK[0] + advA) / (ATK[1] + advB));
            Debug.Log("La ventaja total de A es de :" + result);
        }
        else
        {
            if (number[0] == 0) return Mathf.Infinity;
            if (number[1] == 0) return 0;
            result = Mathf.Sqrt(HP[1] / HP[0] * (ATK[1] + advB) / (ATK[0] + advA));
            Debug.Log("La ventaja total de B es de :" + result);
        }
        return result;
    }

    public float GetAvgAdvantage(UnitT type, int melees, int rangeds, int scouts, int artills)
    {
        float result = 0;
        if (type == UnitT.MELEE)
        {
            result += Melee.atk[UnitT.MELEE] * melees;
            result += Melee.atk[UnitT.RANGED] * rangeds;
            result += Melee.atk[UnitT.SCOUT] * scouts;
            result += Melee.atk[UnitT.ARTIL] * artills;
        }
        else if (type == UnitT.RANGED)
        {
            result += Ranged.atk[UnitT.MELEE] * melees;
            result += Ranged.atk[UnitT.RANGED] * rangeds;
            result += Ranged.atk[UnitT.SCOUT] * scouts;
            result += Ranged.atk[UnitT.ARTIL] * artills;
        }
        else if (type == UnitT.SCOUT)
        {
            result += Scout.atk[UnitT.MELEE] * melees;
            result += Scout.atk[UnitT.RANGED] * rangeds;
            result += Scout.atk[UnitT.SCOUT] * scouts;
            result += Scout.atk[UnitT.ARTIL] * artills;
        }
        else if (type == UnitT.ARTIL)
        {
            result += Artillery.atk[UnitT.MELEE] * melees;
            result += Artillery.atk[UnitT.RANGED] * rangeds;
            result += Artillery.atk[UnitT.SCOUT] * scouts;
            result += Artillery.atk[UnitT.ARTIL] * artills;
        }

        return (float)result / (melees + rangeds + scouts + artills);
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

    public float GetAreaInfluence(Faction fac, Node node, float areaSize) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };
        List<Node> nodes = GetNodesInArea(node, areaSize);

        foreach (Node nodo in nodes) {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    public float GetAreaInfluence(Faction fac, List<Node> nodes) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in nodes) {
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
