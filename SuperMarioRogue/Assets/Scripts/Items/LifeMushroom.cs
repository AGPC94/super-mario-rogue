﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeMushroom : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.LifeMushroom();
            Destroy(gameObject);
        }
    }
}