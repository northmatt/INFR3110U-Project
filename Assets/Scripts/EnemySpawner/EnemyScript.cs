using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private EnemySpawner enemySpawn;

    public void takeDamage() {
        Destroy(gameObject);
        enemySpawn = FindObjectOfType<EnemySpawner>();
        enemySpawn.enemyCount--;
    }
    
}
