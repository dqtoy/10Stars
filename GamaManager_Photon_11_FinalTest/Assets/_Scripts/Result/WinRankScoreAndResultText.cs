using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WinRankScoreAndResultText : MonoBehaviour
{
    private Text myRankScoreText;
    public Text myResultText;
    private float duringTime;

    public PlayerData playerData;
    public string path;
    public int plusScore;

    void Start()
    {
        SoundManager.instance.PlaySFX("win");
        SoundManager.instance.PlayBGM("Win");

        path = Path.Combine(Application.dataPath, "playerData.json");
        myRankScoreText = gameObject.GetComponent<Text>();

        if (File.Exists(path))
        {
            LoadPlayerDataFromJson();
        }
        Debug.Log("이김");
        WinUpdate();
        SavePlayerDataToJson();
    }

    void Update()
    {
        myRankScoreText.text = "RankScore : " + playerData.rankScore.ToString();

        duringTime = Mathf.PingPong(Time.time, 1);
        myRankScoreText.color = Color.Lerp(Color.green, Color.yellow, duringTime);
        myResultText.color = Color.Lerp(Color.yellow, Color.green, duringTime);
    }

    public void WinUpdate()
    {
        if (playerData.rankScore > 400) plusScore = 5;
        else if (playerData.rankScore > 300) plusScore = 6;
        else if (playerData.rankScore > 200) plusScore = 7;
        else if (playerData.rankScore > 100) plusScore = 8;
        else if (playerData.rankScore >= 0) plusScore = 10;

        playerData.rankScore += plusScore;
        playerData.winCount += 1;
    }

    public void SavePlayerDataToJson()
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(path, jsonData);
    }

    public void LoadPlayerDataFromJson()
    {
        string jsonData = File.ReadAllText(path);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
    }
}
