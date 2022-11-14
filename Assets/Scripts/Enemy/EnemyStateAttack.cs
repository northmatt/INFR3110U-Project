using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateAttack : EnemyState {
    EnemyController enemyScriptReference;
    
    Animator enemyAnimator;
    NavMeshAgent enemyNavAgent;
    Transform player;

    public EnemyStateAttack(EnemyController enemyScriptReference, Transform player, NavMeshAgent enemyNavAgent, Animator enemyAnimator) {
        this.enemyScriptReference = enemyScriptReference;

        this.player = player;
        this.enemyNavAgent = enemyNavAgent;
        this.enemyAnimator = enemyAnimator;
    }

    public void EnterState() {

    }

    public void ExitState() {

    }

    public void UpdateState() {
        enemyScriptReference.isAttacking = enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack");

        //enemy cannot move while attacking
        enemyNavAgent.enabled = !enemyScriptReference.isAttacking;

        //Change to maxAttackRange?
        if (!enemyScriptReference.isAttacking && (!enemyScriptReference.playerInSight || Vector3.Distance(enemyScriptReference.transform.position, player.transform.position) >= enemyNavAgent.stoppingDistance)) {
            enemyScriptReference.ChangeState(EnemyStateNames.Chase);

            return;
        }

        if (!enemyScriptReference.isAttacking)
            enemyAnimator.SetBool("isAttacking", true);

        if (enemyScriptReference.fireCooldown <= 0) {
            enemyScriptReference.Shoot();
        }

        //consider moving code so that enemy cant change direction during attack animation, prolly too OP
        //consider using Chase state for changing directions with above in mind, also smoothed rotation
        Vector3 tempVector = player.transform.position - enemyScriptReference.transform.position;
        tempVector.y = 0;
        enemyScriptReference.transform.rotation = Quaternion.LookRotation(tempVector);
    }
}
