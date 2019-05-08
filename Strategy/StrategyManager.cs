using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StrategyManager : MonoBehaviour {

    [SerializeField]
    Faction faction;

	[SerializeField]
	Text dbt, dht, aht, abt;

    public bool forceStrats; //TESTGGG para permitir las pruebas con cambios de estrategia
	public bool onlyOne;
	bool block = false;

	[SerializeField]
	float atkbase, defbase, atkhalf, defhalf; //Valores iniciales

    [SerializeField]
    float offensiveFactor;

    StrategyLayer strategyLayer;
    public MilitaryResourcesAllocator militaryResourceAllocator;

    Dictionary<StrategyT, SchedulerStrategy> strategySchedulers = new Dictionary<StrategyT, SchedulerStrategy>() {
                                                                              { StrategyT.DEF_BASE, new SchedulerDefBase() },
                                                                              { StrategyT.DEF_HALF, new SchedulerDefHalf() },
                                                                              { StrategyT.ATK_BASE, new SchedulerAtkBase() },
                                                                              { StrategyT.ATK_HALF, new SchedulerAtkHalf() } };

    // Use this for initialization
    void Start () {
        strategyLayer = new StrategyLayer(faction);

        foreach (StrategyT strategy in strategySchedulers.Keys) {
            strategySchedulers[strategy].Initialize(faction);
        }

		militaryResourceAllocator = new MilitaryResourcesAllocator(faction, atkbase, defbase, atkhalf, defhalf);
        militaryResourceAllocator.SetOffensiveFactor(offensiveFactor);
	}

    // Update is called once per frame
    void Update() {
		if (Time.frameCount % 120 == 0 || forceStrats == true) {
            //Layer1
			if ((strategyLayer.Apply() || forceStrats == true) && block == false) { //TESTGGG eliminar lo del block
				if (onlyOne) block = true;
				forceStrats = false; // Esta no deberia eliminarse porque la necesitamos para crear unidades 
                Debug.Log("HAN CAMBIADO LOS VALORES DE ESTRATEGIA, REASIGNANDO TROPAS");
				DrawStrategyValues();
                //Layer 2
                militaryResourceAllocator.SetPriority(strategyLayer.GetPriority()); //TESTGGG DESACTIVAR MIENTRAS ESTEMOS HACIENDO PRUEBAS
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

	void DrawStrategyValues (){
		if (faction == Faction.A) { // Para que solo lo hagan los aliados y no los enemigos
			dbt.text = "Defend the base: [" + strategyLayer.priority[StrategyT.DEF_BASE] + "]";
			dht.text = "Defend territory: [" + strategyLayer.priority[StrategyT.DEF_HALF] + "]";
			aht.text = "Attack territory: [" + strategyLayer.priority[StrategyT.ATK_HALF] + "]";
			abt.text = "Attack the base: [" + strategyLayer.priority[StrategyT.ATK_BASE] + "]";
		}
	}
}
