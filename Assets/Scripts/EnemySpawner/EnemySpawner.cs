using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //create holder for spawnPoints
    public GameObject[] spawnPoints;
    GameObject currentPoint;
    int index;

    public GameObject[] enemies;
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
    public bool canSpawn;
    public float spawnTime;
    public int enemyCount;
    public bool spawnerDone;
    public GameObject spawnerDoneGameObject;


    private void Start()
    {
        Invoke("spawnEnemy", 0.5f);
    }

    private void Update()
    {
        if(canSpawn)
        {
            spawnTime -= Time.deltaTime;
            if(spawnTime < 0)
            {
                canSpawn = false;
            }
        }
    }

    void spawnEnemy()
    {
        index = Random.Range(0, spawnPoints.Length);
        currentPoint = spawnPoints[index];
        float timeBetweenSpawns = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);

        if(canSpawn)
        {
            Instantiate(enemies[Random.Range(0, enemies.Length)], currentPoint.transform.position, Quaternion.identity);
            enemyCount++;
        }
        Invoke("spawnEnemy", timeBetweenSpawns);
        if(spawnerDone)
        {
            spawnerDoneGameObject.SetActive(true);
        }
    }
}
