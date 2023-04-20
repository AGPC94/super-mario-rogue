using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    protected Animator animator;
    protected bool isOn;
    protected Transform animation;
    public GameObject item;

    public virtual void Start()
    {
        animation = transform.parent.Find("Animation");
        animator = animation.GetComponent<Animator>();
        //itemGO = Resources.Load("PowerUpItem") as GameObject;
    }

    public virtual void Activate()
    {

    }

    public virtual void Action()
    {

    }
}
