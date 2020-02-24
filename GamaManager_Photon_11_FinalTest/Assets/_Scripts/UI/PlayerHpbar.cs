using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHpbar : MonoBehaviourPun
{
    private float viewHp;
    private GameObject mainPlayer;
    public Image hpBarImage;
    [SerializeField] private Text HpText;
    [SerializeField] private Text LvText;

    void Start()
    {
        mainPlayer = GameManager.instance.mainPlayer;
        viewHp = mainPlayer.GetComponent<Player>().maxHp;
    }

    void Update()
    {
        if (GameManager.instance.mainPlayer.GetPhotonView().IsMine)
        {
            viewHp = Mathf.Lerp(viewHp, mainPlayer.GetComponent<Player>().hp, 0.2f);
            hpBarImage.fillAmount = viewHp / mainPlayer.GetComponent<Player>().maxHp;
            HpText.text = mainPlayer.GetComponent<Player>().hp.ToString() + " / " + mainPlayer.GetComponent<Player>().maxHp.ToString();
            LvText.text = mainPlayer.GetComponent<Player>().lv.ToString();
        }
    }

}
