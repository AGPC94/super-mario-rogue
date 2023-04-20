using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneLoader.instance.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
