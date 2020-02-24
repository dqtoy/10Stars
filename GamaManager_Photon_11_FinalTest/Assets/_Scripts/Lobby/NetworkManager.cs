using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DisconnectPanel")]
    public GameObject DisconnectPanel;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ListText;
    public Text RoomInfoText;
    public Text[] ChatText;
    public InputField ChatInput;
    public GameObject PlayerCharacter;
    public Text LoadingText;
    public Text ReadyText;
    public bool ReadyBtnClick = false;
    public Button ReadyBtn;
    public int ReadyPlayerCount;
    public bool StartRunning = false;
    public Image Player1CharacterImg;
    public Image Player2CharacterImg;

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    public bool playOneShot = true;
    public PlayerData playerData;
    public string path;
    public Text myRankScore;
    public float duringTime;

    public void LoadPlayerDataFromJson()
    {
        string path = Path.Combine(Application.dataPath, "playerData.json");
        string jsonData = File.ReadAllText(path);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);
    }

    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        SoundManager.instance.PlaySFX("upClick");

        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();

        myRankScore.rectTransform.localPosition = new Vector3(-430, 195, 0);
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion


    #region 서버연결
    void Awake()
    {
        //Screen.SetResolution(1920, 1080, true);

        path = Path.Combine(Application.dataPath, "playerData.json");
        ReadyBtn.enabled = false;
        Player1CharacterImg.enabled = false;
        Player2CharacterImg.enabled = false;

        PhotonNetwork.AutomaticallySyncScene = true;
        playOneShot = true;

        if (File.Exists(path))
        {
            LoadPlayerDataFromJson();
        }

        myRankScore.text ="RankScore : " + playerData.rankScore.ToString();
        myRankScore.text += '\n' + "Win : " + playerData.winCount + '\n' + "Lose : " + playerData.loseCount;

        myRankScore.rectTransform.localPosition = new Vector3(-695, 170, 0);
    }

    void Start()
    {
        SoundManager.instance.PlaySFX("appOpen");
        Invoke("LobbyBGMStart", 0.5f);
    }

    void LobbyBGMStart()
    {
        SoundManager.instance.PlayBGM("Lobby");
    }

    void Update()
    {
        duringTime = Mathf.PingPong(Time.time, 0.8f);
        myRankScore.color = Color.Lerp(Color.yellow, Color.red , duringTime);
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";

        if (ReadyPlayerCount == 2 && StartRunning == false)
        {
            StartCoroutine("StartCount");
        }

        // 방안일 때
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Player1CharacterImg.enabled = true;
                Player2CharacterImg.enabled = false;
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                Player1CharacterImg.enabled = true;
                Player2CharacterImg.enabled = true;
            }
        }

        // 이미 접속해 있는 상태라면
        if (PhotonNetwork.NetworkClientState.ToString() == "ConnectedToMaster" && playOneShot)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public void Connect()
    {
        SoundManager.instance.PlaySFX("upClick");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        playOneShot = false;
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        DisconnectPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = "용사(" + Random.Range(1, 1000) + ")";
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
    }

    public void Disconnect()
    {
        SoundManager.instance.PlaySFX("downClick");
        PhotonNetwork.Disconnect();
        myRankScore.rectTransform.localPosition = new Vector3(-695, 170, 0);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        DisconnectPanel.SetActive(true);
    }
    #endregion


    #region 방
    public void CreateRoom()
    {
        SoundManager.instance.PlaySFX("upClick");
        PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 2 });
        myRankScore.rectTransform.localPosition = new Vector3(-430, 195, 0);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        myRankScore.rectTransform.localPosition = new Vector3(-430, 195, 0);
    }

    public void LeaveRoom()
    {
        SoundManager.instance.PlaySFX("downClick");
        PhotonNetwork.LeaveRoom();
        myRankScore.rectTransform.localPosition = new Vector3(-695, 170, 0);
    }

    public override void OnJoinedRoom()
    {
        RoomPanel.SetActive(true);
        LobbyPanel.SetActive(false);
        DisconnectPanel.SetActive(false);
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }
    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        SoundManager.instance.PlaySFX("joinGameRoom");
        RoomRenewal();
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        SoundManager.instance.PlaySFX("leaveGameRoom");
        RoomRenewal();
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }

    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";

        // test
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            LoadingText.text = "상대 입장! 준비 해주세요!";
            ReadyBtn.enabled = true;
            ReadyText.color = Color.red;
            ReadyText.text = "준비 완료";
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            LoadingText.text = "상대를 기다리고 있습니다.";
            ReadyText.color = Color.blue;
            ReadyText.text = "대기 중";
        }
    }
    #endregion

    #region 방 접속
    public void Ready()
    {
        SoundManager.instance.PlaySFX("Ready");
        // 레디 취소
        if (ReadyBtnClick == true)
        {
            ReadyBtnClicked();
            ReadyText.color = Color.red;
            ReadyText.text = "준비 완료";
            LoadingText.text = "상대 입장! 준비 해주세요!";

            ReadyPlayerCount--;
            PV.RPC("ApplyReadyPlayerCount", RpcTarget.Others, ReadyPlayerCount);
            return;
        }

        LoadingText.text = "상대가 준비중 입니다! 잠시 기다려주세요.";
        ReadyPlayerCount++;
        ReadyBtnClicked();

        ReadyText.color = Color.white;
        ReadyText.text = "준비 취소";
        PV.RPC("ApplyReadyPlayerCount", RpcTarget.Others, ReadyPlayerCount);
    }

    [PunRPC]
    public void ApplyReadyPlayerCount(int NewReadyPlayerCount)
    {
        ReadyPlayerCount = NewReadyPlayerCount;
    }

    IEnumerator StartCount()
    {
        ReadyBtn.enabled = false;
        StartRunning = true;

        LoadingText.text = "모두 준비완료!";
        yield return new WaitForSeconds(1.0f);

        PhotonNetwork.CurrentRoom.IsOpen = false;

        LoadingText.text = "잠시 후 게임이 시작됩니다!";
        yield return new WaitForSeconds(1.0f);
        LoadingText.text = "3!";
        yield return new WaitForSeconds(1.0f);
        LoadingText.text = "2!";
        yield return new WaitForSeconds(1.0f);
        LoadingText.text = "1!";
        yield return new WaitForSeconds(1.0f);
        playOneShot = true;

        myRankScore.rectTransform.position = new Vector3(-695, 170, 0);

        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.LoadLevel("Main");
            PhotonNetwork.LoadLevel("Loading");
        }
        StartRunning = false;
    }

    public void ReadyBtnClicked()
    {
        ReadyBtnClick = !ReadyBtnClick;
    }

    #endregion
    #region 채팅
    public void Send()
    {
        string msg = PhotonNetwork.NickName + " : " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion
}
