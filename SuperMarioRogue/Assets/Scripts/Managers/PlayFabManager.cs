using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System;

public class PlayFabManager : MonoBehaviour
{
    [SerializeField] GameObject rowPrefab;
    [SerializeField] Transform rowsParent;
    [SerializeField] Color highlightColor;

    string loggedInPlayFabID;
    string displayName;

    public static PlayFabManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSucess, OnError);
    }

    void OnLoginSucess(LoginResult result)
    {
        loggedInPlayFabID = result.PlayFabId;
        if (result.InfoResultPayload.PlayerProfile != null)
            displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
        Debug.Log("Successful login/account create!");
    }

    public void SubmitName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name!");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account!");
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "Level", Value = score}
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    public void OpenLeaderBoard()
    {
        if (displayName == null)
            GetLeaderBoard();
        else
            GetLeaderBoardAroundPlayer();
    }

    void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Level",
            StartPosition = 0,
            MaxResultsCount = 100
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void GetLeaderBoardAroundPlayer()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Level",
            MaxResultsCount = 100
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardAroundPlayerGet, OnError);
    }

    void OnLeaderboardAroundPlayerGet(GetLeaderboardAroundPlayerResult result)
    {
        foreach (Transform item in rowsParent)
            Destroy(item.gameObject);

        foreach (var item in result.Leaderboard)
        {
            GameObject rowGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = rowGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = GameManager.instance.LoadLevel(item.StatValue);

            if (item.PlayFabId == loggedInPlayFabID)
            {
                rowGo.GetComponent<Image>().color = highlightColor;
                Debug.Log("Ilumina jugador");
            }

            Debug.Log($"{item.Position} {item.PlayFabId} {item.StatValue}");
        }
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfull leaderboard sent");
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (Transform item in rowsParent)
            Destroy(item.gameObject);

        foreach (var item in result.Leaderboard)
        {
            GameObject rowGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = rowGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            Debug.Log($"{item.Position} {item.PlayFabId} {item.StatValue}");
        }
    }

    
}

/*
 

    void GetPlayerProfile(string playFabId)
    {
        var request = new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(request, OnGetPlayerProfile, OnError);
    }

    void OnGetPlayerProfile(GetPlayerProfileResult result)
    {
        //result.PlayerProfile.DisplayName
    }
 
 */