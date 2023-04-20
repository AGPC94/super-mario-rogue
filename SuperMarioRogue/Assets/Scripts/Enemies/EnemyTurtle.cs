using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TurtleState { NORMAL, SHELL, SHELLMOVING, RECOVER }
public class EnemyTurtle : EnemyMovement
{
    [Header("Turtle")]
    [SerializeField] TurtleState state = TurtleState.NORMAL;
    [SerializeField] float shellSpeed;
    [SerializeField] float recoveringPhase1;
    [SerializeField] float recoveringPhase2;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] LayerMask whatIsPlatform;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        if (canJump)
            animator.SetTrigger("Bounce");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Push();
        ActivateBlocks();

        if (state == TurtleState.SHELLMOVING && health > 0)
            transform.Find("HitboxShell").gameObject.SetActive(true);
        else
            transform.Find("HitboxShell").gameObject.SetActive(false);

        if (health <= 0)
            state = TurtleState.NORMAL;
    }

    public override void JumpDamage(float direction)
    {
        if (canJump)
        {
            canJump = false;
            animator.SetTrigger("Walk");
        }
        else
        {
            switch (state)
            {
                case TurtleState.NORMAL:
                    StartCoroutine(Recovering());
                    break;

                case TurtleState.SHELL:
                    state = TurtleState.SHELLMOVING;
                    hSpeed = shellSpeed;
                    velocity.x = hSpeed * direction;
                    animator.SetTrigger("ShellMoving");
                    StopAllCoroutines();
                    break;

                case TurtleState.SHELLMOVING:
                    StartCoroutine(Recovering());
                    break;

                case TurtleState.RECOVER:
                    StopAllCoroutines();
                    StartCoroutine(Recovering());
                    break;
            }
        }
    }

    public void Push()
    {
        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector2.right, 0.6f, whatIsPlayer);
        Debug.DrawRay(transform.position, Vector2.right * 1f);

        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.left, 0.6f, whatIsPlayer);
        Debug.DrawRay(transform.position, Vector2.left * 1f);

        if (hitR )
        {
            if (hitR.collider.TryGetComponent(out Player player))
            {
                if (state == TurtleState.SHELL)
                {
                    state = TurtleState.SHELLMOVING;
                    hSpeed = shellSpeed;
                    velocity.x = hSpeed * player.Controller.collisions.faceDir;
                    animator.SetTrigger("ShellMoving");
                    AudioManager.instance.Play("Kick");
                    StopAllCoroutines();
                }
            }

        }

        if (hitL)
        {
            if (hitL.collider.TryGetComponent(out Player player))
            {
                if (state == TurtleState.SHELL)
                {
                    state = TurtleState.SHELLMOVING;
                    hSpeed = shellSpeed;
                    velocity.x = hSpeed * player.Controller.collisions.faceDir;
                    animator.SetTrigger("ShellMoving");
                    AudioManager.instance.Play("Kick");
                    StopAllCoroutines();
                }
            }

        }
    }

    public void ActivateBlocks()
    {
        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector2.right, 0.6f, whatIsGround);
        Debug.DrawRay(transform.position, Vector2.right * 1f);

        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.left, 0.6f, whatIsGround);
        Debug.DrawRay(transform.position, Vector2.left * 1f);

        if (hitR)
        {
            if (state == TurtleState.SHELLMOVING && GetComponent<SpriteRenderer>().isVisible)
                AudioManager.instance.Play("Bump");

            if (hitR.collider.TryGetComponent(out Block block))
            {
                if (state == TurtleState.SHELLMOVING)
                {
                    hSpeed = shellSpeed;
                    velocity.x = -hSpeed;
                    block.Activate();
                }
            }
        }

        if (hitL)
        {
            if (state == TurtleState.SHELLMOVING && GetComponent<SpriteRenderer>().isVisible)
                AudioManager.instance.Play("Bump");

            if (hitL.collider.TryGetComponent(out Block block))
            {
                if (state == TurtleState.SHELLMOVING)
                {
                    hSpeed = shellSpeed;
                    velocity.x = hSpeed;
                    block.Activate();
                }
            }
        }
    }

    public override void Turn()
    {
        if (state != TurtleState.SHELLMOVING)
            base.Turn();
    }

    IEnumerator Recovering()
    {
        state = TurtleState.SHELL;
        animator.SetTrigger("Shell");
        velocity = Vector2.zero;
        yield return new WaitForSeconds(recoveringPhase1);

        state = TurtleState.RECOVER;
        animator.SetTrigger("ShellRecover");
        yield return new WaitForSeconds(recoveringPhase2);

        state = TurtleState.NORMAL;
        animator.SetTrigger("Walk");
        hSpeed = walkSpeed;
        velocity.x = hSpeed * direction;
    }
}
