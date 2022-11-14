using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthController : MonoBehaviour {
    public static StealthController instance;

    private List<EnemyController> subjects = new List<EnemyController>();

    void Start() {
        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject curEnemy in enemies)
            AddSubjectToObserver(curEnemy.GetComponent<EnemyController>());
    }

    public void NotifyAll(Vector3 soundPosition, float soundDB) {
        foreach (EnemyController curEnemy in subjects)
            curEnemy.OnNotify(soundPosition, soundDB);
    }

    private void AddSubjectToObserver(EnemyController enemyController) {
        subjects.Add(enemyController);
    }
}
