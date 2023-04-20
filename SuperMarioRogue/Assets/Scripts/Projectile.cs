using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float hForce;
    [SerializeField] float vForce;
    [SerializeField] float gravity = 0;

    public float HForce { get => hForce; set => hForce = value; }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 scale = transform.localScale;
        if (rb.velocity.x > 0)
            scale.x = -1;
        else
            scale.x = 1;
        transform.localScale = scale;

        if (!GetComponent<SpriteRenderer>().isVisible)
            Destroy(gameObject);
    }

    public void InheritSpeed(float speed, float direction)
    {
        hForce += speed * direction;
    }

    public void Move(float direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.velocity = new Vector2(hForce * direction, vForce);
    }
}