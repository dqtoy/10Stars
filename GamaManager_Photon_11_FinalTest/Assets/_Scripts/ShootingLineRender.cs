using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingLineRender : MonoBehaviour
{
    public LineRenderer shootingLineRender;

    private Vector2 nomalizedValue;

    void Start()
    {
        shootingLineRender = this.GetComponent<LineRenderer>();
        shootingLineRender.startWidth = 1;
        shootingLineRender.endWidth = 1;
    }

    void Update()
    {
        if(ShootJoystick.instance.isTouch)
        {
            shootingLineTrue();
        }
        else if(!ShootJoystick.instance.isTouch)
        {
            shootingLineFalse();
        }
    }

    public void shootingLineTrue()
    {
        shootingLineRender.enabled = true;
        nomalizedValue = ShootJoystick.instance.value.normalized;
        shootingLineRender.SetPosition(0, new Vector3(GameManager.instance.mainPlayer.transform.position.x, GameManager.instance.mainPlayer.transform.position.y + 0.5f, GameManager.instance.mainPlayer.transform.position.z));
        this.transform.position = new Vector3(GameManager.instance.mainPlayer.transform.position.x + nomalizedValue.x * 8, GameManager.instance.mainPlayer.transform.position.y + 0.5f, GameManager.instance.mainPlayer.transform.position.z + nomalizedValue.y * 8);
        shootingLineRender.SetPosition(1, this.transform.position);
    }

    public void shootingLineFalse()
    {
        shootingLineRender.enabled = false;
    }
}
