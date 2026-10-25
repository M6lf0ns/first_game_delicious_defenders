using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints; //List of the prop location Objects
    public List<GameObject> propPrefabs; //List of prop Prefabs
    void Start()
    {
        SpawnProps();
    }

    
    void Update()
    {
        
    }

    void SpawnProps() //used to spawn random props (sprites) onto the prop location
    {
        foreach (GameObject sp in propSpawnPoints) //for every prop in the prop location list
        {
            int rand = Random.Range(0,propPrefabs.Count); //creates a variable that chooses a Random index from the range from 0 to number of prefabs
            GameObject prop = Instantiate(propPrefabs[rand], sp.transform.position, Quaternion.identity); //make a copy of the Object in propPrefabs list at the index rand, give it the position of the current prop location in the loop and give it a rotation of zero
            prop.transform.parent = sp.transform; //so spawned prop is parented by the location it spawned at
        } 

    }
}
