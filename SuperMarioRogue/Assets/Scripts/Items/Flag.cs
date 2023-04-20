using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveDown()
    {
        rb.velocity = Vector2.down * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            rb.velocity = Vector2.zero;

            GameManager.instance.Invoke("NextLevel", 0.2f);
            //GameManager.instance.NextLevel();
        }
    }
}
