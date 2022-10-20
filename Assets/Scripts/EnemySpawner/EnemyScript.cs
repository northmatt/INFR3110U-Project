using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private EnemySpawner enemySpawn;

    public void takeDamage()
    {
        Destroy(gameObject);
        enemySpawn = FindObjectOfType<EnemySpawner>();
        enemySpawn.enemyCount--;
        if(enemySpawn.spawnTime <= 0 && enemySpawn.enemyCount <= 0)
        {
            enemySpawn.spawnerDone = true;
        }
    }
    
}
