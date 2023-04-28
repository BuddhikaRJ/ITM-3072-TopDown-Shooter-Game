using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    int spawnPointID;
    [SerializeField] GameObject enemyPrefab;
    float  spawnTicks = 0;
    [SerializeField] float spawnWaitTime = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //wait for some time
        spawnTicks += Time.deltaTime;
        spawnTicks = spawnTicks > spawnWaitTime ? spawnWaitTime : spawnTicks;

        if(spawnTicks == spawnWaitTime){
            //spawn enemy
            spawnPointID = Random.Range(0,spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnPointID];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            //reset spawn timer
            spawnTicks = 0;
        }
        
    }
}
