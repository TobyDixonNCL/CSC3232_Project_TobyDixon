using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
TODO:
- Animation for opneing
- Instantiate spawnables but more controller, no adding velocity to rb randomly.
- Mouse over and click for opening
*/

public class ChestSceneController : MonoBehaviour
{
    private bool opened = false;
    private float despawn_timer = 2.0f;

    private int item_count;

    public GameObject spawnable;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
