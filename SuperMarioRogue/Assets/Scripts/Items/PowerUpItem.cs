using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    [SerializeField] PowerUp powerUp;

    void Awake()
    {
        if (powerUp.aniPowerUp != null)
            GetComponent<Animator>().runtimeAnimatorController = powerUp.aniPowerUp;

        GetComponent<SpriteRenderer>().sprite = powerUp.sprite;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            player.SetPowerUpWithTransformation(powerUp);
            AudioManager.instance.Play("PowerUp");
            Destroy(gameObject);
        }
    }
}

/*
Movimiento del item

PowertUp/Moneda/Chanpiñon...

*/