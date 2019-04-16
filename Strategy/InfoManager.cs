using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfoManager : MonoBehaviour {

    public LayerMask unitsMask;
    public LayerMask healingMask;

    public Dictionary<string, Node> waypoints = new Dictionary<string, Node>();
    public HashSet<AgentUnit> allies, enemies;

    [SerializeField]
    Faction faction;

    [SerializeField]
    float areaSize, sphereSize;

    Collider[] hits = new Collider[40];


    public void Initialize()
    { // Equivalente al Start. Se necesita coordinacion entre los Starts de StrategyLayer e InfoManager
        unitsMask = LayerMask.GetMask("Unit");

        GameObject mid = GameObject.Find("Mid");
        GameObject top = GameObject.Find("Top");
        GameObject bottom = GameObject.Find("Bottom");

        GameObject allyBase = GameObject.Find("BaseAliada");
        GameObject enemyBase = GameObject.Find("BaseEnemiga");

        waypoints.Add("mid", Map.NodeFromPosition(mid.transform.position));
        waypoints.Add("top", Map.NodeFromPosition(top.transform.position));
        waypoints.Add("bottom", Map.NodeFromPosition(bottom.transform.position));

        waypoints.Add("downFront", Map.NodeFromPosition(GameObject.Find("DownFront").transform.position));
        waypoints.Add("upFront", Map.NodeFromPosition(GameObject.Find("UpFront").transform.position));

        waypoints.Add("upMid", Map.NodeFromPosition(mid.transform.Find("UpMid").transform.position));
        waypoints.Add("downMid", Map.NodeFromPosition(mid.transform.Find("DownMid").transform.position));

        waypoints.Add("upTop", Map.NodeFromPosition(top.transform.Find("UpTop").transform.position));
        waypoints.Add("downTop", Map.NodeFromPosition(top.transform.Find("DownTop").transform.position));

        waypoints.Add("upBottom", Map.NodeFromPosition(bottom.transform.Find("UpBottom").transform.position));
        waypoints.Add("downBottom", Map.NodeFromPosition(bottom.transform.Find("DownBottom").transform.position));

        waypoints.Add("allyBase", Map.NodeFromPosition(allyBase.transform.position));
        waypoints.Add("enemyBase", Map.NodeFromPosition(enemyBase.transform.position));

        /* foreach (KeyValuePair<string, GameObject> entry in waypoints)
         {
             waypointNode.Add(entry.Value, Map.NodeFromPosition(entry.Value.transform.position));
             Debug.Log("Añadido el waypoint " + entry.Value + " en la posicion " + Map.NodeFromPosition(entry.Value.transform.position));
         }*/


        allies = new HashSet<AgentUnit>(Map.unitList.Where(agent => agent.faction == faction));
        enemies = new HashSet<AgentUnit>(Map.unitList);
        enemies.ExceptWith(allies);
    }

    private void OnDrawGizmosSelected() {
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
        int nFound = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, unitsMask);
        return new HashSet<AgentUnit>(hits.Take(nFound).Select(hit => hit.GetComponent<AgentUnit>()));
    }

    public HashSet<AgentUnit> GetUnitsArea(Node tile) {
        return GetUnitsArea(tile, areaSize);
    }

    public HashSet<AgentUnit> GetUnitsFactionArea(Node tile, float areaSize, Faction fact) {
        return new HashSet<AgentUnit>(GetUnitsArea(tile, areaSize).Select(hit => hit.GetComponent<AgentUnit>()).Where(unit => unit.faction == fact));
    }

    public HashSet<AgentUnit> GetUnitsFactionArea(Node tile, Faction fact) {
        return GetUnitsFactionArea(tile, areaSize, fact);
    }

    //CUIDADO: Puede devolver null si no hay unidades en ese rango
    public AgentUnit SelectClosestUnit(Node tile, float areaSize, Faction fact) {
        return GetUnitsFactionArea(tile, areaSize, fact).OrderBy(unit => Util.NodeDistance(tile, Map.NodeFromPosition(unit.position))).FirstOrDefault();
    }

    public AgentUnit SelectClosestUnit(Node tile, float areaSize) {
        return GetUnitsArea(tile, areaSize).OrderBy(unit => Util.NodeDistance(tile, Map.NodeFromPosition(unit.position))).FirstOrDefault();
    }

    // Obtiene el numero de uniades aliadas que siguen esa estrategia en un area
    public int StrategyFollowersArea(Node tile, StrategyT strat) {
        return GetUnitsArea(tile).Count(unit => unit.strategy == strat && unit.faction == faction); ;
    }

    public HashSet<AgentUnit> UnitsNearBase(Faction baseFaction, Faction unitsFaction, float areaSize) {
        Node nodo;

        if (baseFaction == Faction.A)
            nodo = waypoints["allyBase"];
        else
            nodo = waypoints["enemyBase"];

        return GetUnitsFactionArea(nodo, areaSize, unitsFaction); 
    }

    // Obtiene un numero que indica la ventaja militar en un area
    public float AreaMilitaryAdvantage(Node tile, float areaSize, Faction fact) {
        return MilitaryAdvantage(GetUnitsArea(tile, areaSize), fact);
    }

    public float MilitaryAdvantage(HashSet<AgentUnit> units, Faction fact) {
        Debug.Assert(fact == Faction.A || fact == Faction.B);

        Vector2 number = new Vector2(0, 0);
        Vector2 HP = new Vector2(0.00000000001f, 0.0000000001f); // Para evitar divisiones por cero
        Vector2 ATK = new Vector2(0.0000000001f, 0.0000000001f);

        Dictionary<UnitT, Vector2> unitGroups = Enum.GetValues(typeof(UnitT)).Cast<UnitT>().ToDictionary(u => u, u => new Vector2(0, 0));

        foreach (AgentUnit unit in units) {
            int index = (int)unit.faction;
            number[index]++;
            HP[index] += unit.militar.health;
            ATK[index] += unit.militar.attack;

            unitGroups[unit.GetUnitType()] += new Vector2(index, 1 - index);
        }
        Vector2 adv = new Vector2(GetAvgAdvantage(unitGroups, 1), GetAvgAdvantage(unitGroups, 0));

        Debug.Log("Numero de unidades de A: " + number[0] + ", y de B: " + number[1]);
        /* Debug.Log("HP de A: " + HP[0] + ", y de B: " + HP[1]);
         Debug.Log("ATK de A: " + ATK[0] + ", y de B: " + ATK[1]);
         Debug.Log("Melees de A: " + melee[0] + ", rangeds: " + ranged[0] + ", scouts: " + scouts[0] + ", y artilleria: " + artill[0]);
         Debug.Log("Melees de B: " + melee[1] + ", rangeds: " + ranged[1] + ", scouts: " + scouts[1] + ", y artilleria: " + artill[1]);*/

        Debug.Log("La ventaja gracias a las tablas de A es de " + adv[0] + ", y la de B es " + adv[1]);

        int i = (int)fact;
        int j = 1 - i;
        if (number[i] == 0) return 0;
        if (number[j] == 0) return Mathf.Infinity;
        float result = Mathf.Sqrt(HP[i] / HP[j] * (ATK[i] + adv[i]) / (ATK[j] + adv[j]));

        Debug.Log("La ventaja total de "+fact.ToString()+" es de :" + result);

        return result;
    }

    public float GetAvgAdvantage(Dictionary<UnitT, Vector2> unitGroups, int factionIndex) {
        float adv = 0f;
        float nTotalUnits = 0;
        foreach (UnitT type in Enum.GetValues(typeof(UnitT))) {
            float nUnits = unitGroups[UnitT.MELEE][factionIndex];
            adv += AgentUnit.atkTable[(int)UnitT.MELEE, (int)UnitT.MELEE] * nUnits;
            nTotalUnits += nUnits;
        }
        Debug.Assert(nTotalUnits > 0);
        return adv / nTotalUnits;
    }

    public List<Body> GetHealingPoints(Node tile, float areaSize) {
        int nFound = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, healingMask);
        return new List<Body>(hits.Take(nFound).Select(hit => hit.GetComponent<Body>()));
    }

    //Funciones que trabajan con influencia:

    //Devuelve un valor entre 0 y 1 que representa el porcentaje
    public float GetMapInfluence(Faction fac) {
        return GetNodesInfluence(fac, Map.grid.Cast<Node>().ToList());
    }

    public float GetAreaInfluence(Faction fac, Node node, float areaSize) {
       return GetNodesInfluence(fac, GetNodesInArea(node, areaSize));
    }

    public float GetNodesInfluence(Faction fac, List<Node> nodes) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

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

    public float GetAreaInfluence(Faction fac, Node node) {
        return GetAreaInfluence(fac, node, areaSize);
    }

    /*public float GetWaypointInfluence(Vector3 position, Faction faction) {
        return GetAreaInfluence(faction, Map.NodeFromPosition(position));
    }*/


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

    public Dictionary<StrategyT, float> GetStrategyPriority(AgentUnit unit) {
        return new Dictionary<StrategyT, float> {
            { StrategyT.ATK_BASE,
                Util.HorizontalDistance(unit.position, waypoints["enemyBase"].worldPosition) },
            { StrategyT.ATK_HALF,
                new List<String>{"mid","mid"}.Min(waypoint => Util.HorizontalDistance(unit.position, waypoints[waypoint].worldPosition)) },
            { StrategyT.DEF_HALF,
                new List<String>{"mid","mid"}.Min(waypoint => Util.HorizontalDistance(unit.position, waypoints[waypoint].worldPosition)) },
            { StrategyT.DEF_BASE,
                Util.HorizontalDistance(unit.position, waypoints["allyBase"].worldPosition) }
        };
    }
}
