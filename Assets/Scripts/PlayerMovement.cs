using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody rb;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float max_speed;
    private float current_speed;
    private float current_max_speed;
    private float sprint_multiplier;

    private bool moving = false;

    [SerializeField]
    private GameObject Weapon;

    Ray mouse_ray;
    RaycastHit mouse_hit;
    Animator animator;



    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponentInChildren<Animator>();
        speed = 500.0f;
        max_speed = 2000.0f;
        sprint_multiplier = 1.5f;

        // ChangeWeapon(weapon);

    }

    // Update is called once per frame
    void FixedUpdate()
    {       
        var forward = Camera.main.transform.forward;
        var right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        var move_direction = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift)) {
            current_speed = speed * sprint_multiplier;
            current_max_speed = max_speed * sprint_multiplier;
        } else {
            current_speed = speed;
            current_max_speed = max_speed;
        }

        var move_vec = move_direction * current_speed * Time.deltaTime;
        move_vec.y = rb.velocity.y;
        rb.velocity = move_vec;
        // rb.position = rb.position + move_vec;
        Vector3.ClampMagnitude(rb.velocity, current_max_speed);


        // Rotation
        mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouse_ray, out mouse_hit)) {
            if (mouse_hit.transform.tag != "Player") {
                Vector3 targetPosition = new Vector3(mouse_hit.point.x, transform.position.y, mouse_hit.point.z);
                transform.LookAt(targetPosition);
            }
        }

        gameObject.GetComponentInChildren<Transform>().position = transform.position;
        gameObject.GetComponentInChildren<Transform>().rotation = transform.rotation;

    }

    void Update() {

        // Debug.Log(Mathf.Abs(Input.GetAxis("Vertical")));

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.5 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.5) {
            animator.SetBool("isRunning", true);
        } else animator.SetBool("isRunning", false);

        // Debug.Log(Mathf.Abs(rb.velocity.magnitude));

        // weapon_pos = new Vector3(transform.position.x + 0.5f, transform.position.y + -0.8f, transform.position.z);
        // weapon.transform.position = weapon_pos;

        // weapon.transform.position = WeaponPos.transform.position;

        // Debug.Log(weapon);
    }

    private void OnTriggerEnter(Collider col) {
        Vector3 pos = new Vector3();
        // Debug.Log($"Should spawn {pos}");
    }

    public void OnCollisionEnter(Collision col) {
        // Debug.Log(col.collider.transform.name);
    }
}
