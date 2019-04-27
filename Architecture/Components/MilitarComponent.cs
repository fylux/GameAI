using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MilitarComponent : UnitComponent {

    public int maxHealth = 10;
    public int health = 10;
    public int attack = 6;
    public int defense = 3;


    public MilitarComponent() {
        health = maxHealth;
    }

    public void Attack(AgentUnit unit) {
        float damage;
        if (Random.Range(0, 100) > 99f) {
            damage = attack * 5;
        } else {
            damage = attack * Random.Range(0.8f, 1.2f) /** factorTable*/;
        }
        unit.militar.ReceiveAttack(agent, (int)Mathf.Round(damage));
    }

    public float ReceiveAttack(AgentUnit enemy, int amount) {
        int damage = Mathf.Max(0, amount - defense);
        Console.Log("Unit caused " + damage + " damage");
        health -= damage;
        if (health < 0) {
            Console.Log("Unit died");
            agent.gameObject.SetActive(false);
        }
        else if (agent.GetTask() is DefendZone) { //To change the target if needed
            DefendZone task = (DefendZone)agent.GetTask();
            task.ReceiveAttack(enemy);
        }
        //Request to update selection text
        return damage;
    }

}
