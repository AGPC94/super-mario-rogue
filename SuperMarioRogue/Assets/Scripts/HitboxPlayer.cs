using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxPlayer : MonoBehaviour
{
    enum AttackType { JUMP, FIRE, HIT}
    [SerializeField] AttackType type;

    [SerializeField] List<GameObject> goHits;

    [SerializeField] bool activateBlocks;

    [SerializeField] float damage = 1;

    void Start()
    {
        goHits = new List<GameObject>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {

            Enemy enemy = other.GetComponent<Enemy>();

            if (!goHits.Contains(enemy.gameObject))
            {
                if (type != AttackType.JUMP)
                    goHits.Add(enemy.gameObject);

                switch (type)
                {
                    case AttackType.JUMP:
                        Player player = transform.parent.GetComponent<Player>();
                        if (!player.Controller.collisions.below && player.Velocity.y < 0 && enemy.IsJumpAffects)
                        {
                            player.BounceEnemy();
                            enemy.JumpDamage(player.Controller.collisions.faceDir);
                        }
                        break;

                    case AttackType.FIRE:
                        enemy.FireDamage(damage);
                        break;

                    case AttackType.HIT:
                        enemy.HitDamage(damage);
                        break;
                }
            }
        }

        if (other.CompareTag("Trampoline"))
        {
            Player player = transform.parent.GetComponent<Player>();
            if (type == AttackType.JUMP)
            {
                player.BounceTrampoline();
                other.transform.parent.GetComponent<Animator>().SetTrigger("Bounce");
            }
        }   
    }

    void OnDisable()
    {
        goHits = new List<GameObject>();
    }
}

/*
    if (other.CompareTag("Block"))
    {
        if (other.TryGetComponent(out Block block))
        {
            if (activateBlocks)
            {
                AudioManager.instance.Play("Bump");
                block.Activate();
            }
        }
    }
*/