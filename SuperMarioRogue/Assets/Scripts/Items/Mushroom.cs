using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.Heal(1);
            AudioManager.instance.Play("PowerUp");
            Destroy(gameObject);
        }
    }
}
