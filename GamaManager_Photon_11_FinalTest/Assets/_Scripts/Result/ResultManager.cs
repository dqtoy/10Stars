using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviourPunCallbacks
{
    public GameObject clickBtn;

    void Start()
    {
        //Screen.SetResolution(1920, 1080, true);
        StartCoroutine("DelaySetActiveBtn");
    }

    public void LeaveRoom()
    {
        SoundManager.instance.PlaySFX("upClick");
        PhotonNetwork.LeaveRoom();
        StartCoroutine("DelayLoadScene");
    }

    IEnumerator DelaySetActiveBtn()
    {
        yield return new WaitForSeconds(3.0f);
        clickBtn.SetActive(true);
    }

    IEnumerator DelayLoadScene()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Lobby");
    }
}
