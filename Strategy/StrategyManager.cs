using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StrategyManager : MonoBehaviour {

    [SerializeField]
    Faction faction;

    public bool test; //TESTGGG para permitir las pruebas con cambios de estrategia

    [SerializeField]
    float offensiveFactor;

    StrategyLayer strategyLayer;
    public MilitaryResourcesAllocator militaryResourceAllocator;

    Dictionary<StrategyT, SchedulerStrategy> strategySchedulers = new Dictionary<StrategyT, SchedulerStrategy>() {
                                                                              { StrategyT.DEF_BASE, new SchedulerDefBase() },
                                                                              { StrategyT.DEF_HALF, new SchedulerDefHalf() },
                                                                              { StrategyT.ATK_BASE, new SchedulerAtkBase() },
                                                                              { StrategyT.ATK_HALF, new SchedulerDefBase() } };

    // Use this for initialization
    void Start () {
        strategyLayer = new StrategyLayer(faction);

        foreach (StrategyT strategy in strategySchedulers.Keys) {
            strategySchedulers[strategy].Initialize(faction);
        }

        militaryResourceAllocator = new MilitaryResourcesAllocator(faction);
        militaryResourceAllocator.SetOffensiveFactor(offensiveFactor);
	}

    // Update is called once per frame
    void Update() {
        if (Time.frameCount % 120 == 0) {
            //Layer1
            if (strategyLayer.Apply() || test == true) {
                test = false; // TESTGGG todo lo relacionado con la variable test se eliminará
                Debug.Log("HAN CAMBIADO LOS VALORES DE ESTRATEGIA, REASIGNANDO TROPAS");
                //Layer 2
               // militaryResourceAllocator.SetPriority(strategyLayer.GetPriority()); DESACTIVAR MIENTRAS ESTEMOS HACIENDO PRUEBAS
                Dictionary<StrategyT, HashSet<AgentUnit>> unitsToStrategy = militaryResourceAllocator.AllocateResources();

                foreach (var strategy in unitsToStrategy.Keys) {
                    strategySchedulers[strategy].Reset();
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
