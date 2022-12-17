using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform target;
	Vector3 offset;


	// Use this for initialization
	void Start () {

        offset = new Vector3(-200.0f, 250.0f, -200.0f);
        // 15 y -15 z
        transform.position = target.position + offset;
        transform.rotation = Quaternion.Euler(30.0f, 45.0f, 0);

	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = Vector3.MoveTowards(target.position, target.position + offset,
        Vector3.Distance(target.position + offset, transform.position) / 100.0f);
        
	}
}
