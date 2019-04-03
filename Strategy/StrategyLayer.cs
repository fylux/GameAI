using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Strategy {
    DEF_BASE, DEF_HALF, ATK_BASE, ATK_HALF
};

public class StrategyLayer : MonoBehaviour {

    public LayerMask unitsMask;

    Dictionary<Strategy, float> weights = new Dictionary<Strategy, float>() { { Strategy.DEF_BASE, 0 },
                                                                              { Strategy.DEF_HALF, 0 },
                                                                              { Strategy.ATK_BASE, 0 },
                                                                              { Strategy.ATK_HALF, 0 } };

    InfoManager info;

    [SerializeField]
    Faction faction;

    string mapSide;
    Faction enemFac;

    // Use this for initialization
    void Start () {
        info = GetComponent<InfoManager>();

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
            
    }
	
	// Update is called once per frame
	void Update () {
        weights = UpdateWeights();
	}

    Dictionary<Strategy, float> UpdateWeights() {
        return new Dictionary<Strategy, float>(){
            { Strategy.DEF_BASE, WeightDefbase() },
            { Strategy.DEF_HALF, WeightDefhalf() },
            { Strategy.ATK_BASE, WeightAtkhalf() },
            { Strategy.ATK_HALF, WeightAtkbase() }
        };
    }

    float WeightDefbase()
    {
        // Desde el centro de la base, un radio de 55 cubre todo el territorio
        // 40 sería un "casi llegando a la base"
        // 25 sería que están en la misma base

        HashSet<AgentUnit> near = info.UnitsNearBase(faction, enemFac, 25);
        HashSet<AgentUnit> mid = info.UnitsNearBase(faction, enemFac, 40);
        HashSet<AgentUnit> far = info.UnitsNearBase(faction, enemFac, 55);

        far.ExceptWith(mid);
        far.ExceptWith(near);

        mid.ExceptWith(near);

        foreach(AgentUnit unit in near) {
            Debug.Log("Cerca --> " + unit);
        }
        foreach (AgentUnit unit in mid) {
            Debug.Log("Medio --> " + unit);
        }
        foreach (AgentUnit unit in far) {
            Debug.Log("Lejos --> " + unit);
        }

        return 0;
    }

    float WeightDefhalf() {

        //TODO: Una a una, comprobar que funcionan las tres partes

        float[] result = new float[3]; //0 = mid, 1 = top, 2 = bottom
        // Separamos los tres waypoints para asegurarnos de tratar el caso de cada waypoint por separado y no mezclar resultados

        // Comprobamos el numero de unidades enemigas cerca de cada waypoint. A más, más interesante será tomar esta estrategia
        /*HashSet<AgentUnit> nearMid = info.GetUnitsFactionArea(info.waypointNode[info.waypoints["mid"]], 10, unitsMask, enemFac);
        HashSet<AgentUnit> farMid = info.GetUnitsFactionArea(info.waypointNode[info.waypoints["mid"]], 25, unitsMask, enemFac);

        farMid.ExceptWith(nearMid);

        HashSet<AgentUnit> nearTop = info.GetUnitsFactionArea(info.waypointNode[info.waypoints["top"]], 10, unitsMask, enemFac);
        HashSet<AgentUnit> farTop = info.GetUnitsFactionArea(info.waypointNode[info.waypoints["top"]], 25, unitsMask, enemFac);

        farTop.ExceptWith(nearTop);

        HashSet<AgentUnit> nearBottom = info.GetUnitsFactionArea(info.waypointNode[info.waypoints["bottom"]], 10, unitsMask, enemFac);
        HashSet<AgentUnit> farBottom = info.GetUnitsFactionArea(info.waypointNode[info.waypoints["bottom"]], 25, unitsMask, enemFac);

        farBottom.ExceptWith(nearBottom);*/

        // El waypoint central cuenta más, ya que es el más importante
        result[0] = WeightUnitsNearWaypoint("mid", 0.5f, 0.4f);
        result[1] = WeightUnitsNearWaypoint("top", 0.4f, 0.3f);
        result[2] = WeightUnitsNearWaypoint("bottom", 0.4f, 0.3f);
        /* result[0] = (0.5f / 20) * nearMid.Count + (0.4f / 20) * farMid.Count;
         result[1] = (0.4f / 20) * nearTop.Count + (0.3f / 20) * farTop.Count;
         result[2] = (0.4f / 20) * nearBottom.Count + (0.3f / 20) * farBottom.Count;*/

        // Ahora comprobamos la influencia aliada cerca del waypoint (pero no exactamente en el waypoint). Si el enemigo tiene mucha más influencia,
        // habría quizá que reconsiderar que el camino es peligroso
        /*float allyInfl = info.GetAreaInfluence(faction, info.waypointNode[info.waypoints[mapSide+"Mid"]]);
        float enemyInfl = info.GetAreaInfluence(enemFac, info.waypointNode[info.waypoints[mapSide+"Mid"]]);*/

  //      result[0] -= WeightAllyInfluenceWaypoint("Mid"); 
  //      result[1] -= WeightAllyInfluenceWaypoint("Top");
  //      result[2] -= WeightAllyInfluenceWaypoint("Bottom");

        /*allyInfl = info.GetAreaInfluence(faction, info.waypointNode[info.waypoints[mapSide + "Top"]]);
        enemyInfl = info.GetAreaInfluence(enemFac, info.waypointNode[info.waypoints[mapSide + "Top"]]);

        result[1] -= Mathf.Min(0.2f, enemyInfl - allyInfl);

        allyInfl = info.GetAreaInfluence(faction, info.waypointNode[info.waypoints[mapSide + "Bottom"]]);
        enemyInfl = info.GetAreaInfluence(enemFac, info.waypointNode[info.waypoints[mapSide + "Bottom"]]);

        result[2] -= Mathf.Min(0.2f, enemyInfl - allyInfl);*/

  //      float area = 40;

        // En el mejor caso para nosotros, tendremos un +0.5 en mid, que significa que al menos doblamos en fuerza al enemigo. En el peor, -0.35,
        // ya que es probable que el enemigo se vea limitado por el terreno
  //      float inflM = Mathf.Max(Mathf.Min(info.MilitaryAdvantage(info.waypointNode[info.waypoints["mid"]], area, unitsMask, faction), 0.5f),-0.35f);
        // Para los otros dos waypoints, el resultado será menos relevante
  //      float inflT = Mathf.Max(Mathf.Min(info.MilitaryAdvantage(info.waypointNode[info.waypoints["top"]], area, unitsMask, faction), 0.35f), -0.25f);
  //      float inflB = Mathf.Max(Mathf.Min(info.MilitaryAdvantage(info.waypointNode[info.waypoints["bottom"]], area, unitsMask, faction), 0.35f), -0.25f);


   //     result[0] += inflM;
   //     result[1] += inflT;
   //     result[2] += inflB;

        return 0;
    }

    float WeightUnitsNearWaypoint(string waypoint, float nearMult, float farMult)
    {

        HashSet<AgentUnit> near = info.GetUnitsFactionArea(info.waypointNode[info.waypoints[waypoint]], 10, unitsMask, enemFac);
        HashSet<AgentUnit> far = info.GetUnitsFactionArea(info.waypointNode[info.waypoints[waypoint]], 25, unitsMask, enemFac);

        far.ExceptWith(near);

        float result = (nearMult / 20) * near.Count + (farMult / 20) * far.Count;

        return result;
    }

    float WeightAllyInfluenceWaypoint(string waypoint) //TODO: Cambiar los nombres para que sean "Upmid", por ejemplo
    {
        float allyInfl = info.GetAreaInfluence(faction, info.waypointNode[info.waypoints[mapSide + waypoint]]);
        float enemyInfl = info.GetAreaInfluence(enemFac, info.waypointNode[info.waypoints[mapSide + waypoint]]);

        return Mathf.Min(0.2f, enemyInfl - allyInfl); // Preferimos que la influencia en nuestro lado sea nuestra
    }

    float WeightAtkhalf() {
        return 0;
    }

    float WeightAtkbase() {
        return 0;
    }
}
