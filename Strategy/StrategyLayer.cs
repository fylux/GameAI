using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Strategy
{
    DEFBASE, DEFHALF, ATKBASE, ATKHALF
};

public class StrategyLayer : MonoBehaviour {

    Dictionary<Strategy, float> weights = new Dictionary<Strategy, float>() { { Strategy.DEFBASE, 0 },
                                                                              { Strategy.DEFHALF, 0 },
                                                                              { Strategy.ATKBASE, 0 },
                                                                              { Strategy.ATKHALF, 0 } };

    InfoManager info;

    [SerializeField]
    Faction faction;

    // Use this for initialization
    void Start () {
        info = GetComponent<InfoManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.C))
            weights = UpdateWeights();
	}

    Dictionary<Strategy, float> UpdateWeights()
    {
        Dictionary<Strategy, float> weights = new Dictionary<Strategy, float>() { { Strategy.DEFBASE, 0 },
                                                                              { Strategy.DEFHALF, 0 },
                                                                              { Strategy.ATKBASE, 0 },
                                                                              { Strategy.ATKHALF, 0 } };

        weights[Strategy.DEFBASE] = WeightDefbase();
        weights[Strategy.DEFHALF] = WeightDefhalf();
        weights[Strategy.ATKHALF] = WeightAtkhalf();
        weights[Strategy.ATKBASE] = WeightAtkbase();


        return weights;
    }

    float WeightDefbase()
    {
        // Desde el centro de la base, un radio de 55 cubre todo el territorio
        // 40 sería un "casi llegando a la base"
        // 25 sería que están en la misma base
        Faction enemFac;

        if (faction == Faction.A)
            enemFac = Faction.B;
        else
            enemFac = Faction.A;

        int layerMask = 1 << 9;

        HashSet<AgentUnit> near = info.UnitsNearBase(faction, enemFac, 25, layerMask);
        HashSet<AgentUnit> mid = info.UnitsNearBase(faction, enemFac, 40, layerMask);
        HashSet<AgentUnit> far = info.UnitsNearBase(faction, enemFac, 55, layerMask);

        far.ExceptWith(mid);
        far.ExceptWith(near);

        mid.ExceptWith(near);

        foreach(AgentUnit unit in near)
        {
            Debug.Log("Cerca --> " + unit);
        }
        foreach (AgentUnit unit in mid)
        {
            Debug.Log("Medio --> " + unit);
        }
        foreach (AgentUnit unit in far)
        {
            Debug.Log("Lejos --> " + unit);
        }

        float result = 0;

        result += (1 / 20) * near.Count; //Dado que si todas las unidades enemigas están en la base, tendría una prioridad de 100%
        result += (0.75f / 20) * mid.Count; //Cada unidad a media distancia cuenta 0.75 de una unidad en la base
        result += (0.4f / 20) * far.Count; //Cada unidad en la mitad del mapa cuenta 0.4 de como si estuviese en la base

        return result;
    }

    float WeightDefhalf()
    {
        return 0;
    }

    float WeightAtkhalf()
    {
        return 0;
    }

    float WeightAtkbase()
    {
        return 0;
    }



}
