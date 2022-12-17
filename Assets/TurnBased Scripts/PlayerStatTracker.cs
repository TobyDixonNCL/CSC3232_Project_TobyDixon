using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyState {
    public Vector3 group;
    public List<GameObject> obj;
    public List<Vector3> positions;
    public bool in_combat;
    public bool is_dead;
};

public class PlayerStatTracker : MonoBehaviour {

    public static PlayerStatTracker instance;

    // Tracks player stats
    public List<(string name, Stats stats)> playerStats;
    
    // Track how many enemies should be in a battle
    public int EnemyCount;

    public int KeyCount = 0;

    public Vector3 player_position;

    public (int players, int enemies) final_game_state;

    // Struct that contains :
    //  - enemy group (Contains a global position)
    //  - enemy positions (relative to their group)
    //  - whether or not they are engaged in combat
    //  - alive or not (stats??)
    //  - etc....
    // Have an overworld script that on start function gets the state for all of the entities

    public List<EnemyState> enemy_states;

    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != this) Destroy(gameObject);

        playerStats = new List<(string name, Stats stats)>();
        enemy_states = new List<EnemyState>();
    }

    public void SaveEnemyState(List<EnemyState> e_states) {
        enemy_states = e_states;
    }

    public void RevertDefaultValues() {
        playerStats = new List<(string name, Stats stats)>();
        enemy_states = new List<EnemyState>();
        KeyCount = 0;
    }
}
