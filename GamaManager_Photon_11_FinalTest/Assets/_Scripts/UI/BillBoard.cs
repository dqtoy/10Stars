using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BillBoard : MonoBehaviourPun
{
    private Transform camTr;

    void Start()
    {
        camTr = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(camTr.position);
    }
}