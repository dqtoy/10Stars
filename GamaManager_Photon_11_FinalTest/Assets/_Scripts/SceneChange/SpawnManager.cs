using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviourPun
{
    public GameObject mainPlayerPrefab;
    public GameObject mainPlayerPrefab2;

    //Spawn
    public GameObject[] wayPoint1;
    public GameObject[] wayPoint2;

    void Awake()
    {
        if(PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer)
            GameManager.instance.mainPlayer = PhotonNetwork.Instantiate(mainPlayerPrefab.name, wayPoint1[Random.Range(0, 3)].transform.position, Quaternion.identity); 
        else
            GameManager.instance.mainPlayer = PhotonNetwork.Instantiate(mainPlayerPrefab2.name, wayPoint2[Random.Range(0, 3)].transform.position, Quaternion.identity);
    }
}
