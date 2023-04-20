using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaPlant : MonoBehaviour
{
    //public PipeTrigger pipeTrigger;
    public bool isRunning;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //anim.SetBool("move", !pipeTrigger.isTriggered && !isRunning);
    }

    void SetRunning(int n)
    {
        isRunning = n == 0 ? false : true;
    }
}
