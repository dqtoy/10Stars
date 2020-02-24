using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LvTestScripts : MonoBehaviour
{
    private Player mainPlayerScript;

    void Start()
    {
        mainPlayerScript = GameManager.instance.mainPlayer.GetComponent<Player>();
    }

    void Update()
    {
        this.GetComponent<Text>().text = mainPlayerScript.lv.ToString();
    }
}
