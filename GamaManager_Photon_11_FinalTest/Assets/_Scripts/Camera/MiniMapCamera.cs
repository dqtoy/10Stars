using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapCamera : MonoBehaviour
{
    private GameObject mainPlayer;

    private Transform enemyPlayer;
    public RawImage miniMapOutline;

    private float minimapViewRange = 30f;
    public Vector3 viewEnemy;

    void Start()
    {
        mainPlayer = GameManager.instance.mainPlayer;
    }

    void FixedUpdate()
    {
        if (!enemyPlayer) enemyPlayer = GameManager.instance.enemyPlayer.transform;

        this.transform.position = new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y + minimapViewRange, mainPlayer.transform.position.z);

        if(CheckMinimapInEnemy(enemyPlayer.transform))
            miniMapOutline.color = Color.Lerp(miniMapOutline.color, Color.red, 0.2f);
        else
            miniMapOutline.color = Color.Lerp(miniMapOutline.color, Color.white, 0.2f);
    }

    public bool CheckMinimapInEnemy(Transform enemyPlayer)
    {
        viewEnemy = this.GetComponent<Camera>().WorldToViewportPoint(enemyPlayer.position);
        bool OnScreen = viewEnemy.z > 0 && viewEnemy.x > 0 && viewEnemy.x < 1 && viewEnemy.y > 0 && viewEnemy.y < 1;
        return OnScreen;
    }
}
