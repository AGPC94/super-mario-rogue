using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float speed;
    [SerializeField] float direction;

    void Update()
    {
        Vector2 position = transform.position;
        position.y += 1f;

        Debug.DrawRay(position, Vector2.up * .5f);

        if (Physics2D.Raycast(position, Vector2.up, .5f, whatIsGround))
            direction = -1;
        else
            direction = 1;

        transform.Rotate(new Vector3(0, 0, speed * direction) * Time.deltaTime);
    }
}
