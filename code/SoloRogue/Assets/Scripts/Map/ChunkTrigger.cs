using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    MapController mc;
    public GameObject targetMap;
    void Start()
    {
        mc = FindObjectOfType<MapController>();
    }

    private void OnTriggerStay2D(Collider2D col) //find out which chunk the player is standing on
    {
        if(col.CompareTag("Player")) // if player is within collider of chunk
        { 
            mc.currentChunk = targetMap; //chunk is the target
            
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.CompareTag("Player")) //same thing but exiting
        {
            if(mc.currentChunk == targetMap) //if chunk being exited is target chunk
            {
                mc.currentChunk = null; //player is no longer on that chunk
            }
        }
    }
    
}
