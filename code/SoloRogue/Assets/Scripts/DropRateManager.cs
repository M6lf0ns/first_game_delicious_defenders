using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name; //name of the drop
        public GameObject itemPrefab; //gameObject that corresponds to the drop
        public float dropRate; //probability that the gameObject will be dropped

    }

    public List<Drops> drops;

    void OnDestroy()
    {

        if (!gameObject.scene.isLoaded) //Stops the spawning error from appearing when exiting play mode
        {
            return;
        }

        float randomNumber = UnityEngine.Random.Range(0f, 100f); //randomly generated dropRate
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops rate in drops) //for each object of type Drops in drops list
        {
            if (randomNumber <= rate.dropRate) //check if the generated dropRate is smaller or equal to the dropRate of the iterated object
            {//each item in drops (rate) has a fixed dropRate


                possibleDrops.Add(rate); //we add the current item to the list of possible drops

            }
            //Check for possible drops
            if (possibleDrops.Count > 0)
            {
                Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)]; //choose one item in possible drops at a randomly generated index
                Instantiate(drops.itemPrefab, transform.position, Quaternion.identity); //we spawn the Prefab of the object at the index of rate
                // and spawn it at the original position that the object that has this script is at
            }
        }
    }
}
