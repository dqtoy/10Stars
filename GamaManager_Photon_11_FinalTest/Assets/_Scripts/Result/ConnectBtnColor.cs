using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectBtnColor : MonoBehaviour
{
    public Image connectBtnImage;
    public float duringTime;

    void Start()
    {
        connectBtnImage = gameObject.GetComponent<Image>();
    }

    void Update()
    {
        duringTime = Mathf.PingPong(Time.time, 1);
        connectBtnImage.color = Color.Lerp(Color.red, Color.blue, duringTime);
    }
}
