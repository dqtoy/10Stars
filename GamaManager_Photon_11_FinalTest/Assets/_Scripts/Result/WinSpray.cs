using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinSpray : MonoBehaviour
{
    public int moveSpeed;

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);

        if (transform.position.x < -10)
        {
            moveSpeed = -moveSpeed;
        }
        else if (transform.position.x > 10)
        {
            moveSpeed = -moveSpeed;
        }
    }
}
