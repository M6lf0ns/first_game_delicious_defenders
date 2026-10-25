using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirectionController : MonoBehaviour
{
    Transform pm;
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>().transform;



    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = pm.position;
    }
}
