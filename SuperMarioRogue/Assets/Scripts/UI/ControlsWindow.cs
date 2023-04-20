using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsWindow : MonoBehaviour
{
    void Start()
    {
        if (!GameManager.instance.level.Equals(new Level(1, 1)))
            gameObject.SetActive(false);
    }
}
