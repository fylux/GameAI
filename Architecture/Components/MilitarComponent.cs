﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MilitarComponent : UnitComponent {

    public int maxHealth = 10;
    public int health = 10;
    public int attack = 6;
    public int defense = 3;

    [SerializeField]
    GameObject projectile;

    public MilitarComponent() {
        health = maxHealth;
    }

    public void Attack(AgentUnit unit) {
        float damage;
        if (Random.Range(0, 100) > 99f) {
            damage = attack * 5;
        } else {
            damage = attack * Random.Range(0.8f, 1.2f) * AgentUnit.atkTable[(int)agent.GetUnitType(), (int)unit.GetUnitType()];
        }

        if (projectile != null) {
            var newProjectile = GameObject.Instantiate(projectile);
            newProjectile.transform.position = agent.position;
            newProjectile.GetComponent<Projectile>().SetTarget(unit);
        }
        else {
            Debug.Log("Falta el prefab del projectil para atacar");
        }


        unit.militar.ReceiveAttack(agent, (int)Mathf.Round(damage));
    }

    public float ReceiveAttack(AgentUnit enemy, int amount) {
        int damage = Mathf.Max(0, amount - defense);
        Console.Log("Unit caused " + damage + " damage");
        health -= damage;
        if (IsDead()) {
            Console.Log("Unit died");
            agent.gameObject.SetActive(false);

            //Update list of units removing this one
            Map.unitList.Remove(agent);
            GameObject.Destroy(agent.gameObject);
        }
        else if (agent.HasTask<HostileTask>()) { //To change the target if needed
            ((HostileTask)agent.GetTask()).ReceiveAttack(enemy);
        }
        //Request to update selection text
        return damage;
    }

    public bool IsDead() {
        return health <= 0;
    }
}
