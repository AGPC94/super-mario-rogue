using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseWindow;
    [SerializeField] float delayTime = .5f;
    public bool IsPaused;
    public static PauseMenu instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        StartCoroutine(DelayResume());
    }

    IEnumerator DelayResume()
    {
        AudioManager.instance.Play("Pause");
        pauseWindow.GetComponent<Animator>().SetTrigger("In");
        yield return new WaitForSecondsRealtime(pauseWindow.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
        Time.timeScale = 1;
        IsPaused = false;
        AudioManager.instance.UnPauseMusic();
        pauseWindow.SetActive(false);
    }

    public void Pause()
    {
        AudioManager.instance.Play("Pause");
        pauseWindow.SetActive(true);
        Time.timeScale = 0;
        IsPaused = true;
        AudioManager.instance.PauseMusic();
        pauseWindow.GetComponent<Animator>().SetTrigger("Out");
        ForceSelectGameObject(pauseWindow.GetComponentInChildren<Button>().gameObject);
    }

    public void Quit()
    {
        AudioManager.instance.Play("Coin");
        SceneLoader.instance.LoadScene("MainMenu");
    }
    void ForceSelectGameObject(GameObject gameObject)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

}
