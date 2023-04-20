using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBrick : Block
{
    [SerializeField] bool hasCoin;

    [Header("Bools")]

    [SerializeField] bool hasStarted;
    [SerializeField] bool isOver;

    [Header("Times")]
    [SerializeField] float coinTime;
    [SerializeField]float destroyTime;

    public override void Start()
    {
        base.Start();
    }
    public override void Activate()
    {
        if (!isOn)
        {
            animator.SetTrigger("Activate");
            Action();
            if (hasCoin)
            {
                if (isOver)
                {
                    isOn = true;
                    animator.SetBool("On", isOn);
                }
                else if (!hasStarted)
                    StartCoroutine(CountDown());
            }
            else
            {
                isOn = true;
                animator.SetBool("On", isOn);
            }
        }
    }

    public override void Action()
    {
        if (hasCoin)
        {
            ParticleSystem particle = transform.parent.Find("ParticleCoin").GetComponent<ParticleSystem>();
            particle.Play();
            GameManager.instance.AddCoin();
        }
        else if (item != null)
        {
            Invoke("SpawnItem", item.GetComponent<ItemMovement>().Delay);
        }
        else
        {
            //Invoke("Destroy", destroyTime);
            StartCoroutine(Break());
        }
    }

    IEnumerator Break()
    {
        yield return null;
        animator.SetTrigger("Activate");
        AudioManager.instance.Play("Brick");
        ParticleSystem particle = transform.parent.Find("ParticleBrick").GetComponent<ParticleSystem>();
        particle.Play();
        Destroy(transform.parent.Find("ParticleCoin").gameObject);
        Destroy(transform.parent.Find("Block").gameObject);
        animation.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(transform.parent.Find("TriggerBlock").gameObject);
    }


    IEnumerator CountDown()
    {
        hasStarted = true;
        for (int i = 0; i < coinTime; i++)
            yield return new WaitForSeconds(1);
        isOver = true;

    }
    /*
    void Destroy()
    {
        animator.SetTrigger("Activate");
        AudioManager.instance.Play("Brick");
        ParticleSystem particle = transform.parent.Find("ParticleBrick").GetComponent<ParticleSystem>();
        particle.Play();
        Destroy(transform.parent.Find("ParticleCoin").gameObject);
        Destroy(transform.parent.Find("Block").gameObject);
        animation.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(transform.parent.Find("TriggerBlock").gameObject);
    }

    */
}
