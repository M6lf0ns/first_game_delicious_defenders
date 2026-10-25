using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))] //Ensures that PlayerCollector always comes with a circle Collider
public class PlayerCollector : MonoBehaviour
{

    PlayerStats player;
    CircleCollider2D detector;
    public float pullSpeed;




    private void Start()
    {
        player = GetComponentInParent<PlayerStats>(); //Collector is child of player

    }


    public void SetRadius(float r) //Will be called everytimne the radius size changes 
    {
        if (!detector) detector = GetComponent<CircleCollider2D>();
        detector.radius = r;
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        //Check if GameObject is a Pickup.
        if (col.TryGetComponent(out PickUp p))
        {
            p.Collect(player, pullSpeed); //Gives over the two parameters needed for Collect() in Pickup class, lifespan is optional
        }


    }
}


