using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour {

    [SerializeField]
    GameObject enemy;

    [SerializeField]
    StrategyManager stratB;

	[SerializeField]
	StrategyManager stratA;

	StrategyManager strat; // strategymanager escogido

	void Start(){
		strat = stratA;
	}

	// Update is called once per frame
	void Update () {
		// Z = Defbase, X = Defhalf, C = Atkhalf, V = Atkbase
		// B = Faction B, N = Faction A
		// M = Massive Attack (A and B -> Atkbase)


        if (Input.GetKeyDown(KeyCode.Z))
        {
			strat.strategyLayer.priority = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, 0.0f},
            { StrategyT.ATK_HALF, 0.0f},
            { StrategyT.DEF_BASE, 1f},
            { StrategyT.DEF_HALF, 0.0f}
            };

			strat.CycleLayer12();
        }

		if (Input.GetKeyDown(KeyCode.V))
		{
			strat.strategyLayer.priority = new Dictionary<StrategyT, float>() {
				{ StrategyT.ATK_BASE, 1f},
				{ StrategyT.ATK_HALF, 0.0f},
				{ StrategyT.DEF_BASE, 0.0f},
				{ StrategyT.DEF_HALF, 0.0f}
			};

			strat.CycleLayer12();
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			strat.strategyLayer.priority = new Dictionary<StrategyT, float>() {
				{ StrategyT.ATK_BASE, 0.0f},
				{ StrategyT.ATK_HALF, 0.0f},
				{ StrategyT.DEF_BASE, 0.0f},
				{ StrategyT.DEF_HALF, 1f}
			};

			strat.CycleLayer12();
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			strat.strategyLayer.priority = new Dictionary<StrategyT, float>() {
				{ StrategyT.ATK_BASE, 0.0f},
				{ StrategyT.ATK_HALF, 1f},
				{ StrategyT.DEF_BASE, 0.0f},
				{ StrategyT.DEF_HALF, 0.0f}
			};

			strat.CycleLayer12();
		}

		if (Input.GetKeyDown(KeyCode.N))
		{
			strat = stratA;
		}

		if (Input.GetKeyDown(KeyCode.B))
		{
			strat = stratB;
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			stratA.strategyLayer.priority = new Dictionary<StrategyT, float>() {
				{ StrategyT.ATK_BASE, 1f},
				{ StrategyT.ATK_HALF, 0.0f},
				{ StrategyT.DEF_BASE, 0.0f},
				{ StrategyT.DEF_HALF, 0.0f}
			};
			stratA.block = true;
			stratA.CycleLayer12();

			stratB.strategyLayer.priority = new Dictionary<StrategyT, float>() {
				{ StrategyT.ATK_BASE, 1f},
				{ StrategyT.ATK_HALF, 0.0f},
				{ StrategyT.DEF_BASE, 0.0f},
				{ StrategyT.DEF_HALF, 0.0f}
			};
			stratB.block = true;
			stratB.CycleLayer12();
		}
    }
}
