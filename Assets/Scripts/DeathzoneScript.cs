using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathzoneScript : MonoBehaviour
{
    void OnCollisionEnter(Collision col) {
        if (col.collider.transform.name != "Player") {
            Destroy(col.collider.gameObject);
        } else {
            col.collider.gameObject.transform.position = new Vector3(0, 3, -3);
        }
    }
}
