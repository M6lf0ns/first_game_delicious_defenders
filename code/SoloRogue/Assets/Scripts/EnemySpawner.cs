using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    float tolerance = 0.1f;
    [System.Serializable]
    public class Wave
    {
        public string waveName; //List of enemy prefabs for this wave
        public List<EnemyGroup> enemyGroups; //List of groups of enemies to be spawned in this wave
        public int waveQuota; //The total number of enemies to spawn in this wave
        public float spawnInterval; //The interval at which to spawn enemies
        public int spawnCount; //The number of enemies of this type already spawned in this wave

    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; //The total number of enemies to spawn in this wave
        public int spawnCount; //The number of enemies of this type already spawned in this wave
        public GameObject enemyPrefab;


    }

    public List<Wave> waves; //List of all the waves in the game
    public int currentWavecount; //Index of the current Wave in the waves list
    Transform player;

    [Header("Spawner Attributes ")]

    public float spawnTimer; //Timer to determine when to spawn next enemy
    public int enemiesAlive; //Track amount of enemies in the field
    public int maxEnemiesAllowed; //Maximum amount of enemies allowed on the map simultaneously
    public bool maxEnemiesReached = false; //flag indicating is maximum number of enemies has been reached
    public float waveInterval; //Interval between each wave
    bool isWaveActive = false;

    [Header("Spawn Positions")]

    public List<Transform> relativeSpawnPoints; //List of all possible enemy Spawn Points



    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();

    }

    void Update()
    {


        if (currentWavecount < waves.Count && GameManager.instance.stopwatchTime % waveInterval < tolerance && !isWaveActive) //If there are still waves left AND the last wave has ended
        {
            StartCoroutine(BeginNextWave());
        }
        spawnTimer += Time.deltaTime;

        //Check if next enemy should be spawned
        if (spawnTimer >= waves[currentWavecount].spawnInterval) //if the spawnTimer exceeds the spawnInterval of the current wave
        {
            spawnTimer = 0f; //reset timer
            SpawnEnemies();
        }


    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true; //A wave is active currently
        //Wait "waveInterval" seconds before initiating next wave
        yield return new WaitForSeconds(waveInterval);

        if (currentWavecount < waves.Count - 1) //If there are more waves after currentWave
        {
            isWaveActive = false; //there is no active wave
            Debug.Log(isWaveActive);
            currentWavecount++;
            CalculateWaveQuota();
        }
    }
    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0; //resets quota everytime function is called
        foreach (var enemyGroup in waves[currentWavecount].enemyGroups) //for each enemyGroup in the list of enemyGroups in the current wave
        {
            currentWaveQuota += enemyGroup.enemyCount; //add the enemyCount of the current enemyGroup to currentWaveQuota, accumulates total number of enemies spawned in the wave
        }
        waves[currentWavecount].waveQuota = currentWaveQuota; //Set wave Quota of the current wave count element to the current wave quota
        Debug.LogWarning(currentWaveQuota); //should amount to total number of enemies set in Inspector
    }
    /// <summary>
    /// Method will stop spawning enemies if the amount of enemies on the map is maximum
    /// </summary>
    void SpawnEnemies()
    {
        if (/*waves[currentWavecount].spawnCount < waves[currentWavecount].waveQuota && */!maxEnemiesReached)
        {
            if (!gameObject.scene.isLoaded) //Stops the spawning error from appearing when exiting play mode
            {
                return;
            }



            foreach (var enemyGroup in waves[currentWavecount].enemyGroups) //for each enemyGroup int the enemyGroups int the current wave
            {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount || waves[currentWavecount].spawnCount < waves[currentWavecount].waveQuota) //if the spawned Enemies of the current enemyGroup has not reached its total
                {

                    //Pick a random enemy group out of the wave to spawn
                    int randEnemyGroup = UnityEngine.Random.Range(0, waves[currentWavecount].enemyGroups.Count);


                    //Spawn enemy at a random Spawn Point close to player
                    Instantiate(waves[currentWavecount].enemyGroups[randEnemyGroup].enemyPrefab, player.position + relativeSpawnPoints[UnityEngine.Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);
                    //Debug.Log("enemyGroup" + enemyGroup.enemyName);
                    enemyGroup.spawnCount++; //increase spawnCount of current enemyGroup
                    waves[currentWavecount].spawnCount++; //increase the spawncount at the current wave
                    enemiesAlive++;

                    //Limits amount of enemies that can be spawned at once 
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }

    }
    //Call when enemy is killed
    public void OnEnemyKilled()
    {
        //Decrement number of enemies alive
        enemiesAlive--;
        //Debug.Log("enemies Alive" + enemiesAlive);
        //Decrease spawnCount in order to keep continously spawning enemies
        waves[currentWavecount].spawnCount--;



        //Reset the maxEnemiesReached flag if number of enemies alive has dropped below the max
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
            SpawnEnemies();

        }
    }
}
