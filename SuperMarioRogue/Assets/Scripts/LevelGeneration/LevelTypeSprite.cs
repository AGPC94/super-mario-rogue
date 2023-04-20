using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTypeSprite : MonoBehaviour
{
    [SerializeField] Sprite[] sprites = new Sprite[3];

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        switch (GameManager.instance.levelType)
        {
            case LevelType.GROUND:
                renderer.sprite = sprites[0];
                break;

            case LevelType.UNDERGROUND:
                renderer.sprite = sprites[1];
                break;

            case LevelType.CASTLE:
                renderer.sprite = sprites[2];
                break;

            case LevelType.ANY:
                renderer.sprite = sprites[0];
                break;

            case LevelType.ISLAND:
                renderer.sprite = sprites[0];
                break;
        }

    }
}
