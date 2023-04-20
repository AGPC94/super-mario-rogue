using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    Bowser bowser;

    void OnTriggerEnter2D(Collider2D collision)
    {
        AudioManager.instance.Stop("Star");

        if (collision.CompareTag("Player"))
        {
            bowser = FindObjectOfType<Bowser>();

            if (bowser == null || bowser.health <= 0)
                GameManager.instance.NextLevel();
            else
                StartCoroutine(BeatBoss());

            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    IEnumerator BeatBoss()
    {
        Time.timeScale = 0;

        GameObject[] bridges = GameObject.FindGameObjectsWithTag("BossBridge");

        System.Array.Reverse(bridges);

        bowser.Stop();

        foreach (GameObject item in bridges)
        {
            Destroy(item.gameObject);
            AudioManager.instance.Play("Brick");
            yield return new WaitForSecondsRealtime(0.1f);
        }

        bowser.Die();

        AudioManager.instance.Play("BowserFalls");

        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;

        GameManager.instance.NextLevel();
    }
}
