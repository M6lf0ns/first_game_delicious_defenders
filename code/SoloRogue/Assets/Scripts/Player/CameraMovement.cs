using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
   private Transform target; //reference what we are tracking --> Player
    [SerializeField]
   public Vector3 offset; //give camera flexibility?

    void Start()
    {
        
    }

    
    void Update()
    {
        transform.position = target.position + offset;
    }
}
