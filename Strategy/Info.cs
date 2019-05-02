using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Info {


    public static Dictionary<string, Node> waypoints = new Dictionary<string, Node>();

    static Collider[] hits = new Collider[40];

    static bool dbg = false; // Para activar o desactivar los debugs

    public static void Init() { 

        GameObject mid = GameObject.Find("mid");
        GameObject top = GameObject.Find("top");
        GameObject bottom = GameObject.Find("bottom");

        GameObject upBase = GameObject.Find("upBase");
        GameObject downBase = GameObject.Find("downBase");

        waypoints.Add("mid", Map.NodeFromPosition(mid.transform.position));
        waypoints.Add("top", Map.NodeFromPosition(top.transform.position));
        waypoints.Add("bottom", Map.NodeFromPosition(bottom.transform.position));

        waypoints.Add("upFront", Map.NodeFromPosition(GameObject.Find("upFront").transform.position));
        waypoints.Add("downFront", Map.NodeFromPosition(GameObject.Find("downFront").transform.position));

        waypoints.Add("upMid", Map.NodeFromPosition(mid.transform.Find("upMid").transform.position));
        waypoints.Add("downMid", Map.NodeFromPosition(mid.transform.Find("downMid").transform.position));

        waypoints.Add("upTop", Map.NodeFromPosition(top.transform.Find("upTop").transform.position));
        waypoints.Add("downTop", Map.NodeFromPosition(top.transform.Find("downTop").transform.position));

        waypoints.Add("upBottom", Map.NodeFromPosition(bottom.transform.Find("upBottom").transform.position));
        waypoints.Add("downBottom", Map.NodeFromPosition(bottom.transform.Find("downBottom").transform.position));

        waypoints.Add("upBase", Map.NodeFromPosition(upBase.transform.position));
        waypoints.Add("downBase", Map.NodeFromPosition(downBase.transform.position));
    }

    public static Node GetWaypoint(String waypoint, Faction faction) {
        var z = new Dictionary<String, String> {
            { "bottom", faction == Faction.A ? "downBottom" : "upBottom"},
            { "mid", faction == Faction.A ? "downMid" : "upMid" },
            { "top", faction == Faction.A ? "downTop" : "upTop"},
            { "base", faction == Faction.A ? "downBase" : "upBase"},
            { "front", faction == Faction.A ? "downFront" : "upFront"}
        };
        Debug.Assert(z.ContainsKey(waypoint));
        return waypoints[z[waypoint]];
    }
 

    /*
     Obtiene la lista de unidades en un area
    */
    //Actualmente, layerMask 9 para unidades
    public static HashSet<AgentUnit> GetUnitsArea(Node tile, float areaSize) {
        int nFound = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, Map.unitsMask);
        return new HashSet<AgentUnit>(hits.Take(nFound).Select(hit => hit.GetComponent<AgentUnit>()));
    }

    public static HashSet<AgentUnit> GetUnitsFactionArea(Node tile, float areaSize, Faction fact) {
        return new HashSet<AgentUnit>(GetUnitsArea(tile, areaSize).Select(hit => hit.GetComponent<AgentUnit>()).Where(unit => unit.faction == fact));
    }

    //CUIDADO: Puede devolver null si no hay unidades en ese rango
    public static AgentUnit SelectClosestUnit(Node tile, float areaSize, Faction fact) {
        return GetUnitsFactionArea(tile, areaSize, fact).OrderBy(unit => Util.NodeDistance(tile, Map.NodeFromPosition(unit.position))).FirstOrDefault();
    }

    public static AgentUnit SelectClosestUnit(Node tile, float areaSize) {
        return GetUnitsArea(tile, areaSize).OrderBy(unit => Util.NodeDistance(tile, Map.NodeFromPosition(unit.position))).FirstOrDefault();
    }

    // Obtiene el numero de uniades aliadas que siguen esa estrategia en un area
    public static int StrategyFollowersArea(Node tile, float areaSize, Faction faction, StrategyT strat) {
        return GetUnitsFactionArea(tile, areaSize, faction).Count(unit => unit.strategy == strat); ;
    }

    public static HashSet<AgentUnit> UnitsNearBase(Faction baseFaction, Faction unitsFaction, float areaSize) {
        Node nodo = GetWaypoint("base", baseFaction);
        return GetUnitsFactionArea(nodo, areaSize, unitsFaction); 
    }

    // Obtiene un numero que indica la ventaja militar en un area
    public static float MilitaryAdvantageArea(Node tile, float areaSize, Faction fact) {
        return MilitaryAdvantage(GetUnitsArea(tile, areaSize), fact);
    }

    public static float MilitaryAdvantage(HashSet<AgentUnit> units, Faction fact) {
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
        
        if (dbg) Debug.Log("Numero de unidades de A: " + number[0] + ", y de B: " + number[1]);
        /* Debug.Log("HP de A: " + HP[0] + ", y de B: " + HP[1]);
         Debug.Log("ATK de A: " + ATK[0] + ", y de B: " + ATK[1]);
         Debug.Log("Melees de A: " + melee[0] + ", rangeds: " + ranged[0] + ", scouts: " + scouts[0] + ", y artilleria: " + artill[0]);
         Debug.Log("Melees de B: " + melee[1] + ", rangeds: " + ranged[1] + ", scouts: " + scouts[1] + ", y artilleria: " + artill[1]);*/

        if (dbg) Debug.Log("La ventaja gracias a las tablas de A es de " + adv[0] + ", y la de B es " + adv[1]);

        int i = (int)fact;
        int j = 1 - i;
        if (number[i] == 0) return 0;
        if (number[j] == 0) return 50000;
        float result = Mathf.Sqrt(HP[i] / HP[j] * (ATK[i] + adv[i]) / (ATK[j] + adv[j]));

        if (dbg) Debug.Log("La ventaja total de "+fact.ToString()+" es de :" + result);

        return result;
    }

    public static float GetAvgAdvantage(Dictionary<UnitT, Vector2> unitGroups, int factionIndex) {
        float adv = 0f;
        float nTotalUnits = 0;
        foreach (UnitT type in Enum.GetValues(typeof(UnitT))) {
            float nUnits = unitGroups[UnitT.MELEE][factionIndex];
            adv += AgentUnit.atkTable[(int)UnitT.MELEE, (int)UnitT.MELEE] * nUnits;
            nTotalUnits += nUnits;
        }
        if (nTotalUnits == 0) return 50000;
        return adv / nTotalUnits;
    }

    public static Body GetClosestHealingPoint(Vector3 position, float areaSize) {
        int nFound = Physics.OverlapSphereNonAlloc(position, areaSize, hits, Map.healingMask);
        var healingPoints = hits.Take(nFound).Select(hit => hit.GetComponent<Body>());
        Debug.Assert(healingPoints.Count() > 0);
        return healingPoints.OrderBy(hPt => Util.HorizontalDist(position, hPt.position)).FirstOrDefault();
    }

    //Funciones que trabajan con influencia:

    //Devuelve un valor entre 0 y 1 que representa el porcentaje
    public static float GetMapInfluence(Faction fac) {
        return GetNodesInfluence(fac, Map.grid.Cast<Node>().ToList());
    }

    public static float GetAreaInfluence(Faction fac, Node node, float areaSize) {
       return GetNodesInfluence(fac, GetNodesInArea(node, areaSize));
    }

    public static float GetNodesInfluence(Faction fac, List<Node> nodes) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in nodes) {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    static float PathInfluence(Faction fac, List<Node> path) {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in path) {
            infl[nodo.getFaction()]++;
        }

        return ((float)infl[fac] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }


    /*public float GetWaypointInfluence(Vector3 position, Faction faction) {
        return GetAreaInfluence(faction, Map.NodeFromPosition(position));
    }*/


    public static List<Node> GetNodesInArea(Node node, float areaSize) {
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

    public static List<HashSet<AgentUnit>> GetClusters(Faction faction) {
        List<HashSet<AgentUnit>> clusters = new List<HashSet<AgentUnit>>();

        var enemies = GetUnitsFactionArea(GetWaypoint("base", faction), 45f, Util.OppositeFaction(faction));

        while (enemies.Count > 0) {
            HashSet<AgentUnit> cluster = new HashSet<AgentUnit>();
            Stack<AgentUnit> neighbours = new Stack<AgentUnit>();
            neighbours.Push(enemies.First());
            cluster.Add(enemies.First());
            enemies.Remove(enemies.First());

            while (neighbours.Count > 0) {
                AgentUnit currentEnemy = neighbours.Pop();
                var nearEnemies = Physics.OverlapSphere(currentEnemy.position, 5f, Map.unitsMask).Select(coll => coll.GetComponent<AgentUnit>()).Intersect(enemies);
                foreach (AgentUnit nearEnemy in nearEnemies) {
                    Debug.DrawLine(currentEnemy.position, nearEnemy.position);
                    neighbours.Push(nearEnemy);
                    enemies.Remove(nearEnemy);
                    cluster.Add(nearEnemy);
                }
            }
            clusters.Add(cluster);
        }
        return clusters;
    }

    public static Vector3 GetClusterCenter(HashSet<AgentUnit> cluster) {
        return cluster.Aggregate(new Vector3(0, 0, 0), (center, unit) => center + unit.position) / cluster.Count;
    }

    public static Dictionary<StrategyT, float> GetStrategyPriority(AgentUnit unit, Faction faction) {
        return new Dictionary<StrategyT, float> {
            { StrategyT.ATK_BASE,
                Util.HorizontalDist(unit.position, GetWaypoint("base", Util.OppositeFaction(faction)).worldPosition) },
            { StrategyT.ATK_HALF,
                Util.HorizontalDist(unit.position, GetWaypoint("front", faction).worldPosition) },
            { StrategyT.DEF_HALF,
                Util.HorizontalDist(unit.position, GetWaypoint(StrategyLayer.chosenWaypoint, faction).worldPosition) },
            { StrategyT.DEF_BASE,
                Util.HorizontalDist(unit.position,GetWaypoint("base", faction).worldPosition) }
        };
    }
}
