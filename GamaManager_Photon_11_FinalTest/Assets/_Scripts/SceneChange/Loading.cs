using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Loading : MonoBehaviourPun
{
    public Image progressBar;

    void Start()
    {
        StartCoroutine("LoadScene");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    IEnumerator LoadScene()
    {
        while (true)
        {
            yield return null;
            if (progressBar.fillAmount < 1.0f)
                progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, 1.0f, Time.deltaTime);
            else break;
        }
        yield return new WaitForSeconds(1.5f);
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Main");
        }
    }
}
