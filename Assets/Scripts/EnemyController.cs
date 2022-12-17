using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private int speed = 5;

    private bool player_found = false;
    private Transform target;
    [SerializeField] private NavMeshAgent agent;

    private int wander_distance = 20;

    private float overlap_radius = 10.0f;

    private Rigidbody rb;

    private bool turning = true;
    private int turn_time;
    private int turn_time_max = 50;
    private bool blocked = false;


    // TODO: Enemy count should be stored somewhere, Perhaps collision sphere for enemies within a radius.
    // TODO: Enemies could communicate within range. 
    
    public void Start() {
        turn_time = turn_time_max;
        agent.speed = speed;
        // rb = GetComponent<Rigidbody>();

        // Set ground position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 10)) {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
        }

    }

    // Update is called once per frame
    public void Update() {

        // TODO: Raycast to player for line of sight, if too far or cannot see then wander
        // TODO: Wandering code - state machine?

        // See if player within radius

        Collider[] colliders = Physics.OverlapSphere(transform.position, overlap_radius);

        foreach (Collider col in colliders) {
            if (col.tag == "Player") {
                // TODO: Get group state, if door is active then dont allow find player
                player_found = true;
            }
        }
        
        if (player_found) {
            agent.destination = player.transform.position;
        } else Wander();

        // Look in move direction
        transform.LookAt(agent.destination, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public void Wander() {

        if (Vector3.Distance(transform.position, agent.destination) < 1 || agent.destination == null || blocked) {

            if (turning) {
                transform.RotateAround(transform.position, Vector3.up, 20);
                turn_time --;
                if (turn_time <= 0) turning = false;
            } else {
                Vector3 dest = Random.insideUnitSphere * wander_distance + transform.position;

                NavMeshHit navhit;
                NavMesh.SamplePosition(dest, out navhit, wander_distance, -1);

                agent.destination = navhit.position;
                turning = true;
                turn_time = turn_time_max;
            }
        } else if (Mathf.Abs(agent.speed) < 1) {
            Vector3 dest = Random.insideUnitSphere * wander_distance + transform.position;

            NavMeshHit navhit;
            NavMesh.SamplePosition(dest, out navhit, wander_distance, -1);

            agent.destination = navhit.position;
            turning = true;
            turn_time = turn_time_max;
            blocked = false;
        }
    }

    public void OnCollisionEnter(Collision col) {
        if (col.collider.tag == "Player")
            transform.parent.GetComponent<EnemyManager>().CollisionDetected(gameObject);

        if (col.collider.tag == "invisible_barrier") {
            blocked = true;
        }
    }
}
