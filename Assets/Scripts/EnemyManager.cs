using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> enemies;
    private HashSet<GameObject> collided; // List of collided enemies;
    [SerializeField] private WorldScripts worldScripts;
    public GameObject group_door;
    public bool door_is_active = true;
    void Start() {
        enemies = new List<GameObject>();
        foreach(Transform child in transform) {
            if (child.gameObject.activeSelf && child.tag == "Enemy") enemies.Add(child.gameObject);
        }
        collided = new HashSet<GameObject>();
    }

    public void CollisionDetected(GameObject g) {
        collided.Add(g);
    }


    public void Update() {

        if (!door_is_active) {
            group_door.transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 90, 0);
            group_door.transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        }

        PlayerStatTracker.instance.EnemyCount = collided.Count;
        if (collided.Count > 0) {
            foreach (var e in enemies) {
                Destroy(e);
            }
            collided = new HashSet<GameObject>();            
            // Save enemy state

            // Get parent child count to find group number
            // Set group in combat state to true

            int child_count = transform.parent.childCount;
            for (int i = 0; i < child_count; i++) {
                if (transform.parent.GetChild(i).position == PlayerStatTracker.instance.enemy_states[i].group && transform.parent.GetChild(i).gameObject == gameObject) {
                    Debug.Log($"{PlayerStatTracker.instance.enemy_states.Count}, {i}");
                    var current_state = PlayerStatTracker.instance.enemy_states[i];
                    current_state.in_combat = true;
                    PlayerStatTracker.instance.enemy_states[i] = current_state;
                }
            }

            PlayerStatTracker.instance.SaveEnemyState(worldScripts.e_state);

            SceneManager.LoadScene("TurnbasedScene", LoadSceneMode.Single);
        }
    }
}
