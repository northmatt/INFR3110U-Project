using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyButton : MonoBehaviour {
    private EnemyFactory factory;

    private EditorController editor;

    TextMeshProUGUI btnText;

    GameObject enemy;

    private void Start()  {
        factory = GameController.instance.gameObject.GetComponent<EnemyFactory>();
        editor = EditorController.instance;

        btnText = GetComponentInChildren<TextMeshProUGUI>();
    }    

    public void OnClickSpawn() {
        switch(btnText.text) {
            case "crab":
                editor.instantiatedPrefab = factory.GetEnemy("crab").Create(factory.enemy1Prefab);
                break;
            case "monster":
                editor.instantiatedPrefab = factory.GetEnemy("monster").Create(factory.enemy2Prefab);
                break;
            default:
                break;
        }
    }
}
