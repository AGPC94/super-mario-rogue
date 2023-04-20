using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockQuestion : Block
{
    //protected GameObject itemGO;


    //[SerializeField] protected PowerUp powerUp;

    public override void Start()
    {
        base.Start();
    }
    public override void Activate()
    {
        if (!isOn)
        {
            animation.GetComponent<SpriteRenderer>().enabled = true;
            isOn = true;
            animator.SetBool("On", isOn);
            animator.SetTrigger("Activate");
            Action();
        }
    }

    public override void Action()
    {
        GetComponent<Collider2D>().enabled = true;

        if (item == null)
        {
            ParticleSystem particle = transform.parent.Find("ParticleCoin").GetComponent<ParticleSystem>();//GetComponentInChildren<ParticleSystem>();
            particle.Play();
            GameManager.instance.AddCoin();
            AudioManager.instance.Play("Coin");
        }
        else
        {
            //powerUpGo.GetComponent<PowerUpItem>().PowerUp = powerUp;
            Invoke("SpawnItem", item.GetComponent<ItemMovement>().Delay);
        }
    }

    public void SpawnItem()
    {
        Instantiate(item, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        if (item != null)
        {
            Gizmos.DrawIcon(transform.position, "Item", true, Color.yellow);
        }
    }
}
