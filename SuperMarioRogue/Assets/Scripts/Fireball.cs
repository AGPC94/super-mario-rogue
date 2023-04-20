using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float jumpHeight;

    bool isDestroy;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        
        Debug.DrawRay(transform.position, Vector2.down * distance);

        if (Physics2D.Raycast(transform.position, Vector2.down, distance, whatIsGround))
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale)));
        }

        if (Physics2D.Raycast(transform.position, Vector2.right * distance, distance, whatIsGround) || Physics2D.Raycast(transform.position, Vector2.left * distance, distance, whatIsGround))
        {
            DestroyFireball();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            DestroyFireball();
        }
    }

    void DestroyFireball()
    {
        if (!isDestroy)
        {
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
            GetComponent<Collider2D>().enabled = false;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            transform.Find("Explosion").GetComponent<ParticleSystem>().Play();
            AudioManager.instance.Play("Bump");
            isDestroy = true;
            Destroy(gameObject, .5f);
        }
    }
}
