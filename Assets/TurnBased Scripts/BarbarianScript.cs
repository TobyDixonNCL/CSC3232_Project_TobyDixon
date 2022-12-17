using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbarianScript : MonoBehaviour
{
    // Start is called before the first frame update
    public List<KeyValuePair<string, int>> stats;
    void Start() {
        // stats = EntityBase.GetStats(EntityBase.entity_type.Barbarian);

        /* -- Checking stats and methods -- 
        foreach (var stat in stats) Debug.Log($"{stat.Key} | {stat.Value}");

        var attack_roll = EntityBase.AttackDamage(stats);
        var damage_reduction = EntityBase.DefenseReduction(stats, attack_roll);

        Debug.Log($"Attack roll = {attack_roll} dmg, Defense reduction = {damage_reduction}");
        Debug.Log($"Total Damage = {attack_roll - damage_reduction}");
        */

    }

    // Update is called once per frame
    void Update() {
        
    }
}
