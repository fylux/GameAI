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

        HashSet<AgentUnit> near = info.UnitsNearBase(faction, enemFac, 25);
        HashSet<AgentUnit> mid = info.UnitsNearBase(faction, enemFac, 40);
        HashSet<AgentUnit> far = info.UnitsNearBase(faction, enemFac, 55);

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

        return 0;
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
