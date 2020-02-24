using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoseRankScoreAndResultText : MonoBehaviour
{
    private Text myRankScoreText;
    public Text myResultText;
    private float duringTime;

    public PlayerData playerData;
    public string path;
    public int minusScore;

    void Start()
    {
        SoundManager.instance.PlaySFX("lose");
        SoundManager.instance.PlayBGM("Lose");

        path = Path.Combine(Application.dataPath, "playerData.json");
        myRankScoreText = gameObject.GetComponent<Text>();

        if (File.Exists(path))
        {
            LoadPlayerDataFromJson();
        }

        LoseUpdate();
        SavePlayerDataToJson();
    }

    void Update()
    {
        myRankScoreText.text = "RankScore : " + playerData.rankScore.ToString();

        duringTime = Mathf.PingPong(Time.time, 1);
        myRankScoreText.color = Color.Lerp(Color.gray, Color.red, duringTime);
        myResultText.color = Color.Lerp(Color.red, Color.gray, duringTime);
    }

    public void LoseUpdate()
    {
        if (playerData.rankScore > 400) minusScore = 10;
        else if (playerData.rankScore > 300) minusScore = 8;
        else if (playerData.rankScore > 200) minusScore = 7;
        else if (playerData.rankScore > 100) minusScore = 6;
        else if (playerData.rankScore >= 4) minusScore = 4;
        else minusScore = playerData.rankScore;

        playerData.rankScore -= minusScore;
        playerData.loseCount += 1;
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
