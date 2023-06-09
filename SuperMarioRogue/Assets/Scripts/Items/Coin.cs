﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			GameManager.instance.AddCoin();
			AudioManager.instance.Play("Coin");
			Destroy(gameObject);
		}
	}
}