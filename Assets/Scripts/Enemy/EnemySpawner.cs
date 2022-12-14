using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    //create holder for spawnPoints
    public GameObject[] spawnPoints;
    public GameObject[] enemies;
    public Transform instantiateParent;
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
    public float spawnTime;
    public int enemyCount;

    private void Start() {
        Invoke("spawnEnemy", 0.5f);
    }

    private void FixedUpdate() {
        if (spawnTime >= 0f)
            spawnTime -= Time.fixedDeltaTime;
    }

    void spawnEnemy() {
        float timeBetweenSpawns = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);

        if(spawnTime >= 0f) {
            ++enemyCount;

            GameObject instantiatedPrefab = Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity, instantiateParent);
            if (Random.value > 0.5f)
                EditorController.instance.observers.AddObserver(new InstantiatedObject2(instantiatedPrefab, new InstantiatedObjectEvent1()));
            else
                EditorController.instance.observers.AddObserver(new InstantiatedObject2(instantiatedPrefab, new InstantiatedObjectEvent2()));

            ++enemyCount;
        }
        else {
            enabled = false;
        }

        Invoke("spawnEnemy", timeBetweenSpawns);
    }
}
