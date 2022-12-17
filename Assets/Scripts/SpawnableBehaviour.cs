using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableBehaviour : MonoBehaviour
{   

    private enum Rarity {
        common,
        uncommon,
        rare,
        unique,
        legendary
    }

    GameObject weapon;

    private Vector3 offset;
    // private Rigidbody rb;

    [SerializeField] private Material mat;

    void Start()
    {   

        // rb = gameObject.GetComponent<Rigidbody>();

        // Get random weapon
        GameObject[] spawnables = Resources.LoadAll<GameObject>("WeaponModels");
        weapon = spawnables[Random.Range(0, spawnables.Length)];

        // weapon = Instantiate(weapon, gameObject.transform.position, Quaternion.Euler(45f, 45f, 45f));
        weapon = Instantiate(weapon, transform);

        // weapon.transform.parent = gameObject.transform;
        // weapon.transform.localScale = Vector3.one * 0.8f;

        
        // float x_mod = Random.Range(10, -10);
        // float z_mod = Random.Range(0, -3);
        // rb.AddForce(new Vector3(x_mod, 10.0f, z_mod), ForceMode.VelocityChange);


        // Define offset position
        offset = new Vector3(Random.Range(-3, 6), Random.Range(0, 3), Random.Range(-3, 3));

        // Change child material
        Material[] weaponMats = weapon.GetComponent<MeshRenderer>().materials;
        

        for (int i = 0; i < weaponMats.Length; i++) {
            weaponMats[i] = mat;
        }

        weapon.GetComponent<MeshRenderer>().materials = weaponMats;

    }

    // Update is called once per frame
    void Update() {
        var step = Vector3.MoveTowards(
            transform.position,
            Camera.main.transform.position + offset,
            2f * Time.deltaTime
        );
        transform.position = step;
    }

    void OnCollisionEnter(Collision col) {
        if (col.collider.transform.root.tag == "Spawnable") 
            Physics.IgnoreCollision(col.collider, gameObject.GetComponent<Collider>(), true);
        if (col.collider.transform.root.tag == "Player") {

            if (Input.GetKey(KeyCode.E)) {
                Physics.IgnoreCollision(col.collider, gameObject.GetComponent<Collider>(), true);
                // transform.Translate(Vector3.Lerp(col.collider.transform.position, -gameObject.transform.position, 1.0f));

                Destroy(gameObject);

            }
            
        }
    }

    Rarity GetRarity() {

        // Each rarity should change material color
        int r = Random.Range(0, 100);

        if (r < 40) return Rarity.common;
        else if (r < 60) return Rarity.uncommon;
        else if (r < 80) return Rarity.rare;
        else if (r < 95) return Rarity.unique;
        else return Rarity.legendary;
    }   
}
