using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Enemy
{
    [Header("Walk")]
    [SerializeField] protected float hSpeed;
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float direction;

    [Header("Turn")]
    [SerializeField] float distance = 1;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] bool canTurn;
    [SerializeField] bool hasTurn;

    [Header("Jump")]
    [SerializeField] float maxJumpHeight;
    [SerializeField] protected bool canJump;
    float maxJumpVelocity;

    [Header("Y Speed")]
    [SerializeField] float fallMaxSpeed;
    [SerializeField] float gravityScale;
    float gravity;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hSpeed = walkSpeed;

        controller = GetComponent<Controller2D>();

        gravity = -((2 * maxJumpHeight) / Mathf.Pow(gravityScale, 2));
        maxJumpVelocity = Mathf.Abs(gravity) * gravityScale;
        velocity.x = -hSpeed;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        ApplyMovement();

        if (health > 0)
        {
            ReverseSpeedInCollision();
            Flip();
            Turn();
            Bounce();
        }
    }

    public override void Stomp()
    {
        HitboxEnemy hb = transform.GetComponentInChildren<HitboxEnemy>();
        hb.gameObject.SetActive(false);
        controller.ActiveCollisions(false);
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Stomp");
        gravity = 0;
        velocity = Vector2.zero;
        Destroy(gameObject, 1);
    }

    void ReverseSpeedInCollision()
    {
        if (controller.collisions.right && velocity.x > 0)
            velocity.x = -hSpeed;

        if (controller.collisions.left && velocity.x < 0)
            velocity.x = +hSpeed;

        //if ((controller.collisions.left || controller.collisions.right) && velocity.x != 0)
            //velocity.x *= -1;
    }

    void Flip()
    {

        Vector2 scale = transform.localScale;

        if (velocity.x > 0)
        {
            Debug.Log("Dcha");
            direction = 1;
            scale.x = -1;
        }
        else
        {
            Debug.Log("Izq");
            direction = -1;
            scale.x = 1;
        }

        transform.localScale = scale;
    }

    public virtual void Turn()
    {
        if (!canTurn)
            return;

        Vector2 pos = new Vector2(transform.position.x, transform.position.y) + new Vector2(.5f * direction, 0);

        Debug.DrawRay(pos, Vector2.down * distance, Color.yellow);

        if (Physics2D.Raycast(pos, Vector2.down, distance, whatIsGround))
            hasTurn = false;
        else if (!hasTurn)
        {
            velocity.x *= -1;
            hasTurn = true;
        }
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

    void Bounce()
    {
        if (canJump && controller.collisions.below)
            velocity.y = maxJumpVelocity;
    }
}
