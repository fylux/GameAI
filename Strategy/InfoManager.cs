using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class InfoManager : MonoBehaviour {

    [SerializeField]
    GameObject mid, top, bottom;

    [SerializeField]
    Faction faction;

    [SerializeField]
    float areaSize, sphereSize;

    public Vector3 position;

    [SerializeField]
    Body allyBase, enemyBase;

    Collider[] hits = new Collider[40];

    LayerMask layerMaskUnit;

    void Start() {
        layerMaskUnit = LayerMask.NameToLayer("Unit");
    }

    /* private void OnDrawGizmosSelected()
     {
         Gizmos.color = Color.yellow;
         Gizmos.DrawSphere(allyBase.position, sphereSize);
     }*/

    void Update() {

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
    public HashSet<AgentUnit> GetUnitsArea(Node tile, float areaSize) {
        HashSet<AgentUnit> units = new HashSet<AgentUnit>();

        int found = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, layerMaskUnit);

        foreach (Collider hit in hits) {
            AgentUnit agent = hit.GetComponent<AgentUnit>();
            units.Add(agent);
            Debug.Log("Encontrada una unidad: " + agent);
        }

        return units;
    }

    public HashSet<AgentUnit> GetUnitsArea(Node tile) {
        return GetUnitsArea(tile, areaSize);
    }

    // Obtiene el numero de uniades aliadas que siguen esa estrategia en un area
    public int StrategyFollowersArea(Node tile, Strategy strat) {
        return GetUnitsArea(tile).Count(unit => unit.strategy == strat && unit.faction == faction); ;
    }

    public HashSet<AgentUnit> UnitsNearBase(Faction baseFaction, Faction unitsFaction, float areaSize) {
        Node nodo;

        if (faction == Faction.A)
            nodo = Map.NodeFromPosition(allyBase.position);
        else
            nodo = Map.NodeFromPosition(enemyBase.position);

        HashSet<AgentUnit> units = GetUnitsArea(nodo, areaSize);
        HashSet<AgentUnit> unitsFound = new HashSet<AgentUnit>(units.Where(u => u.faction == unitsFaction));

        foreach (AgentUnit unit in unitsFound) {
            unitsFound.Union(GetUnitsArea(Map.NodeFromPosition(unit.position), 5)
                            .Where(u => u.faction == unitsFaction));
        }

        return unitsFound;
    }

    // Obtiene un numero que indica la ventaja militar en un area
    public void MilitaryAdvantage(Node tile) {
        HashSet<AgentUnit> units = GetUnitsArea(tile);

        Vector2 number = new Vector2(0, 0);
        Vector2 avgHP = new Vector2(0, 0);
        Vector2 avgATK = new Vector2(0, 0);
        Vector2 melee = new Vector2(0, 0);
        Vector2 ranged = new Vector2(0, 0);
        Vector2 scouts = new Vector2(0, 0);
        Vector2 artill = new Vector2(0, 0);

        foreach (AgentUnit unit in units) {
            int i = unit.faction == Faction.A ? 0 : 1;

            number[i]++;
            avgHP[i] += unit.health;
            avgATK[i] += 1; // TODO Modificar cuando se añada como atributo
            if (unit is Melee) melee[i]++;
            else if (unit is Ranged) ranged[i]++;
            else if (unit is Scout) scouts[i]++;
            else if (unit is Artillery) artill[i]++;
        }
        for (int i = 0; i <= 1; ++i) {
            avgHP[0] /= number[0];
            avgATK[0] /= number[0];
        }

        Debug.Log("Numero de unidades de A: " + number[0] + ", y de B: " + number[1]);
        Debug.Log("HP average de A: " + avgHP[0] + ", y de B: " + avgHP[1]);
        Debug.Log("ATK average de A: " + avgATK[0] + ", y de B: " + avgATK[1]);
        Debug.Log("Melees de A: " + melee[0] + ", rangeds: " + ranged[0] + ", scouts: " + scouts[0] + ", y artilleria: " + artill[0]);
        Debug.Log("Melees de B: " + melee[1] + ", rangeds: " + ranged[1] + ", scouts: " + scouts[1] + ", y artilleria: " + artill[1]);

        //Sacar una formula para calcular ese numero a devolver en base a los datos
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
                Debug.Log("Añadido el nodo " + Map.grid[i, j]);
            }
        }

        return nodes;
    }
}
