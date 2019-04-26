using System.Collections.Generic;
using UnityEngine;

public enum StrategyT {
    DEF_BASE, DEF_HALF, ATK_BASE, ATK_HALF
};

public class StrategyLayer : MonoBehaviour {

    Dictionary<StrategyT, float> importance = new Dictionary<StrategyT, float>() { { StrategyT.DEF_BASE, 0 },
                                                                              { StrategyT.DEF_HALF, 0 },
                                                                              { StrategyT.ATK_BASE, 0 },
                                                                              { StrategyT.ATK_HALF, 0 } };

    Dictionary<StrategyT, OrderAsign> orders = new Dictionary<StrategyT, OrderAsign>() {
                                                                              { StrategyT.DEF_BASE, new OrderAsignDefBase() },
                                                                              { StrategyT.DEF_HALF, new OrderAsignDefHalf() },
                                                                              { StrategyT.ATK_BASE, new OrderAsignDefBase() },
                                                                              { StrategyT.ATK_HALF, new OrderAsignDefBase() } };

    Dictionary<string, List<Node>> waypointArea;
   

    static public string chosenWaypoint = "mid";

    InfoManager info;
    MilitaryResourcesAllocator milit;

    [SerializeField]
    Faction faction;

    string mapSide;
    Faction enemFac;

    void Start()
    {
        info = GetComponent<InfoManager>();
        info.Initialize();
        if (faction == Faction.A)
        {
            enemFac = Faction.B;
            mapSide = "up";
        }
        else
        {
            enemFac = Faction.A;
            mapSide = "down";
        }
        // Dado que, más adelante, tendremos que consultar la influencia en el área alrededor de estos waypoints a cada segundo, vamos a tener guardados
        // los nodos y así nos ahorramos repetir el trabajo
        waypointArea = new Dictionary<string, List<Node>>() { { mapSide+"Mid", info.GetNodesInArea(info.waypoints[mapSide + "Mid"], 5) },
                                                              { mapSide+"Top", info.GetNodesInArea(info.waypoints[mapSide + "Top"], 5) },
                                                              { mapSide+"Bottom", info.GetNodesInArea(info.waypoints[mapSide + "Bottom"], 5) }};

        milit = GetComponent<MilitaryResourcesAllocator>();
    }

    // Update is called once per frame
    void Update () {
        if (Time.frameCount % 60 == 0)
        {
            bool changed = false;

            Dictionary<StrategyT,float> newWeights = UpdateImportance();
            foreach (KeyValuePair<StrategyT, float> entry in newWeights)
            {
                Debug.Log("El valor de la estrategia " + entry.Key + " es de " + entry.Value);
                if (Mathf.Abs(entry.Value - importance[entry.Key]) >= 0.15) //TODO: Decidir valor real. ¿Lo hacemos así o pedimos cambio estable?
                {
                    importance = newWeights;
                    changed = true;
                }
            }
            if (changed)
            {
                Debug.Log("HAN CAMBIADO LOS VALORES DE ESTRATEGIA, REASIGNANDO TROPAS");
                milit.AllocateResources();

                foreach (KeyValuePair<StrategyT, OrderAsign> tuple in orders)
                {
                    tuple.Value.usableUnits = new HashSet<AgentUnit>();
                }

                foreach (AgentUnit unit in info.allies)
                {
                    // Coger la estrategia de la unidad y añadir la unidad a la OrderAssign (diccionario) de esa estrategia
                    // Debug.Log("Añadiendo la unidad " + unit + " a las usableUnits de " + unit.strategy);
                    orders[unit.strategy].usableUnits.Add(unit);
                }
                foreach (KeyValuePair<StrategyT, OrderAsign> tuple in orders)
                {
                    Debug.Log("Miembros recibiendo ordenes de la estrategia " + tuple.Key);
                    foreach (AgentUnit unit in tuple.Value.usableUnits)
                        Debug.Log("------> " + unit);
                }
            }
                
        }
            
	}

    Dictionary<StrategyT, float> UpdateImportance() {
        float defbase = Mathf.Clamp(ImportanceDefbase(), 0, 1);
        float defhalf = Mathf.Clamp(ImportanceDefhalf(), 0, 1);

        return new Dictionary<StrategyT, float>(){
            { StrategyT.DEF_BASE, defbase },
            { StrategyT.DEF_HALF, defhalf },
            { StrategyT.ATK_HALF, 1 - (Mathf.Max(defbase,defhalf)) },
            { StrategyT.ATK_BASE, Mathf.Clamp(ImportanceAtkbase(), 0, 1) }
        };
    }

    float ImportanceDefbase()
    {
        // Desde el centro de la base, un radio de 50 cubre todo el territorio relevante
        // 40 sería un "casi llegando a la base"
        // 25 sería que están en la misma base
        Debug.Log("START DEFBASE");
        HashSet<AgentUnit> near = info.UnitsNearBase(faction, enemFac, 25);
        HashSet<AgentUnit> mid = info.UnitsNearBase(faction, enemFac, 40);
        HashSet<AgentUnit> far = info.UnitsNearBase(faction, enemFac, 50);

        far.ExceptWith(mid);
        far.ExceptWith(near);

        mid.ExceptWith(near);

        float result = 0;

        foreach(AgentUnit unit in near) {
            result += ((float)1 / 20) * 0.8f; // ¿Deberiamos considerar el "peor caso" antes, o no tiene sentido?
        }
        foreach (AgentUnit unit in mid) {
            result += ((float)1 / 20) * 0.55f;
        }
        foreach (AgentUnit unit in far) {
            result += ((float)1 / 20) * 0.3f;
        }

        HashSet<AgentUnit> units = new HashSet<AgentUnit>(info.allies);
        units.UnionWith(info.enemies); // Tenemos ahora un hashset con todas las unidades vivas
        result += Mathf.Clamp(info.MilitaryAdvantage(units, faction) - 1, -0.2f, 0.2f);
        Debug.Log("Teniendo en cuenta todas las unidades vivas, ese peso es ahora " + result);

        Debug.Log("La proximidad de unidades enemigas a la base contribuye a DEFBASE en " + result);

        return result;
    }

    float ImportanceDefhalf() {
        Debug.Log("START DEFHALF");
        float[] result = new float[3]; //0 = mid, 1 = top, 2 = bottom
        // Separamos los tres waypoints para asegurarnos de tratar el caso de cada waypoint por separado y no mezclar resultados

        // Comprobamos el numero de unidades enemigas cerca de cada waypoint. A más, más interesante será tomar esta estrategia
        // El waypoint central cuenta más, ya que es el más importante
        result[0] = ImportanceUnitsNearWaypoint("mid", 0.4f, 0.3f);
        result[1] = ImportanceUnitsNearWaypoint("top", 0.3f, 0.2f);
        result[2] = ImportanceUnitsNearWaypoint("bottom", 0.3f, 0.2f);

        // Ahora comprobamos la influencia aliada cerca del waypoint (pero no exactamente en el waypoint). Si el enemigo tiene mucha más influencia,
        // habría quizá que reconsiderar que el camino es peligroso
        result[0] -= ImportanceAllyInfluenceWaypoint("Mid"); 
        result[1] -= ImportanceAllyInfluenceWaypoint("Top");
        result[2] -= ImportanceAllyInfluenceWaypoint("Bottom");

        float area = 20;
        // En el mejor caso para nosotros, tendremos un +0.5 en mid, que significa que al menos doblamos en fuerza al enemigo. En el peor, -0.35,
        // ya que es probable que el enemigo se vea limitado por el terreno
        float inflM = Mathf.Clamp(info.AreaMilitaryAdvantage(info.waypoints["mid"], area, faction) - 1, -0.3f, 0.4f);
        // Para los otros dos waypoints, el resultado será menos relevante
        float inflT = Mathf.Clamp(info.AreaMilitaryAdvantage(info.waypoints["top"], area, faction) - 1, -0.2f, 0.3f);
        float inflB = Mathf.Clamp(info.AreaMilitaryAdvantage(info.waypoints["bottom"], area, faction) - 1, -0.2f, 0.3f);


        result[0] += inflM;
        Debug.Log("En mid, la ventaja militar de los aliados es de " + inflM);
        result[1] += inflT;
        Debug.Log("En top, la ventaja militar de los aliados es de " + inflT);
        result[2] += inflB;
        Debug.Log("En bottom, la ventaja militar de los aliados es de " + inflB);

        float maxWeight = Mathf.Max(result[0], Mathf.Max(result[1], result[2]));

        if (result[0] >= result[1] && result[0] >= result[2]) chosenWaypoint = "mid";
        else if (result[1] >= result[2]) chosenWaypoint = "top";
        else chosenWaypoint = "bottom";

        HashSet<AgentUnit> units = new HashSet<AgentUnit>(info.allies);
        units.UnionWith(info.enemies); // Tenemos ahora un hashset con todas las unidades vivas
        maxWeight += Mathf.Clamp(info.MilitaryAdvantage(units, faction) - 1, -0.2f, 0.2f);

        Debug.Log("El peso dado a DEFHALF es de " + maxWeight);

        return maxWeight;
    }

    float ImportanceUnitsNearWaypoint(string waypoint, float nearMult, float farMult)
    {
        HashSet<AgentUnit> near = info.GetUnitsFactionArea(info.waypoints[waypoint], 8, enemFac);
        HashSet<AgentUnit> far = info.GetUnitsFactionArea(info.waypoints[waypoint], 15, enemFac);

        far.ExceptWith(near);
        float result = (nearMult / 20) * near.Count + (farMult / 20) * far.Count;
        Debug.Log("El waypoint " + waypoint + " contribuye a DEFHALF en " + result + " debido a la cercanía de unidades enemigas");
        return result;
    }

    float ImportanceAllyInfluenceWaypoint(string waypoint)
    {
        Debug.Log("Vamos a buscar el waypoint" + mapSide + waypoint);
        float allyInfl = info.GetNodesInfluence(faction, waypointArea[mapSide+waypoint]);
        float enemyInfl = info.GetNodesInfluence(enemFac, waypointArea[mapSide + waypoint]);

        Debug.Log("En el waypoint " + waypoint + " hay una influencia aliada de " + allyInfl + " y una influencia enemiga de " + enemyInfl);
        Debug.Log("Por tanto el waypoint " + waypoint + " contribuye al peso debido a influencias enemigas en -" + Mathf.Min(0.2f, Mathf.Max((enemyInfl - allyInfl),0)));

        return Mathf.Min(0.2f, Mathf.Max((enemyInfl - allyInfl), 0)); // Preferimos que la influencia en nuestro lado sea nuestra
    }

    float ImportanceAtkhalf() {
        // Desde el centro de la base, un radio de 50 cubre todo el territorio relevante
        // 40 sería un "casi llegando a la base"
        // 25 sería que están en la misma base
        Debug.Log("START ATKHALF");
        HashSet<AgentUnit> near = info.UnitsNearBase(enemFac, enemFac, 25);
        HashSet<AgentUnit> mid = info.UnitsNearBase(enemFac, enemFac, 40);
        HashSet<AgentUnit> far = info.UnitsNearBase(enemFac, enemFac, 50);

        far.ExceptWith(mid);
        far.ExceptWith(near);

        mid.ExceptWith(near);

        float result = 0;

        foreach (AgentUnit unit in near)
        {
            result += ((float)1 / 20); // ¿Deberiamos considerar el "peor caso" antes, o no tiene sentido?
        }
        foreach (AgentUnit unit in mid)
        {
            result += ((float)1 / 20) * 0.75f;
        }
        foreach (AgentUnit unit in far)
        {
            result += ((float)1 / 20) * 0.5f;
        }

        Debug.Log("La proximidad de unidades enemigas en su territorio contribuye a ATKHALF en " + result);

        // Para calcular la diferencia de fuerza en los waypoints, mismo sistema que en DEFHALF
       // float area = 20;
        float inflM = Mathf.Clamp(info.AreaMilitaryAdvantage(info.waypoints["enemyBase"], 45, faction) - 1, -0.5f, 0.5f);
        /*  float inflT = Mathf.Max(Mathf.Min(info.AreaMilitaryAdvantage(info.waypoints["top"], area, faction) - 1, 0.35f), -0.25f);
          float inflB = Mathf.Max(Mathf.Min(info.AreaMilitaryAdvantage(info.waypoints["bottom"], area, faction) - 1, 0.35f), -0.25f);
          Debug.Log("En mid, la ventaja militar de los aliados es de " + inflM);
          Debug.Log("En top, la ventaja militar de los aliados es de " + inflT);
          Debug.Log("En bottom, la ventaja militar de los aliados es de " + inflB);
          result += Mathf.Max(inflM, Mathf.Max(inflT, inflB));*/
        result += inflM;

        Debug.Log("El peso dado a ATKHALF es de " + result);

        return result;
    }

    float ImportanceAtkbase() {
        Debug.Log("START ATKBASE");
        HashSet<AgentUnit> baseEnemies = info.UnitsNearBase(enemFac, enemFac, 25); //Cogemos los enemigos cercanos a la base enemiga
        baseEnemies.UnionWith(info.GetUnitsFactionArea(info.waypoints["enemyBase"], 45, faction)); // Añadimos los aliados en territorio enemigo

        float result = Mathf.Clamp(info.MilitaryAdvantage(baseEnemies, faction) - 1, -0.4f, 0.4f);
        Debug.Log("Gracias a la ventaja de las fuerzas aliadas en territorio enemigo frente a las enemigas en la base enemigo, tenemos un peso actual de " + result);

        HashSet<AgentUnit> units = new HashSet<AgentUnit>(info.allies);
        units.UnionWith(info.enemies); // Tenemos ahora un hashset con todas las unidades vivas
        result += Mathf.Clamp(info.MilitaryAdvantage(units, faction) - 1, -0.2f, 0.2f);
        Debug.Log("Teniendo en cuenta todas las unidades vivas, ese peso es ahora " + result);

        HashSet<AgentUnit> unitsInOtherHalf = info.UnitsNearBase(enemFac, faction, 40); //Unidades de un bando en territorio del otro
        unitsInOtherHalf.UnionWith(info.UnitsNearBase(faction, enemFac, 40));
        float allyAdv = info.AreaMilitaryAdvantage(info.waypoints["enemyBase"], 40, faction);
        float enemAdv = info.AreaMilitaryAdvantage(info.waypoints["allyBase"], 40, enemFac);

        result += Mathf.Clamp(allyAdv - enemAdv, -0.4f, 0.4f);
        Debug.Log("Finalmente, comparandola ventaja atacante aliada con la enemiga, el peso de ATKBASE es de " + result);

        return result;
  
    }
}
