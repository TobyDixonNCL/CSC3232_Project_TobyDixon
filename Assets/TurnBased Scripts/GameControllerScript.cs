using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// TODO: Level up bar and stats etc...


// TODO: NOW!!!!!!!
// TODO: Change how defense affects health - just subtract from damage and then defense should dissappear
// TODO: Stats need reworking
// TODO: DOORS FOR CHAMBERS BASED ON GROUP STATE

public class GameControllerScript : MonoBehaviour
{

    private enum PlayerAction {
        Attack,
        Heal,
        Defend,
        None
    };

    private PlayerAction player_turn_action = PlayerAction.None;

    [SerializeField] private GameObject healing_particles;
    private (GameObject _entity, Stats _stats) active_entity;
    private (GameObject _entity, Stats _stats) target_entity;
    private int turn_counter;

    private GameObject turn_indicator;
    private GameObject target_indicator;

    [SerializeField] private Material friendly_target;
    [SerializeField] private Material enemy_target;

    private List<(GameObject _entity, Stats _stats)> Entities;
    private List<(GameObject _entity, Stats _stats)> players;
    private List<(GameObject _entity, Stats _stats)> enemies;

    // Turn tracker for turnbased combat
    private bool PlayerTurn = true;

    private int action_count;
    public int options;

    private bool just_changed_turn = true;
    private bool win = false;
    private bool lose = false;

    [SerializeField] private GameObject healthbar_prefab;
    private List<((GameObject _entity, Stats _stats) _ent_obj, GameObject _bar)> healthbars;
    private List<((GameObject _entity, Stats _stats) _ent_obj, GameObject _bar)> defensebars;

    [SerializeField] private Canvas canvas;

    private float timer;

    void Awake() {

    }

    void Start() {  

        turn_indicator = GameObject.Find("turn_indicator").GetComponentInChildren<Transform>().gameObject;
        target_indicator = GameObject.Find("target_indicator").GetComponentInChildren<Transform>().gameObject;

        // Create Character and Enemies lists
        Entities = new List<(GameObject, Stats)>();
        players = new List<(GameObject, Stats)>();
        enemies = new List<(GameObject, Stats)>();

        // Get transforms of children
        var c_trm = GameObject.Find("Characters").GetComponentInChildren<Transform>();
        var e_trm = GameObject.Find("Enemies").GetComponentInChildren<Transform>();

        // Append game objects to respective lists
        foreach (Transform t in c_trm) {
            Stats stats = EntityBase.GetStats(t.gameObject.name);
            Entities.Add((t.gameObject, stats));
            players.Add((t.gameObject, stats));
        }

        foreach (Transform t in e_trm) {
            Stats stats = EntityBase.GetStats(t.gameObject.tag);
            Entities.Add((t.gameObject, stats));
            enemies.Add((t.gameObject, stats));
        }

        Entities = Entities.OrderBy(x => x._stats.speed).ToList();

        foreach (var entity in Entities) {
            Debug.Log($"{entity._entity.name} | {entity._stats.is_dead}");
        }


        // Set turn counter to first entity
        turn_counter = 0;

        // Assign first active entity

        SortEntities();
        active_entity = Entities[turn_counter];


        // Create healthbars
        healthbars = new List<((GameObject _entity, Stats _stats) _ent_obj, GameObject _bar)>();
        defensebars = new List<((GameObject _entity, Stats _stats) _ent_obj, GameObject _bar)>();
        foreach (var e in Entities) {
            Vector3 h_pos = Camera.main.WorldToScreenPoint(e._entity.transform.position);
            h_pos.x -= 25; // Center bar
            Vector3 d_pos = h_pos + new Vector3(0, 10, 0);

            GameObject h = Instantiate(healthbar_prefab, h_pos, Quaternion.Euler(0, 0, 0));
            GameObject d = Instantiate(healthbar_prefab, d_pos, Quaternion.Euler(0, 0, 0));

            h.transform.parent = canvas.transform;
            d.transform.parent = canvas.transform;

            healthbars.Add((e, h));
            defensebars.Add((e, d));

        }

        UpdateHealthbars();


        // Move turn indicator to highlight first active entity
        turn_indicator.transform.position = active_entity._entity.transform.position;

        // Set intial action count
        action_count = active_entity._stats.actions;

        if (active_entity._entity.tag == "Enemy") PlayerTurn = false;

        if (!PlayerTurn) ChangeTarget(players.First());
        else ChangeTarget(enemies.First());

        // target_indicator.transform.position = target_entity._entity.transform.position;

    }

    // Update is called once per frame
    void Update() {
        if (!win && !lose) {
            Turn();
            CheckStats();
            SortEntities();
            UpdateHealthbars();
        } else {
            
            if (win) {
                foreach (var p in players) {
                    Stats stats = p._stats;
                    if (stats.health <= 0) {
                        stats.health = 1;
                        stats.is_dead = false;
                    }

                    EntityBase.SaveStats(p._entity.name, stats);
                }
                for (int i = 0; i < PlayerStatTracker.instance.enemy_states.Count; i++) {
                    var eState = PlayerStatTracker.instance.enemy_states[i];
                    if (eState.in_combat) {
                        eState.is_dead = true;
                        PlayerStatTracker.instance.enemy_states[i] = eState;
                    }
                }
                    

                SceneManager.LoadScene("OverworldScene", LoadSceneMode.Single);
            } else {
                SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
            }
        }

    }

    void Turn() {

        // Turn based on active entity
        if (active_entity._entity.tag == "Enemy") PlayerTurn = false;
        else PlayerTurn = true;

        // Skip turn if entity is dead
        bool is_alive = active_entity._stats.health <= 0 ? false : true;

        // Turn pass key
        if (Input.GetKeyDown(KeyCode.Space)) action_count --;

        if (timer > 0) {
            timer -= Time.deltaTime;
        }  else if (action_count > 0 && is_alive) {

            turn_indicator.transform.position = active_entity._entity.transform.position;

            ParticleSystemRenderer pr = (ParticleSystemRenderer) turn_indicator.GetComponent<ParticleSystem>().GetComponent<Renderer>();

            if (PlayerTurn) {
                pr.material = friendly_target;
                pr.trailMaterial = friendly_target;

            }
            else {
                pr.material = enemy_target;
                pr.trailMaterial = enemy_target;
            }

            // Actions are performed whilst current character has actions
            if (PlayerTurn) PlayerTurnLoop();
            else {
                EnemyTurnLoop();
                timer = 1;
                action_count --;
            }

        } else {
            // Advanced to next turn
            turn_counter = turn_counter >= Entities.Count() - 1 ? 0 : turn_counter + 1;
            active_entity = Entities.ElementAt(turn_counter);

            // Set default target to an enemy
            ChangeTarget(enemies.Where(e => e._stats.health >= 0).ToList()[0]);

            action_count = active_entity._stats.actions;
            
            // Clear player actions
            player_turn_action = PlayerAction.None;

            // Just changed is now true
            just_changed_turn = true;
        }
    }

    public void ChangeTarget((GameObject _entity, Stats _stats) newTarget) {
        
        bool isFriendly = enemies.Contains(newTarget) ? false : true;

        if (!newTarget._stats.is_dead) {
            target_entity = newTarget;
            if (isFriendly)  target_indicator.GetComponentInChildren<MeshRenderer>().material = friendly_target;
            else target_indicator.GetComponentInChildren<MeshRenderer>().material = enemy_target;
            target_indicator.transform.position = newTarget._entity.transform.position;
        }


    }

    public void CheckTargetChange() {
        
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit)) {

                foreach(var e in Entities) {
                    if (e._entity.name == hit.transform.name) ChangeTarget(e);
                }

                ChangeTarget(target_entity);
            }
        }

    }

    public void PlayerTurnLoop() {
        if (just_changed_turn) {
            just_changed_turn = false;
            ChangeTarget(enemies.Where(e => !e._stats.is_dead).ToList()[0]);
        }
        CheckTargetChange();

        switch (player_turn_action) {
            case PlayerAction.Attack:
                Attack();
                player_turn_action = PlayerAction.None;
                action_count --;
                break;
            case PlayerAction.Defend:
                Defend();
                player_turn_action = PlayerAction.None;
                action_count --;
                break;
            case PlayerAction.Heal:
                Heal();
                player_turn_action = PlayerAction.None;
                action_count --;
                break;
            default:
                break;
        }
        
        player_turn_action = PlayerAction.None;

    }

    public void SetPlayerAction(Button b) {
        switch (b.name) {
            case "Attack_Button":
                player_turn_action = PlayerAction.Attack;
                break;
            case "Heal_Button":
                player_turn_action = PlayerAction.Heal;
                break;
            case "Defend_Button":
                player_turn_action = PlayerAction.Defend;
                break;
            default:
                player_turn_action = PlayerAction.None;
                break;
        }
    }

    public void EnemyTurnLoop() {
        Debug.Log("Enemy Turn");

        List<(GameObject _entity, Stats _stats)> target_entities = players.Where(p => p._stats.health >= 0).ToList();


        // If all players are above half health, target a random player
        bool players_above_danger = true;

        foreach (var t in target_entities) {
            if (t._stats.health - (active_entity._stats.attack_power * 2) <= 0) players_above_danger = false;
        }

        if (players_above_danger) {
            ChangeTarget(target_entities[Random.Range(0, target_entities.Count)]);
        } else {
            int lowest_hp = target_entities.Min(e => e._stats.health);
            // Get target wth lowest health
            foreach (var t in target_entities) {
                if (t._stats.health == lowest_hp) ChangeTarget(t);
            }
        }

        if (active_entity._stats.health <= active_entity._stats.maxHealth / 5){
            if (active_entity._stats.health < target_entity._stats.health) {
                ChangeTarget(active_entity);
                Debug.Log($"Healing {target_entity._entity.name}");
                Heal();
            }
            else {
                Debug.Log($"Attacking {target_entity._entity.name}");
                Attack();
            }
        } else {
            Debug.Log($"Attacking {target_entity._entity.name}");
            Attack();
        }
    }

    public void Attack() {
        int dmg = EntityBase.AttackDamage(active_entity._stats);
        int opp_health = target_entity._stats.health;
        int opp_def = target_entity._stats.defense;

        var new_target_stats = target_entity._stats;
        new_target_stats.health = opp_health + opp_def - dmg;
        if (dmg > opp_def) new_target_stats.defense = 0;
        else new_target_stats.defense = opp_def - dmg;

        if (new_target_stats.health <= 0) new_target_stats.is_dead = true;

        // Debug.Log($"{target_entity._entity.name}, HP: {opp_health}, new_HP: {new_target_stats.health}, Red: {opp_def}, dmg: {dmg}");

        RecalcStats(target_entity._entity.name, new_target_stats);

    }

    public void Defend() {
        int def = active_entity._stats.defense;
        def += active_entity._stats.attack_power;

        if (def > active_entity._stats.maxDefense) def = active_entity._stats.maxDefense;

        Stats new_stats = active_entity._stats;
        new_stats.defense = def;
        RecalcStats(active_entity._entity.name, new_stats);
    }

    public void Heal() {

        // Spawn healer particle system
        Instantiate(healing_particles, target_entity._entity.transform.position, target_entity._entity.transform.rotation);

        int heal = EntityBase.HealAction(active_entity._stats);
        int target_health = target_entity._stats.health;

        target_health += heal;
        if (target_health > target_entity._stats.maxHealth) target_health = target_entity._stats.maxHealth;

        Stats new_stats = target_entity._stats;
        new_stats.health = target_health;

        Debug.Log($"Recalcing for {target_entity._entity.name}, hp: {target_entity._stats.health} -> {target_health}");
        RecalcStats(target_entity._entity.name, new_stats);
    }

    public void RecalcStats(string name, Stats new_stats) {

        // Foreach entity, if name = name then replace stats
        int remove_index = -1;
        GameObject obj = Entities[0]._entity;

        for (int i = 0; i < Entities.Count; i++) {
            if (Entities[i]._entity.name == name) {
                remove_index = i;
                obj = Entities[i]._entity;
            }
        }

        int p_index = players.FindIndex(p => p._entity.name == name);
        if (p_index != -1) players[p_index] = (obj, new_stats);

        int ent_index = Entities.FindIndex(e => e._entity.name == name);
        if (ent_index != -1) Entities[ent_index] = (obj, new_stats);

        int enem_index = enemies.FindIndex(e => e._entity.name == name);
        if (enem_index != -1) enemies[enem_index] = (obj, new_stats);

        // Update target entity
        target_entity = (obj, new_stats);
        if (target_entity._stats.is_dead) ChangeTarget(Entities.Where(e => e._stats.is_dead == false).ToList()[0]);


        // Update healthbar entities
        foreach (var e in Entities) {
            int i = healthbars.FindIndex(h => h._ent_obj._entity.name == e._entity.name);
            healthbars[i] = (e, healthbars[i]._bar);
        }


        // players.Remove(new_target);
        // players.Remove(new_target);

        // Debug.Log($"{new_entity.Key.name}, {new_entity.Value.health}");

    }

    public void SortEntities() {
        Entities = Entities.OrderByDescending(e => e._stats.speed).ToList();
    }

    public void CheckStats() {
        // Win if all enemies dead, lose if all players dead
        int p_total = 0;
        foreach (var p in players) {
            if (p._stats.is_dead) {
                p_total ++;
                Kill(p._entity);
            }
        }

        if (p_total == players.Count) lose = true;

        int e_total = 0;
        foreach (var e in enemies) {
            if (e._stats.is_dead) {
                e_total ++;
                Kill(e._entity);
            }
        }

        if (e_total == enemies.Count) win = true;
    }


    public void UpdateHealthbars() { // Also updates defense bars
        for (int i = 0; i < healthbars.Count; i++) {
            Stats s = healthbars[i]._ent_obj._stats;


            // 50 is max width for bars
            float health_ratio = ((float)s.health / (float)s.maxHealth);
            float health_width = health_ratio * 50;

            float def_ratio = ((float)s.defense / (float)s.maxDefense);
            float def_width = def_ratio * 50;
            
            // Get bar components
            GameObject healthbar = healthbars[i]._bar;
            GameObject health_colBar = healthbar.transform.GetChild(0).gameObject;

            GameObject defbar = defensebars[i]._bar;
            GameObject def_colBar = defbar.transform.GetChild(0).gameObject;

            // Change width
            health_colBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health_width);
            def_colBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, def_width);

            // Change colour
            Color healthbar_col = health_ratio <= 0.2 ? Color.red : Color.green;
            health_colBar.GetComponent<Image>().color = healthbar_col;

            Color defbar_col = Color.blue;
            def_colBar.GetComponent<Image>().color = defbar_col;

            // Set component
            healthbars[i] = ((healthbars[i]._ent_obj._entity, s), healthbar);
            defensebars[i] = ((defensebars[i]._ent_obj._entity, s), defbar);
        }
    }

    public void Kill(GameObject entity) {
        entity.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

}
