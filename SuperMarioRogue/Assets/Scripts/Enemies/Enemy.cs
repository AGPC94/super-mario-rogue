using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] public float health;
    [SerializeField] private bool isJumpAffects;
    [SerializeField] private bool isFireAffects;

    //[SerializeField] protected bool isHitAffects;

    HitboxEnemy hitbox;
    protected Animator animator;
    protected Controller2D controller;
    protected Vector2 velocity;

    public bool IsJumpAffects { get => isJumpAffects; set => isJumpAffects = value; }
    public bool IsFireAffects { get => isFireAffects; set => isFireAffects = value; }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }

    public virtual void Start()
    {
        hitbox = GetComponent<HitboxEnemy>();
        animator = GetComponent<Animator>();
        controller = GetComponent<Controller2D>();
        enabled = false;
    }

    public virtual void JumpDamage(float direction)
    {
        if (isJumpAffects)
        {
            health -= 1;
            if (health <= 0)
                Stomp();
        }
    }

    public void FireDamage(float dmg)
    {
        if (isFireAffects)
            HitDamage(dmg);
    }

    public void HitDamage(float dmg)
    {
        AudioManager.instance.Play("Kick");
        health -= dmg;
        if (health <= 0)
            Hit();
    }

    public virtual void Stomp()
    {
        HitboxEnemy hb = transform.GetComponentInChildren<HitboxEnemy>();
        hb.gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().flipY = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
    }

    public void Hit()
    {
        HitboxEnemy hb = transform.GetComponentInChildren<HitboxEnemy>();
        hb.gameObject.SetActive(false);

        if (controller != null)
            controller.ActiveCollisions(false);
        if (animator != null)
            animator.SetTrigger("Hit");

        if (GetComponent<Rigidbody2D>() != null && GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
        }
        else
            velocity = new Vector2(0, 10);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().flipY = true;
    }
}