using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateKoopaInPlatform : MonoBehaviour
{
    [SerializeField] Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, size, 0, Vector2.zero, 0);
        if (hit.collider.CompareTag("Through"))
            transform.Find("KoopaGreen").gameObject.SetActive(false);
        else
            transform.Find("KoopaRed").gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, size);
    }
}
