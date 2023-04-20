using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowser : Enemy
{
    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float timeToReverse;

    [Header("Flame")]
    [SerializeField] int nProjectiles;
    [SerializeField] float timeBetweenProjectiles;
    [SerializeField] float timeResetProjectiles;
    [SerializeField] float flameForce;
    [SerializeField] GameObject flamePrefab;

    [Header("Jump")]
    [SerializeField] float jumpTime;
    [SerializeField] float jumpHeight;
    float maxJumpVelocity;

    [Header("Y Speed")]
    [SerializeField] float fallMaxSpeed;
    [SerializeField] float gravityScale;

    [Header("Coroutines")]
    [SerializeField] bool isStarted;

    float direction;
    float gravity;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        gravity = -((2 * jumpHeight) / Mathf.Pow(gravityScale, 2));
        maxJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            ApplyMovement();
            WatchPlayer();
        }
        else
        {
            transform.Translate(Vector2.down * 10 * Time.unscaledDeltaTime);
        }
    }

    public override void Stomp()
    {
        /*
        HitboxEnemy hb = transform.GetComponentInChildren<HitboxEnemy>();
        hb.gameObject.SetActive(false);
        controller.collider.isTrigger = true;
        animator.SetTrigger("Hit");
        velocity = new Vector2(0, -5);
        StopAllCoroutines();
        */
    }

    public void Die()
    {
        health = 0;
    }

    public void Stop()
    {
        StopAllCoroutines();
        HitboxEnemy hb = transform.GetComponentInChildren<HitboxEnemy>();
        hb.gameObject.SetActive(false);
        velocity = new Vector2(0, 0);
    }

    void ApplyMovement()
    {
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (controller.collisions.above || controller.collisions.below)
            if (controller.collisions.slidingDownMaxSlope)
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            else
                velocity.y = 0;

        if (velocity.y < -fallMaxSpeed)
            velocity.y = -fallMaxSpeed;
    }

    void WatchPlayer()
    {
        Player player = FindObjectOfType<Player>();

        Vector2 scale = transform.localScale;

        if (transform.position.x > player.transform.position.x)
        {
            scale.x = 1;
            direction = -1;
            if (!isStarted)
            {
                isStarted = true;
                StartCoroutines();
                velocity.x = -walkSpeed;
            }
        }
        else
        {
            scale.x = -1;
            direction = 1;
            if (isStarted)
            {
                isStarted = false;
                StopAllCoroutines();
                velocity.x = runSpeed;
                animator.SetTrigger("Walk");
            }
        }
        
        transform.localScale = scale;
    }

    void StartCoroutines()
    {
        StartCoroutine(Walk());
        StartCoroutine(ThrowFlame());
        StartCoroutine(Jump());
    }

    IEnumerator Walk()
    {
        while (true)
        {
            velocity.x = -walkSpeed;
            yield return new WaitForSeconds(timeToReverse);
            velocity.x = walkSpeed;
            yield return new WaitForSeconds(timeToReverse);
        }
    }

    IEnumerator ThrowFlame()
    {
        while (true)
        {
            nProjectiles = Random.Range(1, 4);

            for (int i = 0; i < nProjectiles; i++)
            {
                yield return new WaitForSeconds(timeBetweenProjectiles);
                animator.SetTrigger("Charge");

                yield return new WaitForSeconds(timeBetweenProjectiles);
                animator.SetTrigger("Walk");

                Projectile hammer = Instantiate(flamePrefab, transform.position + new Vector3(direction, 0.5f, 0), Quaternion.identity).gameObject.GetComponent<Projectile>();
                hammer.Move(direction);

                if (GetComponent<SpriteRenderer>().isVisible)
                    AudioManager.instance.Play("BowserFire");
            }
            yield return new WaitForSeconds(timeResetProjectiles);
        }
    }

    IEnumerator Jump()
    {
        while (true)
        {
            yield return new WaitForSeconds(jumpTime);

            if (controller.collisions.below)
                velocity.y = maxJumpVelocity;
        }
    }
}