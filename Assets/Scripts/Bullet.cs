using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    CharController player;

    public float damage = 1;

    private void Awake()
    {
        player = GameController.instance.player.GetComponent<CharController>();
    }

    private void OnEnable()
    {
        transform.GetComponent<Rigidbody>().WakeUp();
    }

    private void OnDisable()
    {
        transform.GetComponent<Rigidbody>().Sleep();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (GameController.instance.useObjectPooling)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }

        if (other.collider.tag == "Player")
        {
            player.TakeDamage(damage);
        }
        else if (other.collider.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
    }
}