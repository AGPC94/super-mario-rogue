using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBro : Enemy
{
    [Header("Walking")]
    [SerializeField] float walkSpeed;
    [SerializeField] float timeToReverse;

    [Header("ThrowHammer")]
    [SerializeField] int nHammers;
    [SerializeField] float timeBetweenHammers;
    [SerializeField] float timeResetHammers;
    [SerializeField] float hForce;
    [SerializeField] float vForce;
    [SerializeField] GameObject hammerPrefab;

    [Header("Jump")]
    [SerializeField] float jumpTime;
    [SerializeField] float resetCollisionTime;
    [SerializeField] float maxJumpHeight;
    [SerializeField] float minJumpHeight;
    float maxJumpVelocity;
    float minJumpVelocity;

    [Header("Y Speed")]
    [SerializeField] float fallMaxSpeed;
    [SerializeField] float gravityScale;

    float direction;
    float gravity;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        gravity = -((2 * maxJumpHeight) / Mathf.Pow(gravityScale, 2));
        maxJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * maxJumpHeight);
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        StartCoroutine(Walk());
        StartCoroutine(ThrowHammer());
        StartCoroutine(Jump());
    }

    // Update is called once per frame
    void Update()
    {
        WatchPlayer();
        ApplyMovement();
    }

    public override void Stomp()
    {
        HitboxEnemy hb = transform.GetComponentInChildren<HitboxEnemy>();
        hb.gameObject.SetActive(false);
        controller.ActiveCollisions(false);
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Hit");
        velocity = new Vector2(0, -5);
        StopAllCoroutines();
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

        if (player != null)
        {
            Vector2 scale = transform.localScale;

            if (transform.position.x > player.transform.position.x)
            {
                scale.x = 1;
                direction = -1;
            }
            else
            {
                scale.x = -1;
                direction = 1;
            }

            transform.localScale = scale;
        }
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

    IEnumerator ThrowHammer()
    {
        while (true)
        {
            nHammers = Random.Range(1, 5);

            for (int i = 0; i < nHammers; i++)
            {
                yield return new WaitForSeconds(timeBetweenHammers);
                animator.SetTrigger("Charge");

                yield return new WaitForSeconds(timeBetweenHammers);
                animator.SetTrigger("Walk");
                Projectile hammer = Instantiate(hammerPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity).gameObject.GetComponent<Projectile>();
                hammer.Move(direction); //gravity 2
            }

            yield return new WaitForSeconds(timeResetHammers);
        }
    }

    IEnumerator Jump()
    {
        while (true)
        {
            yield return new WaitForSeconds(jumpTime);

            if (controller.collisions.below)
                velocity.y = minJumpVelocity;
        }
    }

    /* 
        Collider2D collider = GetComponent<Collider2D>();
       RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2, controller.collisionMask);
        Debug.DrawRay(transform.position, Vector2.down * 1, Color.blue);
     
            
            if (controller.collisions.below)
            {
                controller.ActiveCollisions(false);

                //Si esta en e suelo salta alto para ponerse en las plataformas
                if (hit.collider.CompareTag("Ground"))
                    velocity.y = maxJumpVelocity;
                //Si no, o salta alto o baja
                else
                {
                    int rnd = Random.Range(1, 101);

                    if (rnd > 0 && rnd <= 80)
                        velocity.y = minJumpVelocity;
                    else
                        velocity.y = maxJumpVelocity;
                }
                
            }
                        controller.ActiveCollisions(true);
     */
}
