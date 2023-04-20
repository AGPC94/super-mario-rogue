using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickColor : MonoBehaviour
{
    [SerializeField] Color colorUnder = new Color(22, 89, 102, 100);
    [SerializeField] Color colorCastle = new Color(188, 188, 188, 100);

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem particle = GetComponent<ParticleSystem>();
        //Material material = GetComponent<ParticleSystem>().GetComponent<Renderer>().material;

        //SpriteRenderer renderer = GetComponent<ParticleSystem>().GetComponent<SpriteRenderer>();

        switch (GameManager.instance.levelType)
        {
            case LevelType.UNDERGROUND:
                /*
                material.EnableKeyword("CHANGECOLOR_ON");
                material.SetColor("_ColorChangeNewCol", colorUnder);
                */
                ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
                settings.startColor = new ParticleSystem.MinMaxGradient(colorUnder);
                break;

            case LevelType.CASTLE:
                /*
                material.EnableKeyword("CHANGECOLOR_ON");
                material.SetColor("_ColorChangeNewCol", colorCastle);
                */
                ParticleSystem.MainModule settings2 = GetComponent<ParticleSystem>().main;
                settings2.startColor = new ParticleSystem.MinMaxGradient(colorCastle);
                break;
        }



    }
}
