using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIShootingCounts : MonoBehaviourPun
{
    public ShootJoystick SJ;
    public Image ShootingCountsImage;

    private void Start()
    {
        SJ = GameObject.Find("ShootingJoystickBG").GetComponent<ShootJoystick>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SendCountUI", RpcTarget.All, SJ.shootingCounts, (float)SJ.maxBullets, SJ.countCoolTime, SJ.shootingCoolTime);
        }
    }

    [PunRPC]
    public void SendCountUI(float newShootingCounts, float newMaxBullets, float newCountcoolTime, float newShootingCoolTime)
    {
        ShootingCountsImage.fillAmount = (newShootingCounts / newMaxBullets) + (newCountcoolTime / newShootingCoolTime) / newMaxBullets;
    }
}
