using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalWinSpray : MonoBehaviour
{
    public int moveSpeed;

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed * Time.deltaTime, transform.position.z);

        if(transform.position.y < -6)
        {
            moveSpeed = -moveSpeed;
        }
        else if(transform.position.y > 6)
        {
            moveSpeed = -moveSpeed;
        }
    }
}
