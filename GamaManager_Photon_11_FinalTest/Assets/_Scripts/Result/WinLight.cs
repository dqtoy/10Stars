using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLight : MonoBehaviour
{
    public Light light;
    public float duringTime;

    void Start()
    {
        light = this.GetComponent<Light>();
    }

    void Update()
    {
        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 5 * Time.deltaTime, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(0, 0, -20);
            duringTime = Mathf.PingPong(Time.time, 1);
            light.spotAngle = Mathf.Lerp(20, 70, duringTime);
        }
    }
}
