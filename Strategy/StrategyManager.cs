﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StrategyManager : MonoBehaviour {

    public Faction faction;

	[SerializeField]
	Text strategies;

    public bool forceStrats; //TESTGGG para permitir las pruebas con cambios de estrategia
	public bool onlyOne;
	public bool block = false;

	[SerializeField]
	float atkbase, defbase, atkhalf, defhalf; //Valores iniciales

    [SerializeField]
    float offensiveFactor;

    public StrategyLayer strategyLayer;
    public MilitaryResourcesAllocator militaryResourceAllocator;


    float nextL12Time, nextL3Time = 0.0f;
    float periodL12 = 1.5f;
    float periodL3 = 0.5f;

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
        if (Time.fixedTime > nextL12Time || forceStrats == true) {
            nextL12Time += periodL12;
            if (( forceStrats == true || strategyLayer.Apply() ) && block == false) { //TESTGGG eliminar lo del block
                CycleLayer12();
            }
        }

        if (Time.fixedTime > nextL3Time) {
            nextL3Time += periodL3;
            //Layer 3
            foreach (StrategyT strategy in strategySchedulers.Keys) {
                /*  Debug.Log("Miembros recibiendo ordenes de la estrategia " + strategy);
                  foreach (AgentUnit unit in strategySchedulers[strategy].usableUnits)
                      Debug.Log("------> " + unit);
                  */
                strategySchedulers[strategy].ApplyStrategy();

                //Layer 4
                //Each unit will just apply its assigned task

            }
        }

    }

    //Layer1
    public void CycleLayer12() {
        //strategyLayer.Apply(); //TODO Por qué?

        if (onlyOne) block = true;
        forceStrats = false; // Esta no deberia eliminarse porque la necesitamos para crear unidades 
     //   Debug.Log("HAN CAMBIADO LOS VALORES DE ESTRATEGIA, REASIGNANDO TROPAS");
        DrawStrategyValues();

        //Layer 2
        militaryResourceAllocator.SetPriority(strategyLayer.GetPriority()); //TESTGGG DESACTIVAR MIENTRAS ESTEMOS HACIENDO PRUEBAS
        Dictionary<StrategyT, HashSet<AgentUnit>> unitsToStrategy = militaryResourceAllocator.AllocateResources();

        foreach (var strategy in strategySchedulers.Keys) {
            strategySchedulers[strategy].Reset();
        }

        foreach (var strategy in unitsToStrategy.Keys) {
            Debug.Log(strategy +" "+unitsToStrategy[strategy].Count);
            strategySchedulers[strategy].usableUnits = unitsToStrategy[strategy];
        }
    }
   
	void DrawStrategyValues (){
		strategies.text = "DB " + strategyLayer.priority[StrategyT.DEF_BASE].ToString("F2")
        + " / DH " + strategyLayer.priority[StrategyT.DEF_HALF].ToString("F2")
		+ "\nAH " + strategyLayer.priority[StrategyT.ATK_HALF].ToString("F2")
        + " / AB " + strategyLayer.priority[StrategyT.ATK_BASE].ToString("F2");
	}

	public void RemoveUnitFromSchedulers(AgentUnit unit){
		foreach (SchedulerStrategy sched in strategySchedulers.Values) {
			sched.RemoveUnit (unit);
		}
	}

    public StrategyLayer GetStrategyLayer() {
        return strategyLayer;
    }

    public void UpdateOffensiveFactor() {
        string name = "OffensiveFactor" + (faction == Faction.A ? "A" : "B");
        if (GameObject.Find(name) == null) {
            Debug.LogError("cannot find slider " + name);
        }
        Slider slider = GameObject.Find(name).GetComponent<Slider>();
        
        offensiveFactor = slider.value;
        militaryResourceAllocator.SetOffensiveFactor(slider.value);

        CycleLayer12();
        Debug.Log(Time.frameCount + " change offensive factor");
    }
}
