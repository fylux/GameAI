using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StrategyManager : MonoBehaviour {

    [SerializeField]
    Faction faction;

    [SerializeField]
    float offensiveFactor;

    StrategyLayer strategyLayer;
    MilitaryResourcesAllocator militaryResourceAllocator;
    InfoManager info;

    Dictionary<StrategyT, SchedulerStrategy> strategySchedulers = new Dictionary<StrategyT, SchedulerStrategy>() {
                                                                              { StrategyT.DEF_BASE, new SchedulerDefBase() },
                                                                              { StrategyT.DEF_HALF, new SchedulerDefHalf() },
                                                                              { StrategyT.ATK_BASE, new SchedulerAtkBase() },
                                                                              { StrategyT.ATK_HALF, new SchedulerDefBase() } };

    // Use this for initialization
    void Start () {
        info = InfoManager.instance;
        info.Initialize();
        strategyLayer = new StrategyLayer(faction);

        foreach (KeyValuePair<StrategyT, SchedulerStrategy> tuple in strategySchedulers)
        {
            tuple.Value.Initialize(info.waypoints["allyBase"], info.waypoints["enemyBase"]);
        }

        militaryResourceAllocator = new MilitaryResourcesAllocator(faction);
        militaryResourceAllocator.SetOffensiveFactor(offensiveFactor);
	}

    // Update is called once per frame
    void Update() {
        if (Time.frameCount % 60 == 0) {
            //Layer1
            if (strategyLayer.Apply()) { 
                Debug.Log("HAN CAMBIADO LOS VALORES DE ESTRATEGIA, REASIGNANDO TROPAS");
                //Layer 2
               // militaryResourceAllocator.SetPriority(strategyLayer.GetPriority()); DESACTIVAR MIENTRAS ESTEMOS HACIENDO PRUEBAS
                Dictionary<StrategyT, HashSet<AgentUnit>> unitsToStrategy = militaryResourceAllocator.AllocateResources();

                foreach (var strategy in unitsToStrategy.Keys) {
                    strategySchedulers[strategy].usableUnits = unitsToStrategy[strategy];
                }
            }
        }

        //Layer 3
        foreach (StrategyT strategy in strategySchedulers.Keys) {
          /*  Debug.Log("Miembros recibiendo ordenes de la estrategia " + strategy);
            foreach (AgentUnit unit in strategySchedulers[strategy].usableUnits)
                Debug.Log("------> " + unit);
            */
            strategySchedulers[strategy].ApplyStrategy();
        }

        //Layer 4
        //Each unit will just apply its assigned task
    }
}
