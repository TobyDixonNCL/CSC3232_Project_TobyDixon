using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stats {
    public int speed;
    public int attack_power;
    public int defense;
    public int maxDefense;
    public int actions;
    public int health;
    public int wis;
    public int maxHealth;
    public bool is_dead;
};

public static class EntityBase {

    public enum entity_type {
        Barbarian,
        Mage,
        Paladin,
        Rogue,
        Skeleton // Only enemy currently
    };

    /*
        PALADIN: high defense, healing and attack (physical damage) and taunt (increase chance of being targeted) actions, low attack power, slow speed, regular health.
        BARBARIAN: low defense, high attack power (physical damage), high health, regular speed.
        MAGE: regular everything
        ROGUE: regular defense, regular attack power, stealth (if action used, reduce chance of being targeted) and attack (chance to inflict bleed?) actions, 
            2 actions, low health, very fast speed.
    */

    public static Stats GetStats(string name) {

        // If stats already exist for character then grab them
        if (PlayerStatTracker.instance.playerStats.Count != 0) {
            foreach (var stat in PlayerStatTracker.instance.playerStats) {
                if (stat.name == name) return stat.stats;
            }
        }


        // Base Stats
        int speed = 10;
        int attack_power = 15;
        int defense = 0;
        int maxDefense = 20;
        int actions = 1;
        int health = 100;
        int wis = 50;
        bool is_dead = false;
        
        // float level_mod = 1.2f;

        switch (name) {
            case ("Barbarian"):
                attack_power = attack_power * 3 / 2;
                health += 20;
                maxDefense= maxDefense / 2;
                wis = wis / 2;
                speed = 6;
                break;
            case ("Paladin"):
                speed = 5;
                attack_power = 10;
                maxDefense = maxDefense * 3 / 2;
                wis = wis * 2; 
                break;
            case ("Rogue"):
                health -= 50;
                actions = 2;
                speed = 15;
                break;
            case ("Enemy"):
            attack_power = attack_power * 3 / 2;
                speed = speed + Random.Range(-2, 2);
                break;
            default:
                break;
        }  

        Stats stats;
        stats.speed = speed;
        stats.attack_power = attack_power;
        stats.defense = defense;
        stats.maxDefense = maxDefense;
        stats.actions = actions;
        stats.health = health;
        stats.wis = wis;
        stats.maxHealth = health;
        stats.is_dead = false;

        return stats;
    }

    public static void SaveStats(string name, Stats stats) {

        // If player already exists then replace stats. otherwise create them

        int index = -1;
        for (int i = 0; i < PlayerStatTracker.instance.playerStats.Count; i++) {
            if (PlayerStatTracker.instance.playerStats[i].name == name) index = i;
        }

        if (index > -1) {
            PlayerStatTracker.instance.playerStats.RemoveAt(index);
            PlayerStatTracker.instance.playerStats.Add((name, stats));
        } else PlayerStatTracker.instance.playerStats.Add((name, stats));
    }

    public static int AttackDamage(Stats stats) {
        // int atk = Random.Range(0, 20);

        // if (atk == 20) return Mathf.RoundToInt(stats.attack_power * 2);
        // else if (atk < 10) return Mathf.RoundToInt(stats.attack_power * 0.75f);
        // else return Mathf.RoundToInt(stats.attack_power * 1.25f);

        return stats.attack_power;
    }

    public static int HealAmount (Stats stats) {
       return stats.wis;
    }

    public static Stats LevelUp(Stats stats) {
        
        List<KeyValuePair<string, int>> new_stats = new List<KeyValuePair<string, int>>();

        float levelup_mod = 1.2f;

        stats.speed += (int)(stats.speed * levelup_mod);
        stats.attack_power += (int)(stats.attack_power * levelup_mod);
        stats.defense += (int)(stats.defense * levelup_mod);
        stats.actions += (int)(stats.actions * levelup_mod);
        stats.health += (int)(stats.health * levelup_mod);
        stats.wis += (int)(stats.wis * levelup_mod);

        return stats;
    }

    public static int HealAction(Stats stats) {

        int roll = Random.Range(0, 20);
        
        if (roll <= 10) return (int)(stats.wis * 0.75);
        else if (roll <= 19) return (int)(stats.wis * 1.25);
        else return stats.wis * 2;
    }
}
