using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxShell : MonoBehaviour
{
    [SerializeField] List<GameObject> goHits;
    void Start()
    {
        goHits = new List<GameObject>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().StartHurt(1);
        }

        if (other.CompareTag("Enemy"))
        {
            if (!goHits.Contains(other.gameObject))
            {
                goHits.Add(other.gameObject);
                other.GetComponent<Enemy>().HitDamage(999);
                AudioManager.instance.Play("Kick");
            }
        }
    }
}
