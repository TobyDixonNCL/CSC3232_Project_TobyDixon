using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffController : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject projectile = GameObject.Find("Projectile");

    void Start() {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform p = gameObject.transform.root.transform;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {

            Vector3 target = hit.point.normalized * 100.0f;
            target.y = p.position.y;

            // Instantiate a projectile in that direction
            Instantiate(projectile, transform.position, transform.rotation);

            // Debug.DrawLine(p.position, target, Color.green, 5.0f);
        }
    }
}
