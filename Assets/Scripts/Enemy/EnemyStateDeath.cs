using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateDeath : EnemyState {
    EnemyController enemyScriptReference;

    public EnemyStateDeath(EnemyController enemyScriptReference) {
        this.enemyScriptReference = enemyScriptReference;
    }

    public void EnterState() {
        enemyScriptReference.EnemyDeath();
    }

    public void ExitState() {

    }

    public void UpdateState() {
        
    }
}
