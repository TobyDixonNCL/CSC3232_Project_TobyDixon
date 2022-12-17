using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    private GameObject currentWeapon;
    private GameObject newWeapon;
    
    [SerializeField]
    private GameObject[] weapons;

    // Start is called before the first frame update
    void Start()
    {
        // weapons = Resources.LoadAll<GameObject>("WeaponPrefabs");

        // ChangeWeapon(weapons[Random.Range(0, weapons.Length)]);

        GameObject w = Instantiate(weapons[0], gameObject.transform);
        currentWeapon = w;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ChangeWeapon(GetRandomWeapon());
    }

    public void ChangeWeapon(GameObject newWeapon) {

        newWeapon.transform.localScale = new Vector3(2f, 2f, 2f);
        GameObject w = Instantiate(newWeapon, gameObject.transform);

        Destroy(currentWeapon);        

        // w.transform.rotation = Quaternion.Euler(180f, 90f, 115f);

        currentWeapon = w;
        // w.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y -0.8f, transform.position.z); 

        // weapon = w;
    }

    private GameObject GetRandomWeapon(string name = null) {
        if (name != null) {
            for (int i = 0; i < weapons.Length; i++) {
                if (weapons[i].name == name) return weapons[i];
            }
        }
        return weapons[Random.Range(0, weapons.Length)];
    }
}
