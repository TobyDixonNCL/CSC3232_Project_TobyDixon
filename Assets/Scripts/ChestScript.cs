using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{

    /*
    TODO:
        - Proximity effect where slightly opens lid
        - Select and instantiate spawnables before opening
            - *less resources*
            - Spawnables need a rb, collider, light?, collectable bool that only becomes true on release  
    */
    
    [SerializeField]
    private bool opened = false;
    private float despawn_timer = 2.0f;

    private int item_count;

    public GameObject spawnable;

    [SerializeField]
    private GameObject state_unopen;
    
    [SerializeField]
    private GameObject state_open;
    
    void Start() {

        state_unopen = Instantiate(state_unopen, gameObject.transform);
        state_open = Instantiate(state_open, gameObject.transform);

        if (opened) state_unopen.SetActive(false);
        else state_open.SetActive(false);

        item_count = Random.Range(1, 5);
    }

    // Update is called once per frame
    void Update() {
        if (opened) {
            despawn_timer -= Time.deltaTime;
            // Debug.Log(despawn_timer);
            if (despawn_timer <= 0.0f) {
                gameObject.transform.localScale *= 0.9f;
                if (gameObject.transform.localScale.magnitude < 0.05f) Destroy(gameObject);
            }
        } 

        if (Input.GetKeyDown(KeyCode.E)) ChangeState();

    }


    void OnCollisionEnter(Collision col) {

        // Debug.Log(col.collider.transform.parent.name);
        if (col.collider.transform.root.tag == "Player") {
            // Play open animation
            // Instantiate contents (add inital force?)
            // TODO: Create floor item GameObject that stores a real game object but is smaller and floats and spins and glows.
            
            // if (!opened) ChangeState();
            ChangeState();
        }
    }

    void ChangeState() {
        // If state is unopen, open it
        opened = !opened;

        if (opened) {
            state_unopen.SetActive(false);
            state_open.SetActive(true);
            Open();
        }
        else {
            state_open.SetActive(false);
            state_unopen.SetActive(true);
        }

        // state_open.SetActive(!state_open.activeSelf);
        // state_unopen.SetActive(!state_unopen.activeSelf);

    }

    void Open() {
        // Disable collider
        foreach (Collider c in gameObject.GetComponents<Collider>()) {
            c.enabled = false;
        }

        // var rb = gameObject.GetComponent<Rigidbody>();

        // rb.isKinematic = false;
        // rb.velocity = Vector3.zero;
        // rb.useGravity = false;


        for (int i = 0; i < item_count; i++) {
            Instantiate(spawnable, transform.position, transform.rotation);
            // Instantiate(spawnable, this.transform);
        }

    }

    void OnTriggerEnter(Collider col) {
        Debug.Log("Trigger Hit");
        ChangeState();
    }

}