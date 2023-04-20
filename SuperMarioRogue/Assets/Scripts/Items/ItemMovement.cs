using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    [Header("Grow")]
    [SerializeField] float delay;
    [SerializeField] float upSpeed;
    [SerializeField] float timeMoveUp;
    [Header("Movement")]
    [SerializeField] float hspeed;
    [Header("Bounce")]
    [SerializeField] bool bounces;
    [SerializeField] bool startTobounce;
    [SerializeField] float jumpHeight;
    [Header("TurnAround")]
    float distance = 0.6f;
    float direction;
    [SerializeField] LayerMask whatIsGround;
    /*
    Controller2D controller;
    float maxJumpHeight = 4;
    float maxJumpVelocity;
    float gravity;
    float fallMaxSpeed;
    Vector2 velocity;
    */

    Rigidbody2D rb;


    public float Delay { get => delay; set => delay = value; }

    void Awake()
    {
        //controller = GetComponent<Controller2D>();
        //gravity = -((2 * maxJumpHeight) / Mathf.Pow(gravityScale, 2));
        //maxJumpVelocity = Mathf.Abs(gravity) * gravityScale;

        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveUp());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        //Turn
        if (rb.velocity.x > 0)
            direction = -1;
        else
            direction = 1;
        
        Debug.DrawRay(transform.position, Vector2.left * distance * direction, Color.yellow);

        if (rb.velocity.x != 0 && Physics2D.Raycast(transform.position, Vector2.left, distance * direction, whatIsGround))
        {
            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
        }

        //Bounce
        Debug.DrawRay(transform.position, Vector2.down * distance);

        if (bounces && Physics2D.Raycast(transform.position, Vector2.down, distance, whatIsGround) && startTobounce)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale)));
        }
    }

    IEnumerator MoveUp()
    {
        AudioManager.instance.Play("PowerUpAppears");

        rb.velocity = new Vector2(0, upSpeed);
        rb.gravityScale = 0;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;

        yield return new WaitForSeconds(timeMoveUp);

        rb.velocity = new Vector2(hspeed, 0);
        rb.gravityScale = 3;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        startTobounce = true;
    }
}
