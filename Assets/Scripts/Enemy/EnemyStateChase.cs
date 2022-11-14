using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateChase : EnemyState {
    NavMeshAgent enemyNavAgent;
    Transform player;
    EnemyController enemyScriptReference;

    public EnemyStateChase(EnemyController enemyScriptReference, Transform player, NavMeshAgent enemyNavAgent) {
        this.enemyScriptReference = enemyScriptReference;
        this.player = player;
        this.enemyNavAgent = enemyNavAgent;
    }

    public void EnterState() {

    }

    public void ExitState() {

    }

    public void UpdateState() {
        if (!enemyScriptReference.playerInSight)
            enemyScriptReference.currentAgroTime -= Time.fixedDeltaTime;

        if (enemyScriptReference.currentAgroTime <= 0f) {
            enemyScriptReference.currentAgroTime = 0f;
            enemyScriptReference.ChangeState(EnemyStateNames.Wander);

            return;
        }


        if (Vector3.Distance(enemyScriptReference.transform.position, player.transform.position) <= enemyNavAgent.stoppingDistance && enemyScriptReference.playerInSight || enemyScriptReference.isAttacking) {
            enemyScriptReference.ChangeState(EnemyStateNames.Attack);

            return;
        }

        enemyScriptReference.changeNavDest(player.transform.position);
    }
}
