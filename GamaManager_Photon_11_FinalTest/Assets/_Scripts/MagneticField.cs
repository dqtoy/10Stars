using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MagneticField : MonoBehaviourPun
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player") other.gameObject.GetComponent<Player>().isInCircle = true;
        if (other.tag == "Animals") other.gameObject.GetComponent<Wolf>().isInCircle = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") other.gameObject.GetComponent<Player>().isInCircle = false;
        if (other.tag == "Animals") other.gameObject.GetComponent<Wolf>().isInCircle = false;
    }
}

