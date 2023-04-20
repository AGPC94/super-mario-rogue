using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    EventSystem eventSystem;
    GameObject lastSelectedObject;
    GameObject openMenu;

    [Header("Windows")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject LeaderBoards;
    [SerializeField] GameObject deleteWindow;

    [Header("Texts")]
    [SerializeField] Text txtRecord;

    void Awake()
    {
        openMenu = mainMenu;
        
        eventSystem = EventSystem.current;

        GameManager.instance.Reset();

        //Screen.SetResolution(1024, 960, false);
    }

    
    void Start()
    {
        AudioManager.instance.PlayMusic("MainMenu");

        if (PlayerPrefs.HasKey("Name"))
            txtRecord.text = "TOP - " + GameManager.instance.GetTopName() + " - " + GameManager.instance.GetRecord().ToString();
        else
            txtRecord.text = "TOP - SuperMario - 1-1";
    }

    void Update()
    {
        KeepSelectedButton();
    }

    void KeepSelectedButton()
    {
        if (eventSystem.currentSelectedGameObject == null)
            eventSystem.SetSelectedGameObject(lastSelectedObject); // no current selection, go back to last selected
        else
            lastSelectedObject = eventSystem.currentSelectedGameObject; // keep setting current selected object
    }

    public void Play()
    {
        mainMenu.SetActive(false);
        AudioManager.instance.StopMusic();
        SceneLoader.instance.LoadScene("Game");
    }

    public void OpenLeaderBoards()
    {
        OpenMenu(LeaderBoards);
        LeaderBoards.GetComponent<Animator>().SetTrigger("Down");

        PlayFabManager.instance.OpenLeaderBoard();
        //PlayFabManager.instance.GetLeaderBoardAroundPlayer();
    }
    public void CloseLeaderBoards()
    {
        OpenMenu(mainMenu);
        LeaderBoards.GetComponent<Animator>().SetTrigger("Up");
    }

    public void OpenMenu(GameObject menu)
    {
        //openMenu.SetActive(false);
        menu.SetActive(true);
        ForceSelectGameObject(menu.GetComponentInChildren<Button>().gameObject);
        //openMenu = menu;

    }

    public void OpenURL(string s)
    {
        Application.OpenURL(s);
    }

    void ForceSelectGameObject(GameObject gameObject)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

/*
    public void OpenWindowDelete()
    {
        deleteWindow.SetActive(true);
        ForceSelectGameObject(deleteWindow.GetComponentInChildren<Button>().gameObject);
    }
    public void CloseWindowDelete(bool delete = false)
    {
        if (delete)
            DeleteRecord();

        deleteWindow.SetActive(false);
        ForceSelectGameObject(LeaderBoards.GetComponentInChildren<Button>().gameObject);
    }
 */


/*
 * Abrir los leaderboards
 *  Si tengo usuario
 *      mostrar leaderboard alredador del jugador
 *  si no
 *      mostrar leaderboard en los primeros
 */