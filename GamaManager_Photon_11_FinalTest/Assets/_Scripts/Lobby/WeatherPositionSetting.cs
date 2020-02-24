using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherPositionSetting : MonoBehaviour
{
    public GameObject TargetPosition;

    void FixedUpdate()
    {
        transform.position = new Vector3(TargetPosition.transform.position.x, TargetPosition.transform.position.y + 20, TargetPosition.transform.position.z);
    }
}
