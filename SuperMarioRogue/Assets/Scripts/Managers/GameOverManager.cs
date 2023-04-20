using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject scoreEntry;

    [SerializeField] Text txtLevel;
    [SerializeField] InputField ifName;

    // Start is called before the first frame update
    void Start()
    {
        scoreEntry.SetActive(false);

        if (GameManager.instance.IsANewRecord())
        {
            AudioManager.instance.Play("StageClear");
            ShowNameEntry();
        }
        else
        {
            AudioManager.instance.Play("GameOver");
            Invoke("GoToMainMenu", 5);
        }

        txtLevel.text = "WORLD. . . . . . . . " + GameManager.instance.level.ToString();
    }

    void ShowNameEntry()
    {
        scoreEntry.SetActive(true);
        ifName.Select();
        ifName.ActivateInputField();
    }

    void GoToMainMenu()
    {
        AudioManager.instance.Stop("GameOver");
        AudioManager.instance.Stop("StageClear");
        SceneLoader.instance.LoadScene("MainMenu");
    }

    public void SubmitName(string s)
    {
        if (s != string.Empty)
        {
            if (GameManager.instance.IsANewRecord())
                GameManager.instance.SetRecord(s);

            PlayFabManager.instance.SubmitName(s);
            PlayFabManager.instance.SendLeaderBoard(GameManager.instance.ParseLevelToInt());

            GoToMainMenu();
            Debug.Log(s);
        }
    }
}