using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WorldScripts : MonoBehaviour {

    public List<EnemyState> e_state;
    [SerializeField] GameObject e_manager;
    [SerializeField] GameObject location_indicator;

    void Start() {  

        int dead = 0;

        // If enemy states exist then get, otherwise create
        if (PlayerStatTracker.instance.enemy_states.Count > 0) {
            e_state = PlayerStatTracker.instance.enemy_states;

        } else {
            e_state = new List<EnemyState>();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyGroup");
            foreach (var e in enemies) {
                EnemyState enemyState = new EnemyState();

                enemyState.group = e.transform.position;

                enemyState.in_combat = false;

                var objects = new List<GameObject>();
                var positions = new List<Vector3>();

                foreach (Transform child in transform) {
                    positions.Add(child.position);
                    objects.Add(child.gameObject);
                }
                enemyState.positions = positions;
                enemyState.obj = objects;

                enemyState.is_dead = false;
                e_state.Add(enemyState);
            }
            PlayerStatTracker.instance.enemy_states = e_state;
        }

        // Gather enemies in scene and edit based on state
        Debug.Log($"child_count {e_manager.transform.childCount}");

        bool prev_group_dead = true;

        for (int group = 0; group < e_manager.transform.childCount; group++) {
            var state = e_state[group];
            var enemyGroup = e_manager.transform.GetChild(group);
            if (!state.is_dead) {
                // If previous group is dead
                if (prev_group_dead) {
                    // Open door and set variable
                    enemyGroup.GetComponent<EnemyManager>().door_is_active = false;
                    prev_group_dead = false;
                    location_indicator.transform.position = enemyGroup.GetComponent<EnemyManager>().group_door.transform.position;
                    location_indicator.transform.rotation = enemyGroup.GetComponent<EnemyManager>().group_door.transform.rotation;
                }
                for (int i = 0; i < state.positions.Count; i++) {
                    // Get instantiated object
                    var inst_obj = enemyGroup.GetChild(i).gameObject;
                    inst_obj.transform.position = state.positions[i];
                    state.obj[i].transform.position = state.positions[i];
                }
            } else {
                dead ++;
                for (int i = 0; i < enemyGroup.transform.childCount; i++) {
                    Destroy(enemyGroup.GetChild(i).gameObject);
                }
                enemyGroup.GetComponent<EnemyManager>().door_is_active = false;
                prev_group_dead = true;
            }
        }

        if (dead >= e_manager.transform.childCount) {
            SceneManager.LoadScene("GameWin", LoadSceneMode.Single);
        }



    }
}
