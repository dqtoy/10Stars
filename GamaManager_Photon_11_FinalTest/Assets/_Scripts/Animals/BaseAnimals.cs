using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BaseAnimals : MonoBehaviourPun
{
    public float BaseHp;
    public float BaseMaxHp;
    public float BaseSpeed;
    public int BaseDamage;
    public float BaseExp;
    public bool BaseIsDead;
    public float RandomValue;

    public bool BaseIsViewHpBar;
    public float BaseIsViewHpTime;
}
