using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerExpbar : MonoBehaviourPun
{
    private float viewExp;
    private GameObject mainPlayer;
    public Image expBarImage;

    void Start()
    {
        mainPlayer = GameManager.instance.mainPlayer;
        viewExp = mainPlayer.GetComponent<Player>().Exp;
    }

    void Update()
    {
        if (GameManager.instance.mainPlayer.GetPhotonView().IsMine)
        {
            viewExp = Mathf.Lerp(viewExp, mainPlayer.GetComponent<Player>().Exp, 0.2f);
            expBarImage.fillAmount = viewExp / mainPlayer.GetComponent<Player>().maxExp;
        }
    }
}
