using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Strategy {
    DEF_BASE, DEF_HALF, ATK_BASE, ATK_HALF
};

public class StrategyLayer : MonoBehaviour {

    Dictionary<Strategy, float> weights = new Dictionary<Strategy, float>() { { Strategy.DEF_BASE, 0 },
                                                                              { Strategy.DEF_HALF, 0 },
                                                                              { Strategy.ATK_BASE, 0 },
                                                                              { Strategy.ATK_HALF, 0 } };

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
        return 0;
    }

    float WeightAtkhalf() {
        return 0;
    }

    float WeightAtkbase() {
        return 0;
    }
}
