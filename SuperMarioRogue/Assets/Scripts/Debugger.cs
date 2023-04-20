using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] PowerUp[] powerUps;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        NextLevel();

        UseItem();

        DeletePlayerPrefs();
    }

    void NextLevel()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GameManager.instance.NextLevel();
    }

    void DeletePlayerPrefs()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
            PlayerPrefs.DeleteAll();
    }

    void UseItem()
    {
        Player player = FindObjectOfType<Player>();

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Debug.Log("SuperMushroom");
            GameManager.instance.Heal(1);
            //Sonido Champión
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Debug.Log("FireFlower");
            player.SetPowerUpWithTransformation(powerUps[0]);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Debug.Log("SuperLeaf");
            player.SetPowerUpWithTransformation(powerUps[1]);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Debug.Log("FrogSuit");
            player.SetPowerUpWithTransformation(powerUps[2]);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Debug.Log("HammerSuit");
            player.SetPowerUpWithTransformation(powerUps[3]);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            Debug.Log("SuperStar");
            player.StartStarEffect();
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Debug.Log("LifeMushroom");
            GameManager.instance.LifeMushroom();
        }
    }
}
