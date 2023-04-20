using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;

    [SerializeField] Text txtWorld;

    [SerializeField] Animator transition;

    public static SceneLoader instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadAsynchronously(scene));
    }

    IEnumerator LoadAsynchronously(string scene)
    {
        Time.timeScale = 0;
        transition.SetTrigger("End");

        yield return new WaitForSecondsRealtime(transition.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);

        loadingScreen.SetActive(true);

        txtWorld.text = "World " + GameManager.instance.level.ToString();

        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }

        Time.timeScale = 1;
        transition.SetTrigger("Start");
        loadingScreen.SetActive(false);
    }
}
