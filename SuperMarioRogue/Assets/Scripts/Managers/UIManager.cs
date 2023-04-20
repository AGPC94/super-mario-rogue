using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] Color[] livesColors;
    [SerializeField] Image imgLives;
    [SerializeField] Text txtLives;

    [Header("Max Lives")]
    [SerializeField] Image imgMaskLives;
    [SerializeField] Sprite max3;
    [SerializeField] Sprite max6;

    [Header("Coins")]
    [SerializeField] Text txtCoins;

    [Header("Level")]
    [SerializeField] Text txtLevel;

    // Update is called once per frame
    void Update()
    {
        //Lives
        imgLives.fillAmount = GameManager.instance.lives / GameManager.instance.maxLives;

        txtLives.text = GameManager.instance.lives.ToString();

        if (GameManager.instance.lives > 3)
            imgLives.color = livesColors[0];

        if (GameManager.instance.lives == 3)
            imgLives.color = livesColors[1];

        if (GameManager.instance.lives == 2)
            imgLives.color = livesColors[2];

        if (GameManager.instance.lives == 1)
            imgLives.color = livesColors[3];

        //MaxLives
        if (GameManager.instance.lives <= 3)
        {
            imgMaskLives.sprite = max3;
            GameManager.instance.SetMaxLives(3);
        }
        else
        {
            imgMaskLives.sprite = max6;
        }


        //Coins
        txtCoins.text = "x" + GameManager.instance.coins.ToString();

        //Level
        txtLevel.text = GameManager.instance.level.ToString();
    }
}