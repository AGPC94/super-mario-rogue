using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBill : MonoBehaviour
{
    [SerializeField] GameObject bulletBill;
    [SerializeField] float shotTime;
    float direction;

    ParticleSystem cloudL;
    ParticleSystem cloudR;

    void OnBecameVisible()
    {
        enabled = true;
        StartCoroutine(ShotBullet());
    }

    void OnBecameInvisible()
    {
        enabled = false;
        StopAllCoroutines();
    }
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(ShotBullet());

        cloudL = transform.Find("CloudL").GetComponent<ParticleSystem>();
        cloudR = transform.Find("CloudR").GetComponent<ParticleSystem>();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        WatchPlayer();
    }

    IEnumerator ShotBullet()
    {
        while (true)
        {
            yield return new WaitForSeconds(shotTime);
            Vector2 pos = transform.position;
            pos.x += direction;

            GameObject clone = Instantiate(bulletBill, pos, Quaternion.identity);
            clone.GetComponent<Projectile>().Move(direction);


            if (GetComponent<SpriteRenderer>().isVisible)
            {
                if (direction == 1)
                    cloudR.Play();
                else
                    cloudL.Play();

                AudioManager.instance.Play("Fireworks");
            }

        }
    }
    void WatchPlayer()
    {
        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            if (transform.position.x > player.transform.position.x)
                direction = -1;
            else
                direction = 1;
        }
    }
}
