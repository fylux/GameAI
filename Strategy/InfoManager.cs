using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InfoManager : MonoBehaviour {

    Map map;

    [SerializeField]
    GameObject mid;
    [SerializeField]
    GameObject top;
    [SerializeField]
    GameObject bottom;

    [SerializeField]
    Faction faction;

    [SerializeField]
    float areaSize;

    public Vector3 position;

    [SerializeField]
    Body allyBase;

    [SerializeField]
    float sphereSize;

    [SerializeField]
    Body enemyBase;

    Collider[] hits = new Collider[40]; 

    void Start () {
        map = GameObject.Find("Terrain").GetComponent<Map>();
        
    }

   /* private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(allyBase.position, sphereSize);
    }*/

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
    public HashSet<AgentUnit> GetUnitsArea(Node tile, float areaSize, int layerMask) //Actualmente, layerMask 9 para unidades
    {
        HashSet<AgentUnit> units = new HashSet<AgentUnit>();

        int found = Physics.OverlapSphereNonAlloc(tile.worldPosition, areaSize, hits, layerMask);

        for (int i = 0; i < found; i++)
        {
            AgentUnit agent = hits[i].GetComponent<AgentUnit>();
            units.Add(agent);
            Debug.Log("Encontrada una unidad: " + agent + " .. " + agent.position);
        }

        return units;
    }

    public HashSet<AgentUnit> GetUnitsArea(Node tile, int layerMask)
    {
        return GetUnitsArea(tile, areaSize, layerMask);
    }

    // Obtiene el numero de uniades aliadas que siguen esa estrategia en un area
    public int StrategyFollowersArea(Node tile, Strategy strat, int layerMask) 
    {
        HashSet<AgentUnit> units = GetUnitsArea(tile, layerMask);
        int number = 0;

        foreach (AgentUnit unit in units)
        {
              if (unit.strategy == strat && unit.faction == faction)
              {
                number++;
              }
        }
        return number;
    }

    public HashSet<AgentUnit> UnitsNearBase(Faction baseFaction, Faction unitsFaction, float areaSize, int layerMask)
    {
        HashSet<AgentUnit> unitsFound = new HashSet<AgentUnit>();

        Node nodo;

        if (faction == Faction.A)
            nodo = map.NodeFromPosition(allyBase.position);
        else
            nodo = map.NodeFromPosition(enemyBase.position);

        HashSet<AgentUnit> units = GetUnitsArea(nodo, areaSize, layerMask);

        foreach (AgentUnit unit in units)
        {
            if (unit.faction == unitsFaction)
            {
                unitsFound.Add(unit);
            }
        }

       // Dado que vamos a tener en cuenta todas las unidades por cercanía, no hace falta tener en cuenta grupos
       /* foreach (AgentUnit unit in unitsFound)
        {
            units = GetUnitsArea(map.NodeFromPosition(unit.position), 5);
            foreach (AgentUnit un in units)
            {
                if (un.faction == unitsFaction)
                {
                    unitsFound.Add(un);
                }
            }
        }*/

        return unitsFound;
    }

    // Obtiene un numero que indica la ventaja militar en un area
    public void MilitaryAdvantage(Node tile, int layerMask)
    {
        HashSet<AgentUnit> units = GetUnitsArea(tile, layerMask);

        int numberA = 0; // Numero de unidades de cada faccion
        int numberB = 0;

        float avgHPA = 0; // Vida media de las unidades de cada bando
        float avgHPB = 0;

        float avgATKA = 0; // Ataque medio de las unidades de cada bando
        float avgATKB = 0;

        float meleeA = 0; // Numero de unidades de cada tipo
        float rangedA = 0;
        float scoutsA = 0;
        float artillA = 0;

        float meleeB = 0;
        float rangedB = 0;
        float scoutsB = 0;
        float artillB = 0;

        foreach (AgentUnit unit in units)
        {
            if (unit.faction == Faction.A)
            {
                numberA++;
                avgHPA += unit.health;
                avgATKA += 1; // TODO Modificar cuando se añada como atributo
                if (unit is Melee) meleeA++;
                else if (unit is Ranged) rangedA++;
                else if (unit is Scout) scoutsA++;
                else if (unit is Artillery) artillA++;
            }
            else
            {
                numberB++;
                avgHPB += unit.health;
                avgATKB += 1; // TODO Modificar cuando se añada como atributo
                if (unit is Melee) meleeB++;
                else if (unit is Ranged) rangedB++;
                else if (unit is Scout) scoutsB++;
                else if (unit is Artillery) artillB++;
            }
        }
        if (numberA > 0)
        {
            avgHPA /= numberA;
            avgATKA /= numberA;
        }
        
        if (numberB > 0)
        {
            avgHPB /= numberB;
            avgATKB /= numberB;
        }
        
        Debug.Log("Numero de unidades de A: " + numberA + ", y de B: " + numberB);
        Debug.Log("HP average de A: " + avgHPA + ", y de B: " + avgHPB);
        Debug.Log("ATK average de A: " + avgATKA + ", y de B: " + avgATKB);
        Debug.Log("Melees de A: " + meleeA + ", rangeds: " + rangedA + ", scouts: " + scoutsA + ", y artilleria: " + artillA);
        Debug.Log("Melees de B: " + meleeB + ", rangeds: " + rangedB + ", scouts: " + scoutsB + ", y artilleria: " + artillB);

        //Sacar una formula para calcular ese numero a devolver en base a los datos
    }

    //Funciones que trabajan con influencia:

    //Devuelve un valor entre 0 y 1 que representa el porcentaje
    public float GetMapInfluence(Faction fac)
    {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in map.grid)
        {
            infl[nodo.getFaction()]++;
        }


        if (fac == Faction.A)
            return ((float)infl[Faction.A] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
        else
            return ((float)infl[Faction.B] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    public float GetBaseInfluence(Body bs, Faction faction)
    {
        Node node = map.NodeFromPosition(bs.position);

        return GetAreaInfluence(faction, node);
    }

    public float GetWaypointInfluence(Vector3 position, Faction faction)
    {
        Node node = map.NodeFromPosition(position);

        return GetAreaInfluence(faction, node);
    }

    public float GetAreaInfluence(Faction fac, Node node)
    {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };
        List<Node> nodes = GetNodesInArea(node, areaSize);

        foreach (Node nodo in nodes)
        {
            infl[nodo.getFaction()]++;
        }


        if (fac == Faction.A)
            return ((float)infl[Faction.A] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
        else
            return ((float)infl[Faction.B] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }

    public List<Node> GetNodesInArea(Node node, float areaSize)
    {
        List<Node> nodes = new List<Node>();

        int x = node.gridX;
        int y = node.gridY;

        int xstart = Mathf.Max(0, x - (int)(areaSize/2));
        int ystart = Mathf.Max(0, y - (int)(areaSize/2));

        int xend = Mathf.Min(map.mapX, x + (int)areaSize);
        int yend = Mathf.Min(map.mapY, y + (int)areaSize);

        for (int i = xstart; i < xend; i++)
        {
            for (int j = ystart; j < yend; j++)
            {
                nodes.Add(map.grid[i, j]);
                Debug.Log("Añadido el nodo " + map.grid[i, j]);
            }
        }

        return nodes;
    }

    float PathInfluence (List<Node> path, Faction fac)
    {
        Dictionary<Faction, int> infl = new Dictionary<Faction, int>() { { Faction.A, 0 }, { Faction.B, 0 }, { Faction.C, 0 } };

        foreach (Node nodo in path)
        {
            infl[nodo.getFaction()]++;
        }


        if (fac == Faction.A)
            return ((float)infl[Faction.A] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
        else
            return ((float)infl[Faction.B] / (infl[Faction.A] + infl[Faction.B] + infl[Faction.C]));
    }
}
