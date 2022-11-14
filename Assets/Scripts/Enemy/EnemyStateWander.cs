using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateWander : EnemyState {
    Transform player;
    NavMeshAgent enemyNavAgent;
    EnemyController enemyScriptReference;

    public EnemyStateWander(EnemyController enemyScriptReference, Transform player, NavMeshAgent enemyNavAgent) {
        this.enemyScriptReference = enemyScriptReference;
        this.player = player;
        this.enemyNavAgent = enemyNavAgent;
    }

    public void EnterState() {

    }

    public void ExitState() {

    }

    public void UpdateState() {
        if (enemyScriptReference.currentAgroTime > 0f) {
            enemyScriptReference.ChangeState(EnemyStateNames.Chase);
            return;
        }

        //if enemy is outside the wander range then only update the nav dest every five seconds
        if (Vector3.Distance(player.position, enemyScriptReference.transform.position) >= enemyScriptReference.wanderRange) {
            if (enemyScriptReference.navUpdateTime <= 0f) {
                enemyScriptReference.changeNavDest(player.position, enemyScriptReference.wanderRange - 1f);
                enemyScriptReference.navUpdateTime = 5f;
            }

            enemyScriptReference.navUpdateTime -= Time.fixedDeltaTime;

            return;
        }

        //change the dest just before it reaches it's destination
        if (Vector3.Distance(enemyScriptReference.transform.position, enemyNavAgent.destination) <= enemyNavAgent.stoppingDistance + 1f)
            enemyScriptReference.changeNavDest(player.position, enemyScriptReference.wanderRange - 1f);
    }
}
