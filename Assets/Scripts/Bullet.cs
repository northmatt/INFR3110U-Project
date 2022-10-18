using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet : MonoBehaviour {
    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);

        if(other.collider.CompareTag("Player")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if(other.collider.CompareTag("Enemy")) {
            Destroy(other.gameObject);
        }
    }
}
